# Cake Cutting CLI

## Descripción

Este proyecto implementa una interfaz de línea de comandos (CLI) para generar y resolver instancias del problema de
división justa de una torta discreta (_discrete cake-cutting_) entre múltiples jugadores. Utiliza un algoritmo basado en
técnicas genéticas para encontrar asignaciones **libres de envidia** (_envy-free_).

La herramienta permite:

- Generar y exportar instancias del problema con diferentes configuraciones.
- Resolver instancias utilizando un algoritmo genético (_todavía en desarrollo_).

## Contexto

El problema de **cake-cutting** es un problema de asignación justa, donde un recurso discreto debe ser distribuido entre
varios jugadores con valoraciones diferentes sobre cada parte.

El pastel está compuesto por **átomos indivisibles**, y el objetivo es lograr una asignación **libre de envidia**, es
decir, que ningún jugador prefiera la porción de otro en vez de la suya.

## Manual de la CLI

### Uso básico

```bash
cc-cli.exe [comando] [opciones]
```

### Comandos disponibles

#### 1. Generar instancias

Genera matrices de valoración para el problema. Las instancias generadas usan el formato:

```
[numero_de_atomos] [numero_de_agentes]
[valor_11] [valor_12] ... [valor_1n]
[valor_21] [valor_22] ... [valor_2n]
...
```

El archivo de salida se sobrescribe sin confirmación.

**Sintaxis**:

```bash
cc-cli.exe generar [opciones]
```

**Opciones principales**:

| Opción      | Descripción         | Valores aceptados           |
| ----------- | ------------------- | --------------------------- |
| `--atomos`  | Cantidad de átomos  | Entero positivo (requerido) |
| `--agentes` | Cantidad de agentes | Entero positivo (requerido) |

**Opciones secundarias**:

| Opción           | Descripción                              | Valores aceptados                  |
| ---------------- | ---------------------------------------- | ---------------------------------- |
| `--valor-maximo` | Valor máximo para cada valoración        | Entero positivo (default: 1000)    |
| `--output`       | Ruta donde guardar la instancia generada | Un path (default: `instancia.dat`) |
| `--disjuntas`    | Indica si las valoraciones son disjuntas |                                    |

**Ejemplos**:

```bash
# Instancia básica
cc-cli.exe generar --atomos 10 --agentes 3

# Con valoraciones disjuntas
cc-cli.exe generar --atomos 15 --agentes 4 --disjuntas

# Especificando valor máximo
cc-cli.exe generar --atomos 8 --agentes 2 --valor-maximo 100

# Especificando archivo de salida
cc-cli.exe generar --atomos 8 --agentes 2 --output datos/instancia1.txt

# Ejemplo completo
cc-cli.exe generar --atomos 5 --agentes 3 --disjuntas --valor-maximo 500 --output instancia.txt
```

#### 2. Resolver instancias

Resuelve una instancia del problema de cake-cutting utilizando un algoritmo genético para encontrar una asignación
libre de envidia.
Durante la ejecución se puede presionar `Ctrl+C` para finalizar el algoritmo.
En ese caso se muestra el mejor individuo encontrado hasta el momento.

**Sintaxis**:

```bash
cc-cli.exe resolver [opciones]
```

**Opciones principales**:

| Opción        | Descripción                     | Valores aceptados   |
| ------------- | ------------------------------- | ------------------- |
| `--instancia` | Ruta de la instancia a resolver | Un path (requerido) |

**Opciones secundarias**:

| Opción                   | Descripón                                        | Valores aceptados                                       |
| ------------------------ | ------------------------------------------------ | ------------------------------------------------------- |
| `--limite-generaciones`  | Límite de generaciones a computar (0 = infinito) | Entero positivo (default: 0)                            |
| `--cantidad-individuos`  | Cantidad de individuos por generación            | Entero positivo (default: 100)                          |
| `--limite-estancamiento` | Límite de generaciones sin mejora (0 = infinito) | Entero positivo (default: 1000)                         |
| `--tipo-individuo`       | Tipo de individuo a utilizar                     | "intercambio" u "optimizacion" (default: "intercambio") |

**Ejemplos**:

```bash
# Resolver una instancia (corre indefinidamente con tamaño de poblacion = 100)
cc-cli.exe resolver --instancia instancia.dat

# Especificando límite de generaciones
cc-cli.exe resolver --instancia instancia.dat --limite-generaciones 1000

# Especificando cantidad de individuos
cc-cli.exe resolver --instancia instancia.dat --cantidad-individuos 500

# Especificando límite de estancamiento
cc-cli.exe resolver --instancia instancia.dat --limite-estancamiento 500

# Especificando tipo de individuo
cc-cli.exe resolver --instancia instancia.dat --tipo-individuo optimizacion

# Ejemplo completo
cc-cli.exe resolver --instancia instancia.dat --cantidad-individuos 500 --limite-generaciones 1000 --limite-estancamiento 500 --tipo-individuo intercambio
```

#### 3. Otros comandos

**Mostrar versión:**

```bash
cc-cli.exe --version
```

**Ayuda:**

```bash
cc-cli.exe --help
cc-cli.exe generar --help
cc-cli.exe resolver --help
```

## Referencias

- Marenco, J., & Tetzlaff, T. (2013). _Envy-free division of discrete cakes_. Discrete Applied Mathematics, 163(Part 2),
  233-244. [DOI: 10.1016/j.dam.2013.06.032](https://doi.org/10.1016/j.dam.2013.06.032)
