namespace Shared.Models;

public class TodoList
{
    public Guid Owner { get; set; }
    public List<TodoItem> Items { get; set; } = new();
}