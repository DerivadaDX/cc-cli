# Cake Cutting Genético

## Descripción

Este proyecto implementa un algoritmo basado en técnicas genéticas para la división justa de una torta discreta (discrete
cake-cutting) entre múltiples jugadores, con el objetivo de lograr una distribución libre de envidia (envy-free).
Se basa en la teoría presentada en el artículo _Envy-free division of discrete cakes_ de Javier Marenco y Tomás Tetzlaff.

## Contexto

El problema de división del pastel es un problema de asignación justa en el que un recurso discreto debe ser distribuido
entre varios jugadores, quienes tienen valoraciones diferentes para cada parte del recurso. El pastel está compuesto por
átomos indivisibles que son valorados de manera distinta por cada jugador.

Las valoraciones de cada jugador sobre los átomos deben sumar 1 (100%), representando el valor total que asignan al
pastel completo.

El objetivo es lograr una asignación que satisfaga la propiedad de **libre de envidia** (envy-freeness), lo que
significa que ningún jugador prefiere la porción de otro jugador por encima de la suya.

## Referencias

- Marenco, J., & Tetzlaff, T. (2013). _Envy-free division of discrete cakes_. Discrete Applied Mathematics, 163(Part 2),
  233-244. [DOI: 10.1016/j.dam.2013.06.032](https://doi.org/10.1016/j.dam.2013.06.032)
