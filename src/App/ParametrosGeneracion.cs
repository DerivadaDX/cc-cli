namespace App
{
    internal class ParametrosGeneracion
    {
        internal ParametrosGeneracion(int atomos, int agentes, int valorMaximo, string rutaSalida, bool valoracionesDisjuntas)
        {
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
    }
}
