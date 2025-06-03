using Solver.Fitness;
using Solver.Individuos;

namespace Solver.Tests.Individuos
{
    public class IndividuoTests
    {
        [Fact]
        public void Constructor_CromosomaNull_LanzaArgumentNullException()
        {
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,] { { 1 } });

            var ex = Assert.Throws<ArgumentNullException>(() => new IndividuoStub(null, instanciaProblema));
            Assert.Equal("cromosoma", ex.ParamName);
        }

        [Fact]
        public void Constructor_InstanciaProblemaNull_LanzaArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new IndividuoStub([], null));
            Assert.Equal("problema", ex.ParamName);
        }

        [Fact]
        public void Constructor_CromosomaVacio_LanzaArgumentException()
        {
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,] { { 1 } });

            var ex = Assert.Throws<ArgumentException>(() => new IndividuoStub([], instanciaProblema));
            Assert.Contains("no puede estar vacío", ex.Message);
            Assert.Equal("cromosoma", ex.ParamName);
        }

        [Fact]
        public void Constructor_CantidadGenesInvalidaParaInstanciaDelProblema_LanzaArgumentException()
        {
            // Para k agentes se esperan k-1 cortes y k asignaciones
            decimal[,] matriz = { { 1m, 0m }, { 0m, 1m } };
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(matriz);

            var ex = Assert.Throws<ArgumentException>(() => new IndividuoStub([1, 2], instanciaProblema));
            Assert.Contains("Cantidad de genes inválida", ex.Message);
            Assert.Equal("cromosoma", ex.ParamName);
        }

        [Fact]
        public void Constructor_PosicionPrimerCorteEnCromosomaEsNegativa_LanzaArgumentException()
        {
            decimal[,] matriz = { { 1m, 0m }, { 0m, 1m } };
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(matriz);

            var ex = Assert.Throws<ArgumentException>(() => new IndividuoStub([-1, 2, 1], instanciaProblema));
            Assert.Contains("primer corte no puede ser negativo", ex.Message);
            Assert.Equal("cromosoma", ex.ParamName);
        }

        [Fact]
        public void Constructor_PosicionUltimoCorteEnCromosomaEsMayorQueCantidadAtomos_LanzaArgumentException()
        {
            decimal[,] matriz = { { 1m, 0m }, { 0m, 1m } };
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(matriz);

            var ex = Assert.Throws<ArgumentException>(() => new IndividuoStub([3, 2, 1], instanciaProblema));
            Assert.Contains("no puede superar la cantidad de átomos", ex.Message);
            Assert.Equal("cromosoma", ex.ParamName);
        }

        [Fact]
        public void Constructor_HayUnaAsignacionAAgentesInvalidosEnCromosoma_LanzaArgumentException()
        {
            // El rango permitido para las asignaciones de k agentes es [1, k]
            decimal[,] matriz = { { 1m, 0m }, { 0m, 1m } };
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(matriz);

            var ex = Assert.Throws<ArgumentException>(() => new IndividuoStub([0, 1, 0], instanciaProblema));
            Assert.Contains("asignaciones fuera de rango", ex.Message);
            Assert.Equal("cromosoma", ex.ParamName);
        }

        [Fact]
        public void Constructor_HayMasDeUnaAsignacionAAgentesInvalidosEnCromosoma_LanzaArgumentException()
        {
            decimal[,] matriz = { { 1m, 0m }, { 0m, 1m } };
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(matriz);

            var ex = Assert.Throws<ArgumentException>(() => new IndividuoStub([0, -1, 5], instanciaProblema));
            Assert.Contains("asignaciones fuera de rango", ex.Message);
            Assert.Equal("cromosoma", ex.ParamName);
        }

        [Fact]
        public void Constructor_ListadoDeAsignacionesRepetidasSeMuestraOrdenadoAscendente_LanzaArgumentException()
        {
            decimal[,] matriz =
            {
                { 1m, 0m, 0m, 0m },
                { 0m, 1m, 0m, 0m },
                { 0m, 0m, 1m, 0m },
                { 0m, 0m, 0m, 1m }
            };
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(matriz);

            var ex = Assert.Throws<ArgumentException>(() => new IndividuoStub([1, 2, 3, 5, 2, 0, -1], instanciaProblema));
            Assert.Contains("asignaciones fuera de rango", ex.Message);
            Assert.Equal("cromosoma", ex.ParamName);
        }

        [Fact]
        public void Constructor_HayUnaPorcionAsignadaAMasDeUnAgenteEnCromosoma_LanzaArgumentException()
        {
            decimal[,] matriz = { { 1m, 0m }, { 0m, 1m } };
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(matriz);

            var ex = Assert.Throws<ArgumentException>(() => new IndividuoStub([0, 1, 1], instanciaProblema));
            Assert.Contains("asignaciones repetidas", ex.Message);
            Assert.Equal("cromosoma", ex.ParamName);
        }

        [Fact]
        public void Constructor_HayMasDeUnaPorcionAsignadaAMasDeUnAgenteEnCromosoma_LanzaArgumentException()
        {
            decimal[,] matriz =
            {
                { 1m, 0m, 0m, 0m },
                { 0m, 1m, 0m, 0m },
                { 0m, 0m, 1m, 0m },
                { 0m, 0m, 0m, 1m }
            };
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(matriz);

            var ex = Assert.Throws<ArgumentException>(() => new IndividuoStub([0, 0, 4, 2, 2, 3, 3], instanciaProblema));
            Assert.Contains("asignaciones repetidas", ex.Message);
            Assert.Equal("cromosoma", ex.ParamName);
        }

        [Fact]
        public void Constructor_CromosomaValido_NoLanzaExcepciones()
        {
            // Cortes: 1, Asignaciones: 2, 1
            // - 1ra porción se asigna al agente 2
            // - 2da Porción se asigna al agente 1
            var cromosomaValido = new List<int> { 1, 2, 1 };

            // 3 átomos, 2 agentes
            var instanciaProblema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { 1m, 0m },
                { 0m, 1m },
                { 0m, 1m },
            });

            var ex = Record.Exception(() => new IndividuoStub(cromosomaValido, instanciaProblema));
            Assert.Null(ex);
        }

        [Fact]
        public void ExtraerAsignacion_AsignacionEstandar_DevuelveAtomosCorrectos()
        {
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { 1m, 0m },
                { 0m, 1m },
                { 0m, 1m },
            });
            var cromosoma = new List<int> { 2, 1, 2 };
            var individuo = new IndividuoStub(cromosoma, problema);

            List<Agente> agentes = individuo.ExtraerAsignacion();

            Agente agente1 = agentes.First(a => a.Id == 1);
            Assert.Equal([1, 2], agente1.Valoraciones.Select(v => v.Posicion));

            Agente agente2 = agentes.First(a => a.Id == 2);
            Assert.Equal([3], agente2.Valoraciones.Select(v => v.Posicion));
        }

        [Fact]
        public void ExtraerAsignacion_AtomosNoValorados_IncluyeConValorCero()
        {
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { 1m, 0m },
                { 0m, 1m },
            });
            var cromosoma = new List<int> { 1, 2, 1 };
            var individuo = new IndividuoStub(cromosoma, problema);

            List<Agente> agentes = individuo.ExtraerAsignacion();

            Agente agente1 = agentes.First(a => a.Id == 1);
            Assert.Single(agente1.Valoraciones);
            Assert.Equal(2, agente1.Valoraciones[0].Posicion);
            Assert.Equal(0m, agente1.Valoraciones[0].Valoracion);

            Agente agente2 = agentes.First(a => a.Id == 2);
            Assert.Single(agente2.Valoraciones);
            Assert.Equal(1, agente2.Valoraciones[0].Posicion);
            Assert.Equal(0m, agente2.Valoraciones[0].Valoracion);
        }


        [Fact]
        public void ExtraerAsignacion_TodosValorados_AsignacionCompleta()
        {
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { 1m, 1m },
                { 1m, 1m },
                { 1m, 1m },
                { 1m, 1m },
            });
            var cromosoma = new List<int> { 2, 2, 1 };
            var individuo = new IndividuoStub(cromosoma, problema);

            List<Agente> agentes = individuo.ExtraerAsignacion();

            Agente agente1 = agentes.First(a => a.Id == 1);
            Assert.Equal([3, 4], agente1.Valoraciones.Select(v => v.Posicion));

            Agente agente2 = agentes.First(a => a.Id == 2);
            Assert.Equal([1, 2], agente2.Valoraciones.Select(v => v.Posicion));
        }

        [Fact]
        public void ExtraerAsignacion_AgenteSinAtomos_ListaValoracionesVacia()
        {
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { 1m, 0m, 0m },
                { 0m, 1m, 0m },
                { 0m, 0m, 1m },
            });
            var cromosoma = new List<int> { 3, 3, 1, 2, 3 };
            var individuo = new IndividuoStub(cromosoma, problema);

            List<Agente> agentes = individuo.ExtraerAsignacion();

            Agente agente1 = agentes.First(a => a.Id == 1);
            Assert.Equal([1, 2, 3], agente1.Valoraciones.Select(v => v.Posicion));

            Agente agente2 = agentes.First(a => a.Id == 2);
            Assert.Empty(agente2.Valoraciones);

            Agente agente3 = agentes.First(a => a.Id == 3);
            Assert.Empty(agente3.Valoraciones);
        }

        [Fact]
        public void ExtraerAsignacion_CortesConsecutivos_PorcionesUnitarias()
        {
            var problema = InstanciaProblema.CrearDesdeMatrizDeValoraciones(new decimal[,]
            {
                { 1m, 1m, 1m },
                { 1m, 1m, 1m },
                { 1m, 1m, 1m },
            });
            var cromosoma = new List<int> { 1, 2, 1, 2, 3 };
            var individuo = new IndividuoStub(cromosoma, problema);

            List<Agente> agentes = individuo.ExtraerAsignacion();

            Agente agente1 = agentes.First(a => a.Id == 1);
            Assert.Single(agente1.Valoraciones);
            Assert.Equal(1, agente1.Valoraciones[0].Posicion);

            Agente agente2 = agentes.First(a => a.Id == 2);
            Assert.Single(agente2.Valoraciones);
            Assert.Equal(2, agente2.Valoraciones[0].Posicion);

            Agente agente3 = agentes.First(a => a.Id == 3);
            Assert.Single(agente3.Valoraciones);
            Assert.Equal(3, agente3.Valoraciones[0].Posicion);
        }

        private class IndividuoStub : Individuo
        {
            internal IndividuoStub(List<int> cromosoma, InstanciaProblema problema) : base(cromosoma, problema)
            {
            }

            internal override void Mutar()
            {
                throw new NotImplementedException();
            }

            internal override Individuo Cruzar(Individuo otro)
            {
                throw new NotImplementedException();
            }

            internal override void CalcularFitness(ICalculadoraFitness calculadoraFitness)
            {
                throw new NotImplementedException();
            }
        }
    }
}
