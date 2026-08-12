# Decisiones — TP1

## 1. Por qué Git no pudo resolver el conflicto solo

Las ramas A y B partieron del mismo commit en `main` y ambas modificaron la primera línea del
README con contenido distinto. Git puede fusionar automáticamente cuando los cambios tocan partes
distintas del archivo, pero cuando dos ramas tocan exactamente la misma línea no tiene forma de
saber cuál versión es "la correcta" — es una decisión de contenido, no algo que se pueda resolver
con un algoritmo. Para que el conflicto nunca hubiera aparecido, alguna de las dos ramas tendría
que haberse integrado antes de que la otra empezara a trabajar sobre esa misma línea, o directamente
no tocar la misma línea.

## 2. Qué problemas encontré y cómo los solucioné

[Completá acá con tus propios tropiezos reales, por ejemplo:]
- Al clonar el repo por primera vez dejé los signos `<` y `>` en la URL (copiados literal de la
  guía) y tiré error 400. Lo solucioné reemplazando por mi usuario real.
- En PowerShell `git switch main && git pull` no funcionó porque `&&` no es válido como separador
  en esa versión de PowerShell. Lo solucioné poniendo cada comando en su propia línea.
- La rama que creó GitHub al editar el README desde la web se llamó `MateoAlfei-patch-1` en vez de
  seguir la convención `feature/<descripcion>` que pide la guía. [Contá si la renombraste o la
  dejaste así.]

## 3. Declaración de uso de IA

Usé Claude (Anthropic) como guía paso a paso durante todo el TP: para entender la teoría de Git
(ramas, merges, conflictos), para los comandos exactos en cada paso, y para resolver errores
puntuales (URL mal copiada, sintaxis de PowerShell). Verifiqué cada paso ejecutando los comandos
yo mismo y confirmando el resultado en la terminal y en GitHub antes de continuar — no copié nada
a ciegas sin ver que funcionara. Entiendo el porqué de cada configuración (protección de rama,
squash and merge, resolución de conflictos) y puedo explicarlo en la defensa oral.