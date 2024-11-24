namespace TodoApp_Backend.DTOs
{
    public class TodoDTO
    {

        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public bool? Completed { get; set; }

    }
}
