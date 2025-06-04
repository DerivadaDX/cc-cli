# Convenciones y buenas prácticas

## Commits

- Mensajes en español, usando estructura impersonal y pretérito perfecto simple.  
  Ejemplo: `se agregó función para validar usuarios`, `se eliminó archivo obsoleto`.

## Pull Requests

- Títulos bajo convención Angular:  
  `tipo(ámbito): descripción`
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

- Descripciones en estructura impersonal, igual que los commits.

## Nombres de ramas

- Formatos válidos:
  - `tipo/descripcion-breve`
  - `tipo/ámbito/descripcion-breve`

## Código

- Interfaces y clases con nombres en español.
- Código siempre en español.
- No lanzar excepciones desde constructores.
- Controladores: sin lógica, solo pasamanos.
- No incluir comentarios `arrange`, `act`, `assert` en tests.

## Tests

- Nombres de tests:
  - `MetodoBajoPrueba_CasoOEstadoBajoPrueba_ComportamientoEsperado`
  - `MetodoBajoPrueba_ComportamientoEsperado`
- Solo se testean métodos públicos.  
  Métodos privados se cubren indirectamente a través de los públicos.
