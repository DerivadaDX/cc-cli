# Changelog

All notable changes to this project will be documented in this file. See [standard-version](https://github.com/conventional-changelog/standard-version) for commit guidelines.

## [3.0.0](https://github.com/DerivadaDX/cc-cli/compare/v2.0.0...v3.0.0) (2025-10-27)


### ⚠ BREAKING CHANGES

* **resolver-command:** cambio de individuo por defecto a 'optimizacion' (#72)
* **commands:** cambio en forma de recibir rutas de archivos a generar y resolver (#68)

### Features

* **commands:** cambio en forma de recibir rutas de archivos a generar y resolver ([#68](https://github.com/DerivadaDX/cc-cli/issues/68)) ([d522bdd](https://github.com/DerivadaDX/cc-cli/commit/d522bddc06d4f25b823343277422fd311da5262f))
* **commands:** incorporación de parámetro `seed` ([#70](https://github.com/DerivadaDX/cc-cli/issues/70)) ([bba03f9](https://github.com/DerivadaDX/cc-cli/commit/bba03f945ee25f0bbec9efa12b26cc9c7084fb0d))
* **resolver-command:** cambio de individuo por defecto a 'optimizacion' ([#72](https://github.com/DerivadaDX/cc-cli/issues/72)) ([6c27705](https://github.com/DerivadaDX/cc-cli/commit/6c27705f5c1dcb77b475753751481b060d506881))
* **resolver-command:** notificación de detención por estancamiento ([#61](https://github.com/DerivadaDX/cc-cli/issues/61)) ([6e3f87d](https://github.com/DerivadaDX/cc-cli/commit/6e3f87d315fe2aa645f1132b056341bc7bbb4370))

## [2.0.0](https://github.com/DerivadaDX/cc-cli/compare/v1.0.0...v2.0.0) (2025-06-19)


### ⚠ BREAKING CHANGES

* **resolver-command:** cambio de nombre de parámetros `--tamaño-poblacion` y `--max-generaciones` (#48)

### Features

* **app:** agregado de descripción en opciones ([#51](https://github.com/DerivadaDX/cc-cli/issues/51)) ([5add16b](https://github.com/DerivadaDX/cc-cli/commit/5add16b9d61a8cd590b11e3f142bbc8ce74df310))
* **app:** extracción de lógica para presentación de información al usuario a una clase propia ([#54](https://github.com/DerivadaDX/cc-cli/issues/54)) ([89e9129](https://github.com/DerivadaDX/cc-cli/commit/89e9129142bea6958d9859377897a78ab6c00be3))
* **app:** mejoras en la representación del resultado al resolver instancias ([#47](https://github.com/DerivadaDX/cc-cli/issues/47)) ([492e7ac](https://github.com/DerivadaDX/cc-cli/commit/492e7ac24f2ebe5385430646bcd94ec04af70007))
* **individuos:** implementación de nuevo individuo `IndividuoOptimizacionAsignaciones` ([#58](https://github.com/DerivadaDX/cc-cli/issues/58)) ([74cf494](https://github.com/DerivadaDX/cc-cli/commit/74cf49458c8d9a7ed42fd464a8c3a52a0064578a))
* **individuos:** se muestran ordenadas las posiciones de los cortes en `ToString` del `Individuo` ([#46](https://github.com/DerivadaDX/cc-cli/issues/46)) ([fbb8606](https://github.com/DerivadaDX/cc-cli/commit/fbb8606dd7e17e56fdeab709edd2fbead10e352a))
* **resolver-command:** cambio de nombre de parámetros `--tamaño-poblacion` y `--max-generaciones` ([#48](https://github.com/DerivadaDX/cc-cli/issues/48)) ([52155c5](https://github.com/DerivadaDX/cc-cli/commit/52155c5b65090ac3e8bedc4d23e65a483e9e9b1a))
* **resolver-command:** implementación de barra de progreso y colores ([#53](https://github.com/DerivadaDX/cc-cli/issues/53)) ([31672db](https://github.com/DerivadaDX/cc-cli/commit/31672dbbed12b24560cd31d57d5212d103f21003))
* **resolver-command:** implementación de límite de estancamiento ([#57](https://github.com/DerivadaDX/cc-cli/issues/57)) ([269a1f0](https://github.com/DerivadaDX/cc-cli/commit/269a1f054240a54bb1ebd092b386ad49be6c7ba6))
* **resolver-command:** incorporación de parámetro para indicar tipo de individuo a utilizar ([#59](https://github.com/DerivadaDX/cc-cli/issues/59)) ([03a8533](https://github.com/DerivadaDX/cc-cli/commit/03a8533ec05958b9aae19a1d954cdd6511188679))
* **resolver-command:** incorporación de soporte de cancelación para obtención de resultado parcial ([#49](https://github.com/DerivadaDX/cc-cli/issues/49)) ([75ee94b](https://github.com/DerivadaDX/cc-cli/commit/75ee94b37b1f6e45afbb0bd2c8f0203f699ba273))
* **resolver-command:** notificación de procesamiento de generaciones en tiempo real ([#50](https://github.com/DerivadaDX/cc-cli/issues/50)) ([e22f7c4](https://github.com/DerivadaDX/cc-cli/commit/e22f7c4cdce87481325cf950bea326bd1170f302))

### Bug Fixes

* **app:** despliegue de ayuda cuando se invoca la cli sin argumentos ([#52](https://github.com/DerivadaDX/cc-cli/issues/52)) ([8b36ee7](https://github.com/DerivadaDX/cc-cli/commit/8b36ee70b6c95f6eb83aded184ac9c64fa12bc23))
* **resolver-command:** se evita línea en blanco innecesaria tras cancelación ([#56](https://github.com/DerivadaDX/cc-cli/issues/56)) ([32a3c83](https://github.com/DerivadaDX/cc-cli/commit/32a3c83f8b2b64ddfad4820787159fc6690cb583))

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
