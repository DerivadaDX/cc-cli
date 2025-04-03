# Cake Cutting Genético

## Descripción

Este proyecto implementa un algoritmo basado en técnicas genéticas para la división justa de una torta discreta
(_discrete cake-cutting_) entre múltiples jugadores, con el objetivo de lograr una distribución **libre de envidia**
(_envy-free_).

Se basa en la teoría presentada en el artículo _Envy-free division of discrete cakes_ de Javier Marenco y Tomás Tetzlaff.

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

**Sintaxis**:

```bash
cc-cli.exe generar [opciones]
```

**Opciones principales**:

| Opción      | Descripción       | Valores aceptados           |
| ----------- | ----------------- | --------------------------- |
| `--atomos`  | Número de átomos  | Entero positivo (requerido) |
| `--agentes` | Número de agentes | Entero positivo (requerido) |

**Opciones secundarias**:

| Opción           | Descripción                              | Valores aceptados                  |
| ---------------- | ---------------------------------------- | ---------------------------------- |
| `--valor-maximo` | Valor máximo para las valoraciones       | Entero positivo (default: 1000)    |
| `--disjuntas`    | Flag para generar valoraciones disjuntas |                                    |
| `--output`       | Ruta y nombre del archivo de salida      | Un path (default: `instancia.dat`) |

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
cc-cli.exe generar --atomos 5 --agentes 3 --disjuntas --valor-maximo 500 -output instancia.txt
```

**Notas:**

- El archivo de salida se sobrescribe sin confirmación.

#### 2. Otros comandos

**Mostrar versión:**

```bash
cc-cli.exe --version
```

**Ayuda:**

```bash
cc-cli.exe --help
cc-cli.exe generar --help
```

## Referencias

- Marenco, J., & Tetzlaff, T. (2013). _Envy-free division of discrete cakes_. Discrete Applied Mathematics, 163(Part 2),
  233-244. [DOI: 10.1016/j.dam.2013.06.032](https://doi.org/10.1016/j.dam.2013.06.032)
