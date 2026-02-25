namespace Json_Demo.Models
{
    // Clase para respuestas personalizadas de nuestra API
    public class TareaResponse
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public int UsuarioId { get; set; }
        public bool Completada { get; set; }
        public string Estado { get; set; } = string.Empty;
    }
}
