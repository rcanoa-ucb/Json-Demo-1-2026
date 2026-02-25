namespace Json_Demo.Models
{
    public class ResumenResponse
    {
        public int UsuarioId { get; set; }

        private string? _nombreUsuario;
        public string NombreUsuario
        {
            get => _nombreUsuario ?? $"Usuario {UsuarioId}";
            set => _nombreUsuario = value;
        }

        public int TotalTareas { get; set; }
        public int TareasCompletadas { get; set; }
        public int TareasPendientes { get; set; }
        public double PorcentajeProgreso { get; set; }
        public List<TareaResponse> Tareas { get; set; } = new();
    }
}
