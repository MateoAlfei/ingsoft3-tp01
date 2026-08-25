# Decisiones — TP1

## 1. Por qué Git no pudo resolver el conflicto solo

Las ramas A y B partieron del mismo commit en `main`, y ambas modificaron la primera línea del
README con contenido distinto (versión A vs. versión B). Git fusiona automáticamente cuando dos
ramas tocan partes distintas de un archivo, pero cuando ambas cambian exactamente la misma línea
no tiene forma de decidir cuál versión es "la correcta" — eso es una decisión de contenido, no
algo resoluble con un algoritmo. Por eso Git se detiene y me delega la decisión, marcando el
archivo con los delimitadores `<<<<<<<`, `=======` y `>>>>>>>`.

Para que este conflicto nunca hubiera aparecido, alguna de las dos ramas tendría que haberse
integrado a `main` antes de que la otra empezara a trabajar sobre esa misma línea. Por ejemplo,
si la rama B hubiera partido de `main` recién después de que A ya estuviera mergeada, habría
heredado el cambio de A en vez de pisarlo.

## 2. Qué problemas encontré y cómo los solucioné

- **URL de clone con los signos `<` y `>` literales**: al copiar el comando `git clone` de la guía
  dejé `<MateoAlfei>` tal cual, sin reemplazar el placeholder por mi usuario real. Git tiró error
  400 porque esa no es una URL válida. Lo solucioné sacando los signos `<>` y dejando la URL
  limpia: `https://github.com/MateoAlfei/ingsoft3-tp01.git`.

- **`&&` no funciona en PowerShell**: intenté encadenar `git switch main && git pull` como se hace
  habitualmente en bash, y PowerShell tiró un error de sintaxis (`El token '&&' no es un separador
  de instrucciones válido en esta versión`). Lo solucioné poniendo cada comando en su propia línea.

- **Nombre de rama no convencional**: al editar el README desde la web de GitHub, la plataforma
  creó automáticamente una rama llamada `MateoAlfei-patch-1` en vez de seguir la convención
  `feature/<descripcion>` que pide la guía en §4.9. Me di cuenta después de crear el PR, cuando ya
  no tenía sentido rehacerlo — lo dejo documentado acá como aprendizaje: la próxima vez voy a
  crear la rama manualmente con el nombre correcto antes de editar el archivo.

## 3. Declaración de uso de IA

Usé Claude (Anthropic) como asistente durante todo el desarrollo del TP, principalmente para:
- Entender los comandos de Git de cada paso (clone, add, commit, push, tag, resolución de
  conflictos) y el porqué detrás de cada uno.
- Resolver errores puntuales que me fueron apareciendo (URL mal copiada, sintaxis de `&&` en
  PowerShell, organización de las capturas en `img/`).
- Estructurar el contenido de `evidencias.md` y este mismo archivo.

Verifiqué cada sugerencia ejecutándola yo mismo y confirmando el resultado en la terminal o en
GitHub antes de avanzar al paso siguiente — no copié nada sin ver que funcionara. Las decisiones
de flujo de trabajo (branching, squash and merge, protección de rama) las definió la cátedra en
la guía; usé la IA como apoyo para ejecutar los pasos correctamente y entender los conceptos, no
para tomar esas decisiones, por lo que puedo explicarlas en la defensa oral sin depender de ella.


---

## TP2 — Contenedores

### 1. Elección de la app del semestre

Elegí **AppGastos**, mi propio proyecto (backend .NET 8 + frontend React/TypeScript + PostgreSQL),
contra los criterios de la guía:

- **¿Buildea y corre localmente sin magia?** Sí — ya lo tenía corriendo nativo antes de empezar
  este TP (backend en `dotnet run`, base en contenedor Postgres suelto).
- **¿Tiene o puedo escribirle tests?** Todavía no tiene tests, pero la estructura (EF Core con
  DbContext separado, minimal API) permite agregarlos sin refactor grande — lo dejo para el TP5.
- **¿Entiendo el código lo suficiente como para modificarlo?** Sí
- **Tamaño**: cumple con CRUD + pocas pantallas (usuarios, categorías, gastos, dashboard) — no se
  pasa de lo que pide la guía.

### 2. Decisiones de contenerización

- **Imágenes base**: `mcr.microsoft.com/dotnet/sdk:8.0` / `aspnet:8.0` para el backend (coincide
  con el `TargetFramework net8.0` fijado también por `global.json`); `node:22-alpine` para buildear
  el frontend y `nginx:alpine` para servirlo — mismas imágenes que usé en la práctica sobre el
  sample de la cátedra.
- **Estructura multi-stage**: en ambos Dockerfiles, una etapa de build (SDK/Node) y una etapa final
  liviana (runtime/nginx) que solo copia los artefactos ya compilados. Resultado: imagen final del
  backend de **335MB** contra los **1.2GB** de la imagen del SDK.
- **Qué persiste y qué no**: los datos de PostgreSQL viven en un volumen nombrado (`db_data`),
  declarado en `docker-compose.yml`. Sobreviven a `docker compose down`, se destruyen con `down -v`
  (verificado en `evidencias.md`). El resto del sistema (backend, frontend) es efímero por diseño:
  se puede recrear en cualquier momento sin pérdida de información.
- **Migraciones automáticas**: el backend aplica `db.Database.Migrate()` al arrancar, así que la
  base nace vacía en el contenedor y las tablas (`Users`, `Categories`, `Expenses`) se crean solas
  la primera vez que el backend levanta contra una base nueva — no hace falta correr nada a mano.
- **Frontend con proxy en nginx**: el frontend llama a `/api/...` con rutas relativas (`BASE_URL =
  "/api"` en `client.ts`), así que en el contenedor `nginx.conf` reenvía ese prefijo al servicio
  `backend` de la red de compose. Mismo enfoque que recomienda la guía en §2.6(a): sin CORS, misma
  imagen sirve en cualquier entorno.
- **Arquitectura de las imágenes publicadas**: construidas en Windows con Docker Desktop, resultan
  en `linux/amd64` (confirmado con `docker version`) — compatibles con los runners de GitHub
  Actions que se van a usar en el TP7.

### 3. Problemas encontrados y cómo los resolví

- **`host.docker.internal` resuelve por IPv6 en Docker Desktop para Windows**: al correr el backend
  en contenedor suelto contra una base en el host, la conexión fallaba primero con
  `Network is unreachable` (intentaba conectar a una IP IPv6) y después con `Connection refused`
  incluso usando `--add-host=host.docker.internal:host-gateway`. Lo resolví evitando el problema de
  raíz: conecté el contenedor del backend a una red Docker compartida con el contenedor de la base
  (`docker network create` + `docker network connect`), y usé el nombre del contenedor de la base
  como host en la connection string, en vez de pasar por el host de Windows. Este es exactamente el
  mismo mecanismo que después uso en `docker-compose.yml` (red interna con DNS por nombre de
  servicio), así que el problema terminó siendo una buena introducción al concepto de §2.6.

- **Puerto 8080 en conflicto con mi propio backend nativo**: al intentar levantar el backend en
  contenedor, chocaba con `AppGastos.Api` corriendo nativamente en mi máquina en el mismo puerto.
  Lo identifiqué con `Get-NetTCPConnection -LocalPort 8080` + `Get-Process`, y lo resolví frenando
  el proceso nativo antes de correr el contenedor.

- **Error transitorio al publicar la imagen del backend en ghcr**: el primer `docker push` del
  backend falló con `error from registry: unknown` después de montar varias capas ya existentes de
  otra imagen mía. Reintenté el mismo comando y esta vez completó sin problema (`Layer already
  exists` para todas las capas) — quedó documentado como un fallo transitorio del registry, no de
  mi configuración.

### 4. Declaración de uso de IA

Usé Claude (Anthropic) como asistente durante todo el desarrollo del TP, tanto en la práctica sobre
el sample de la cátedra como en la aplicación real sobre AppGastos, principalmente para:
- Entender el porqué de cada instrucción de los Dockerfiles (multi-stage, orden de capas, por qué
  el SDK no viaja a producción) y de las claves de `docker-compose.yml` (healthcheck, depends_on,
  volúmenes nombrados vs. bind mounts).
- Diagnosticar y resolver los errores reales que fueron apareciendo (IPv6 de
  `host.docker.internal`, puertos ocupados, `.env` mal armado, error transitorio de push).
- Adaptar los archivos de la guía (pensados para el sample `demo-fullstack`) a la estructura real
  de mi proyecto (proyecto único en vez de solución multi-proyecto, connection string con nombre
  `appgastos`, migraciones EF en vez de `EnsureCreated`, endpoint `/api/health` en vez de `/health`).
- Estructurar este archivo y `evidencias.md`.

Verifiqué cada paso ejecutándolo yo mismo en mi terminal y confirmando el resultado (build exitoso,
`curl` respondiendo, interfaz cargando, persistencia de datos) antes de avanzar al siguiente.


---

## TP3 — Planificación y trazabilidad

### 1. Duración del sprint

Elegí un sprint de **1 semana**. La razón principal es alinear el ritmo de planificación con
el ritmo real de avance de la materia: al trabajar solo (sin necesidad de sincronizar con un
equipo), un sprint más corto me permite revisar mi propio progreso con más frecuencia y
reajustar el plan si algo no sale como esperaba, en vez de esperar semanas para darme cuenta de
un desvío. Descarté sprints más largos (2-3 semanas) porque, para trabajo individual, alargan
innecesariamente el ciclo de feedback sin aportar ningún beneficio de coordinación (que sí
tendría sentido en un equipo grande).

### 2. Límite de trabajo en progreso

Configuré el límite en **2** para la columna *In Progress*. Sigo la regla de arranque que
sugiere la guía: personas trabajando + 1. Trabajando solo, eso da 1 + 1 = 2. El "+1" funciona
como válvula: si algo queda esperando (por ejemplo, una revisión propia pendiente o un bloqueo
externo), puedo avanzar en otra cosa sin quedarme completamente detenido, pero sin caer en la
tentación de abrir tres o cuatro frentes en paralelo — que es exactamente lo que el límite
busca evitar (trabajo empezado y no terminado, que es inventario, no avance real).

### 3. Diagnóstico de la historia mal escrita

La historia de ejemplo — *"Como desarrollador quiero crear la tabla usuarios"* — está mal
escrita por dos motivos relacionados. El más superficial es que le falta el "para", la parte
que justifica el beneficio. Pero el problema de fondo es más importante: es una **tarea
disfrazada de historia**, no una historia real. El beneficiario de "crear una tabla" no es un
usuario de la aplicación — es el propio desarrollador o el sistema; ningún usuario final
percibe ni le importa que exista una tabla llamada `usuarios` en la base de datos, es un
detalle interno de implementación. La reescribiría subiendo un nivel de abstracción a la
necesidad real del usuario, por ejemplo: *"Como usuario quiero registrarme con mi email y
contraseña para tener mi propia cuenta en la app"* — ahí sí hay alguien que se beneficia de
forma observable. "Crear la tabla usuarios" pasaría a ser una de las tareas técnicas
necesarias para cumplir esa historia, no la historia en sí.

### 4. Problemas encontrados y cómo los resolví

No encontré complicaciones técnicas relevantes durante este TP — el flujo de crear issues,
vincular sub-issues, configurar el board, el sprint y el límite de trabajo en progreso, y cerrar
una tarea automáticamente desde un Pull Request, funcionó siguiendo los pasos de la guía sin
inconvenientes. Verifiqué cada checkpoint manualmente antes de avanzar al siguiente (proyecto
público en ventana de incógnito, jerarquía navegable en la épica y la historia, tarea cerrada
automáticamente y movida a Done en el tablero).

### 5. Declaración de uso de IA

Usé Claude (Anthropic) como asistente durante todo el desarrollo del TP, principalmente para:
- Entender la diferencia entre épica, historia y tarea, y por qué la jerarquía de tres niveles
  existe (qué pregunta responde cada nivel).
- Diagnosticar la historia mal escrita del §3.2 — a partir de la pista de "quién es el
  beneficiario", llegué yo mismo a la conclusión de que era una tarea disfrazada de historia,
  y la IA confirmó y completó el razonamiento.
- Guiarme paso a paso por la interfaz de GitHub Projects (crear el proyecto, hacerlo público,
  crear sub-issues, configurar el campo Iteration, el workflow automático, el límite de
  trabajo en progreso) ya que era mi primera vez usando esta herramienta.
- Explicar por qué el `Closes #N` tiene que ir en la descripción del PR (y no en un comentario
  posterior) y por qué referencia a la tarea y no a la historia.

Verifiqué cada paso ejecutándolo yo mismo en la web de GitHub y confirmando el resultado
(proyecto público accesible sin sesión, jerarquía visible con sub-issues, tarjetas en el
tablero, tarea cerrada automáticamente tras el merge del PR) antes de avanzar al siguiente. Las
decisiones de duración de sprint y límite de trabajo en progreso las razoné y elegí yo mismo
con apoyo de la IA para entender las implicancias de cada opción — puedo explicarlas en la
defensa oral sin depender de ella.
