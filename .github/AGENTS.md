# Convenciones y buenas prácticas

## Commits

- Mensajes en español, usando estructura impersonal y pretérito perfecto simple. No se debe usar verbos en infinitivo.
  Ejemplo: `se agregó función para validar usuarios`, `se eliminó archivo obsoleto`.

## Pull Requests

### Título

- Usar convención Angular: `tipo(ámbito): descripción`
- Tipos permitidos:
  - `feat(ámbito):` Nueva funcionalidad
  - `fix(ámbito):` Corrección de errores
  - `chore(ámbito):` Mantenimiento, dependencias, configs
  - `refactor(ámbito):` Refactor sin cambios funcionales
  - `test(ámbito):` Cambios en tests
  - `docs(ámbito):` Cambios en docs
  - `style(ámbito):` Cambios de formato/código
  - `perf(ámbito):` Mejoras de performance
  - `ci(ámbito):` Cambios en CI/CD
- La "descripción" del título debe estar escrita en español, usando estructura impersonal y pretérito perfecto
  simple, igual que los mensajes de commit. No usar infinitivo.

Ejemplo: `feat(usuario): se agregó validación de email`

### Ámbito

- El ámbito identifica el área, módulo, carpeta, funcionalidad o clase específica del sistema afectada por el cambio.
- Si es un cambio global se puede usar `proyecto` o `solución`.
- Si corresponde a una clase, se utiliza el nombre de la clase tal como está en el código, en minúsculas y sin espacios.
- Para clases con nombres compuestos, se puede separar con guiones para mayor claridad.
- Ejemplos: `usuario`, `login`, `proyecto`, `solución`, `config`, `usuario-controller`, `validador-email`.

### Descripción

- Escribir la descripción del PR en estructura impersonal, igual que los commits.
- La descripción debe iniciar con `En este PR...` seguida de los cambios realizados.
- En la descripción del PR se puede volver a mencionar el ámbito para dejar claro qué parte del sistema fue modificada.
- Ejemplo:
  ```text
  En este PR se implementó la validación de email del usuario.
  Se agregó una nueva clase `ValidadorEmail` que verifica el formato del email ingresado.
  ```

## Nombres de ramas

- Formatos válidos:
  - `tipo/nombre-de-feature`
  - `tipo/ámbito/nombre-de-feature`
- No usar tildes ni caracteres con acentos.

## Código

- Código siempre en español.
- No lanzar excepciones desde constructores.
- Controladores: sin lógica, solo pasamanos.

## Tests

- Nombres de tests:
  - `MetodoBajoPrueba_CasoOEstadoBajoPrueba_ComportamientoEsperado`
  - `MetodoBajoPrueba_ComportamientoEsperado`
- Solo se testean métodos públicos. Métodos privados se cubren indirectamente a través de los públicos.
- No incluir comentarios `arrange`, `act`, `assert`.
