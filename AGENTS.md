# Repository Guidelines

## Project Structure & Module Organization
`CakeCuttingCLI.sln` reúne cuatro proyectos en `src/`. `App` maneja la CLI y los `Command`. `Common` concentra
los servicios compartidos. `Generator` arma las instancias de valoración. `Solver` empuja la búsqueda
genética. Cada módulo cuenta con su espejo de pruebas en `tests/<Proyecto>.Tests/` mediante referencias
directas al código productivo. Guardá los assets específicos cerca del módulo que los consume y evitá
versionar artefactos generados como `bin/` u `obj/`.

## Branch, Commit & PR Workflow
Usá ramas con forma `tipo/ambito/descripcion` (sin acentos) o `tipo/descripcion` cuando el ámbito sea obvio.
Mantené los commits en español, voz impersonal y pretérito perfecto simple (`se agregó`, `se corrigió`),
siguiendo el formato Angular `tipo(ámbito): descripción`. En los PR, repetí esa convención, arrancá la
descripción con `En este PR...`.

## Build, Test, and Development Commands
Restaurá dependencias con `dotnet restore`. Compilá todo con `dotnet build CakeCuttingCLI.sln`. Para probar la
CLI localmente hacé `dotnet run --project src/App/App.csproj -- generar --atomos 5 --agentes 3` o `-- resolver
instancia.dat`. Ejecutá la suite con `dotnet test`; sumá `--collect:"XPlat Code Coverage"` si necesitás
métricas de Coverlet. Cuando quieras binarios listos para release corré `dotnet publish
src/App/App.csproj -c Release`.

## Coding Style & Naming Conventions
El código, los comentarios y los identificadores van en español. Usá sangría de cuatro espacios, un tipo por
archivo y `namespace` explícito. Aplicá `PascalCase` para tipos y métodos, `camelCase` para parámetros y
variables locales, y `_camelCase` para campos privados. Priorizá cláusulas de guarda
(`ArgumentNullException.ThrowIfNull`) y reutilizá factories existentes (`ConsoleProxyFactory`,
`GeneradorNumerosRandomFactory`) antes que instanciar dependencias a mano. Corré `dotnet format` antes de
subir cambios para mantener estilo y `using` ordenados.

## Testing Guidelines
Las pruebas se escriben con xUnit y NSubstitute. Creá nuevas clases en `tests/<Proyecto>.Tests/` y poneles
nombres con el siguiente formato: `<ClaseBajoPrueba>Tests.cs`. Los métodos `[Fact]` siguen el patrón
`MetodoBajoPrueba_CasoOEstadoBajoPrueba_ComportamientoEsperado`, o `MetodoBajoPrueba_ComportamientoEsperado`. Limitá
tus pruebas a métodos públicos, cubriendo la lógica interna de forma indirecta. No incluyas comentarios indicando
"Arrenge", "Act" y "Assert". Al sumar funcionalidad agregá escenarios felices y fallidos relevantes y sostené o mejorá
la cobertura existente.

## Additional Notes
Documentá cualquier variable de entorno o configuración extra al lado del código que la usa y enlazá la
sección correspondiente en el README si hace falta. Actualizá este archivo cuando cambien las políticas del
equipo para que siga siendo la única fuente de verdad.
