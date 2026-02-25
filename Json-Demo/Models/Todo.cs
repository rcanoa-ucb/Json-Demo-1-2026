namespace Json_Demo.Models
{
    // Esta clase representa la tarea tal como viene de JSONPlaceholder
    public class Todo
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool Completed { get; set; }
    }
}
