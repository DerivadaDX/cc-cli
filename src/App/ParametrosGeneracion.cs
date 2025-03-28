namespace App
{
    internal class ParametrosGeneracion
    {
        internal ParametrosGeneracion(int atomos, int agentes, int valorMaximo, string rutaSalida, bool valoracionesDisjuntas)
        {
            ValidarAtomos(atomos);
            ValidarAgentes(agentes);
            ValidarValorMaximo(valorMaximo);
            ValidarRuta(rutaSalida);

            Atomos = atomos;
            Agentes = agentes;
            ValorMaximo = valorMaximo;
            RutaSalida = rutaSalida;
            ValoracionesDisjuntas = valoracionesDisjuntas;
        }

        internal int Atomos { get; }
        internal int Agentes { get; }
        internal int ValorMaximo { get; }
        internal string RutaSalida { get; }
        internal bool ValoracionesDisjuntas { get; }

        private void ValidarAtomos(int atomos)
        {
            if (atomos <= 0)
                throw new ArgumentException("Se requieren al menos 1 átomo", nameof(atomos));
        }

        private void ValidarAgentes(int agentes)
        {
            if (agentes <= 0)
                throw new ArgumentException("Se requieren al menos 1 agente", nameof(agentes));
        }

        private void ValidarValorMaximo(int valorMaximo)
        {
            if (valorMaximo < 0)
                throw new ArgumentException("El valor máximo no puede ser negativoo", nameof(valorMaximo));
        }

        private void ValidarRuta(string ruta)
        {
            if (string.IsNullOrWhiteSpace(ruta))
                throw new ArgumentException("La ruta no puede estar vacía", nameof(ruta));
        }
    }
}
