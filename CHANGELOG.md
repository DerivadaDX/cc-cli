# Changelog

All notable changes to this project will be documented in this file. See [standard-version](https://github.com/conventional-changelog/standard-version) for commit guidelines.

## [1.0.0](https://github.com/DerivadaDX/cc-cli/compare/v1.0.0-beta.1...v1.0.0) (2025-06-11)

### ⚠ BREAKING CHANGES

* **generador-instancia:** Se volvieron obligatorios los llamados a `ConValorMaximo(int)` y `ConValoracionesDisjuntas(bool)` de `InstanciaBuilder` antes de hacer `Build`. Arroja `InvalidOperationException` si no se invocaron.
* **dominio:** Se renombró la entidad `Jugador` a `Agente`, lo cual rompe compatibilidad con código que la referencie por su nombre anterior.

### Features

* **algoritmo-genetico:** remoción de parámetro `esSolucionOptima` ([#40](https://github.com/DerivadaDX/cc-cli/issues/40)) ([bc0ccd8](https://github.com/DerivadaDX/cc-cli/commit/bc0ccd8dd10618efc27abd12a98edf8f2e3cf692))
* **algoritmo-genetico:** se permite ejecución indefinida cuando maxGeneraciones es cero ([#35](https://github.com/DerivadaDX/cc-cli/issues/35)) ([a44ca54](https://github.com/DerivadaDX/cc-cli/commit/a44ca546f47fbc8e58b42296ee064d7c7dae52b9))
* **app:** implementación de comando "resolver" ([#26](https://github.com/DerivadaDX/cc-cli/issues/26)) ([498406d](https://github.com/DerivadaDX/cc-cli/commit/498406d0842fcf94d62a8ad2e082b8b1e8ee8193))
* **generador-instancia:** remoción de valores por defecto del builder ([#18](https://github.com/DerivadaDX/cc-cli/issues/18)) ([ad8583a](https://github.com/DerivadaDX/cc-cli/commit/ad8583a0ba0ccc1f4877ce1598a49579cc04f4ad))
* **generador-numeros-random:** se agregó método Siguiente solo con límite máximo ([#34](https://github.com/DerivadaDX/cc-cli/issues/34)) ([4079585](https://github.com/DerivadaDX/cc-cli/commit/4079585cca38d4b8a6063aa4f7c4d8033451ad64))
* **individuos:** implementación de `IndividuoIntercambioAsignaciones` ([#38](https://github.com/DerivadaDX/cc-cli/issues/38)) ([4678d32](https://github.com/DerivadaDX/cc-cli/commit/4678d3221131ddeede24be07f253cefd61f9d9cb))
* **individuos:** implementación de clases para creación de `Individuo` aleatorio ([#43](https://github.com/DerivadaDX/cc-cli/issues/43)) ([c773c56](https://github.com/DerivadaDX/cc-cli/commit/c773c56933652484819e52fa91180fc27948c88f))
* **instancia-problema:** se modificó InstanciaProblema para que utilice matrices rectangulares en lugar de escalonadas ([#20](https://github.com/DerivadaDX/cc-cli/issues/20)) ([c78df49](https://github.com/DerivadaDX/cc-cli/commit/c78df4960ad18ac073a7f44797a498bf4ed39a97))
* **lector-archivo-matriz-valoraciones:** agregado de control para que filas y columnas sean positivas en archivo de entrada ([#32](https://github.com/DerivadaDX/cc-cli/issues/32)) ([9cbb6fa](https://github.com/DerivadaDX/cc-cli/commit/9cbb6faeed0ecdf0ae1b20c955547fd9f854eb88))
* **poblacion:** inicialización de poblacion con individuos aleatorios ([#44](https://github.com/DerivadaDX/cc-cli/issues/44)) ([3cf673c](https://github.com/DerivadaDX/cc-cli/commit/3cf673c728527b2cd11eaa0fa8119452898027d6))
* **solver:** implementación de algoritmo genético ([#22](https://github.com/DerivadaDX/cc-cli/issues/22)) ([74a9b15](https://github.com/DerivadaDX/cc-cli/commit/74a9b15ea7d4b4cca026e8ec9f5dafb5b56a4d1e))
* **solver:** implementación de CalculadorEnvyFreeness ([#28](https://github.com/DerivadaDX/cc-cli/issues/28)) ([66a4733](https://github.com/DerivadaDX/cc-cli/commit/66a4733ad48782f12fde479437e9111715687800))
* **solver:** implementación de clase Poblacion ([#27](https://github.com/DerivadaDX/cc-cli/issues/27)) ([ad4179b](https://github.com/DerivadaDX/cc-cli/commit/ad4179bacc13de42e40f6c5fc12e3b6536e73af7))
* **solver:** implementación de lector de archivo con matriz de valoraciones ([#21](https://github.com/DerivadaDX/cc-cli/issues/21)) ([85f5567](https://github.com/DerivadaDX/cc-cli/commit/85f5567d390d75dbd747ab031ecb4059cd434921))
* **solver:** remoción de interface `ICalculadoraFitness` ([#37](https://github.com/DerivadaDX/cc-cli/issues/37)) ([8729945](https://github.com/DerivadaDX/cc-cli/commit/87299453a8d527f80dfab8b405e6889b45a95c75))

### Bug Fixes

* **individuos:** validar cortes sin duplicar condiciones ([#33](https://github.com/DerivadaDX/cc-cli/issues/33)) ([399ec12](https://github.com/DerivadaDX/cc-cli/commit/399ec126f7f50d3e611d96ba627c3c58791695e9))
* **proyecto:** uso de ArgumentOutOfRangeException en lugares donde corresponde ([#24](https://github.com/DerivadaDX/cc-cli/issues/24)) ([ae7b2cc](https://github.com/DerivadaDX/cc-cli/commit/ae7b2cc9f93888f124eaa039faca863330413107))
* **readme:** corrección en ejemplo de salida ([#31](https://github.com/DerivadaDX/cc-cli/issues/31)) ([992862a](https://github.com/DerivadaDX/cc-cli/commit/992862a50f6c89dace1a31fd62599f6e3c19dea0))

## Refactors

* **dominio:** se redefine Jugador como Agente ([#14](https://github.com/DerivadaDX/cc-cli/issues/14)) ([f669320](https://github.com/DerivadaDX/cc-cli/commit/f6693208529f3970ec3ad8294d1b91dbcd7b54af))

## [v1.0.0-beta.1](https://github.com/DerivadaDX/cc-cli/compare/v0.0.0...v1.0.0-beta.1) (2025-03-28)

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
