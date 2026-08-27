# AppGastos

[![CI](https://github.com/MateoAlfei/ingsoft3-tp01/actions/workflows/ci.yml/badge.svg)](https://github.com/MateoAlfei/ingsoft3-tp01/actions/workflows/ci.yml)

Aplicación de gestión de gastos personales — backend en .NET 8, frontend en React + TypeScript + Vite, base de datos PostgreSQL. Contenerizada con Docker.

## Instalación

```bash
git clone https://github.com/MateoAlfei/ingsoft3-tp01
cd ingsoft3-tp01
```

## Levantar el sistema (build local)

Requiere [Docker Desktop](https://docs.docker.com/get-docker/) instalado y corriendo.

1. Copiar la plantilla de variables de entorno:

```bash
cp .env.example .env
```

2. (Opcional) Editar `.env` y poner una contraseña propia para `DB_PASSWORD`.

3. Levantar todo el sistema:

```bash
docker compose up -d --build
```

4. Verificar que los tres servicios estén corriendo:

```bash
docker compose ps
```

5. Abrir la aplicación en el navegador:

git clone https://github.com/MateoAlfei/ingsoft3-tp01
# AppGastos

Aplicación de gestión de gastos personales — backend en .NET 8, frontend en React + TypeScript + Vite, base de datos PostgreSQL. Contenerizada con Docker.

## Instalación

```bash
git clone https://github.com/MateoAlfei/ingsoft3-tp01
cd ingsoft3-tp01
```

## Levantar el sistema (build local)

Requiere [Docker Desktop](https://docs.docker.com/get-docker/) instalado y corriendo.

1. Copiar la plantilla de variables de entorno:

```bash
cp .env.example .env
```

2. (Opcional) Editar `.env` y poner una contraseña propia para `DB_PASSWORD`.

3. Levantar todo el sistema:

```bash
docker compose up -d --build
```

4. Verificar que los tres servicios estén corriendo:

```bash
docker compose ps
```

5. Abrir la aplicación en el navegador: **http://localhost:3000**

## Levantar el sistema desde imágenes publicadas (sin el código)

Usa las imágenes ya publicadas en GitHub Container Registry, sin necesidad de compilar nada localmente.

```bash
cp .env.example .env
docker compose -f docker-compose.registry.yml up -d
```

## Apagar el sistema

```bash
docker compose down
```

Para borrar también los datos persistidos (la base de datos vuelve a nacer vacía):

```bash
docker compose down -v
```

## Stack técnico

- **Backend**: .NET 8 (`AppGastos.Api`), Entity Framework Core, PostgreSQL, autenticación JWT
- **Frontend**: React 19 + TypeScript + Vite, servido en producción por nginx
- **Base de datos**: PostgreSQL 16 (alpine)
- **Contenerización**: Docker multi-stage builds + Docker Compose
- **Registry**: [GitHub Container Registry (ghcr.io)](https://github.com/mateoalfei?tab=packages)

## Documentación adicional

- [`decisiones.md`](./decisiones.md) — decisiones técnicas y de arquitectura
- [`evidencias.md`](./evidencias.md) — evidencias de funcionamiento (persistencia, tamaños de imagen, publicación)
