import { expect, test } from '@playwright/test'

const API = process.env.API_BASE_URL || 'http://localhost:8080'
const USER = { email: 'tests-e2e@appgastos.test', password: 'Prueba1234!', name: 'Usuario e2e' }

// Deja el usuario de pruebas listo y le crea una categoría (por la api: no es lo que se prueba acá).
async function prepararCategoria (request) {
  let login = await request.post(`${API}/api/auth/login`, { data: { email: USER.email, password: USER.password } })
  if (!login.ok()) {
    login = await request.post(`${API}/api/auth/register`, { data: USER })
    expect(login.ok(), `no se pudo registrar: ${login.status()} ${await login.text()}`).toBeTruthy()
  }
  const headers = { Authorization: `Bearer ${(await login.json()).token}` }
  const nombre = `e2e ${Date.now()}` // único: no choca con otra corrida
  const cat = await request.post(`${API}/api/categories`, { headers, data: { name: nombre, monthlyBudget: null } })
  expect(cat.ok()).toBeTruthy()
  return { nombre, id: (await cat.json()).id, headers }
}

async function iniciarSesion (page, password = USER.password) {
  await page.goto('/')
  await page.getByLabel('Email').fill(USER.email)
  await page.getByLabel('Contraseña').fill(password)
  // hay dos botones "Ingresar" (la pestaña y el de enviar): el de enviar es el último
  await page.locator('form').getByRole('button', { name: 'Ingresar' }).last().click()
}

async function cargarGasto (page, categoria, descripcion) {
  await page.getByRole('link', { name: 'Gastos', exact: true }).click()
  await page.getByLabel('Categoría').selectOption({ label: categoria })
  await page.getByLabel('Monto').fill('123.45')
  await page.getByLabel('Descripción').fill(descripcion)
  await page.getByRole('button', { name: 'Agregar gasto' }).click()
  const fila = page.getByRole('row').filter({ hasText: descripcion })
  await expect(fila).toBeVisible() // el dato que ESTE test creó, sin sleeps
  return fila
}

test('cargar un gasto lo muestra en la tabla, y eliminarlo lo saca', async ({ page, request }) => {
  const { nombre, id, headers } = await prepararCategoria(request)
  const descripcion = `e2e ${Date.now()}`
  try {
    await iniciarSesion(page)
    const fila = await cargarGasto(page, nombre, descripcion)
    await fila.getByRole('button', { name: 'Eliminar' }).click()
    await expect(fila).toHaveCount(0) // y el borrado, comprobado
  } finally {
    await request.delete(`${API}/api/categories/${id}`, { headers }) // limpia la categoría
  }
})

test('con una contraseña incorrecta el usuario ve el error y no entra', async ({ page }) => {
  await iniciarSesion(page, 'incorrecta123')
  await expect(page.getByRole('alert')).toContainText('incorrectos') // el usuario ve el error…
  await expect(page).toHaveURL(/\/login/) // …y no entró
  await expect(page.getByRole('link', { name: 'Gastos', exact: true })).toHaveCount(0)
})

test('un gasto cargado sigue ahí después de recargar la página', async ({ page, request }) => {
  const { nombre, id, headers } = await prepararCategoria(request)
  const descripcion = `e2e ${Date.now()}`
  try {
    await iniciarSesion(page)
    const fila = await cargarGasto(page, nombre, descripcion)
    await page.reload() // lo que vuelve a pedirle a la base
    await expect(fila).toBeVisible()
    await fila.getByRole('button', { name: 'Eliminar' }).click()
    await expect(fila).toHaveCount(0)
  } finally {
    await request.delete(`${API}/api/categories/${id}`, { headers })
  }
})
