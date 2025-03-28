## [v1.0.0-beta.1](https://github.com/DerivadaDX/CakeCuttingGenetico/compare/v0.0.0...v1.0.0-beta.1) (2025-03-28)

### Breaking Changes

* **instancia-builder:** se modificó para usar matrices rectangulares en vez de escalonadas
* **generador-instancia:** se renombró el proyecto y cambió de consola a librería
* **instancia-problema:** se invirtió el orden esperado en la matriz de valoraciones

### Features

* **app:** implementación de CLI para generar y exportar instancias del problema
* **instancia-builder:** implementación inicial del builder de instancias
* **individuo:** clase base para individuos y definición de cromosomas
* **random:** se removió `IGeneradorNumerosRandom` y se implementó factory
* **solver:** clases básicas: `Átomo`, `Jugador`, `InstanciaProblema`
* **solver:** generador de números aleatorios con tests
* **common:** `FileSystemHelper` para interacción con el sistema de archivos

### Refactors

* **instancia-problema:** se eliminó la condición de valoraciones disjuntas
* **atomo:** se eliminó la restricción de valoración dentro del rango [0,1]

### Chores

* **dotnet:** configuración inicial de la solución y CI/CD
