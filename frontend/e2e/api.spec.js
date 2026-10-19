import { expect, test } from '@playwright/test'

// La api tiene su propia dirección: NO es la del front. Sin barra al final.
const API = process.env.API_BASE_URL || 'http://localhost:8080'

// Usuario fijo de pruebas: si no existe se registra, si existe se loguea.
// Así no se llena la base de QA de usuarios nuevos en cada corrida.
const USER = { email: 'tests-integracion@appgastos.test', password: 'Prueba1234!', name: 'Usuario de pruebas' }

const hoy = () => new Date().toISOString().slice(0, 10) // fecha de hoy, en UTC

async function obtenerToken (request) {
  const login = await request.post(`${API}/api/auth/login`, { data: { email: USER.email, password: USER.password } })
  if (login.ok()) return (await login.json()).token
  const registro = await request.post(`${API}/api/auth/register`, { data: USER })
 expect(registro.ok(), `no se pudo registrar: ${registro.status()} ${await registro.text()}`).toBeTruthy()
  return (await registro.json()).token
}

async function listar (request, headers) {
  const r = await request.get(`${API}/api/expenses`, { headers })
  expect(r.status()).toBe(200)
  return await r.json()
}

test('el alta de un gasto se guarda en la base de verdad, y el borrado lo saca', async ({ request }) => {
  const headers = { Authorization: `Bearer ${await obtenerToken(request)}` }
  const nombre = `api ${Date.now()}` // único: no choca con otra corrida

  const cat = await request.post(`${API}/api/categories`, { headers, data: { name: nombre, monthlyBudget: null } })
  expect(cat.ok()).toBeTruthy()
  const categoria = await cat.json()

  const alta = await request.post(`${API}/api/expenses`, {
    headers,
    data: { categoryId: categoria.id, amount: 123.45, description: nombre, date: hoy() }
  })
  expect(alta.status()).toBe(201) // la api dice «creado»
  const creado = await alta.json()

  const lista = await listar(request, headers) // y la BASE lo devuelve: no es un doble
  expect(lista.some(g => g.id === creado.id)).toBe(true)

  const borrado = await request.delete(`${API}/api/expenses/${creado.id}`, { headers })
  expect(borrado.status()).toBe(204)

  const despues = await listar(request, headers) // y el borrado, comprobado
  expect(despues.some(g => g.id === creado.id)).toBe(false)

  const borraCat = await request.delete(`${API}/api/categories/${categoria.id}`, { headers }) // limpia lo que creó
  expect(borraCat.ok()).toBeTruthy()
})

test('un gasto con monto 0 lo rechaza la api, y no crea nada', async ({ request }) => {
  const headers = { Authorization: `Bearer ${await obtenerToken(request)}` }
  const nombre = `api ${Date.now()}`

  const cat = await request.post(`${API}/api/categories`, { headers, data: { name: nombre, monthlyBudget: null } })
  expect(cat.ok()).toBeTruthy()
  const categoria = await cat.json()

  const antes = await listar(request, headers)

  const alta = await request.post(`${API}/api/expenses`, {
    headers,
    data: { categoryId: categoria.id, amount: 0, description: nombre, date: hoy() }
  })
  expect(alta.status()).toBe(400) // el contrato del error

  const despues = await listar(request, headers)
  expect(despues.length).toBe(antes.length) // y de verdad no se creó nada

  const borraCat = await request.delete(`${API}/api/categories/${categoria.id}`, { headers }) // limpia
  expect(borraCat.ok()).toBeTruthy()
})

test('sin token la api no devuelve los gastos de nadie', async ({ request }) => {
  const sinToken = await request.get(`${API}/api/expenses`)
  expect(sinToken.status()).toBe(401)

  const tokenFalso = await request.get(`${API}/api/expenses`, { headers: { Authorization: 'Bearer esto.no.es.un.token' } })
  expect(tokenFalso.status()).toBe(401)
})