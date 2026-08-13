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