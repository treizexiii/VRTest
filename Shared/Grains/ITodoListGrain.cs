using Shared.Models;

namespace Shared.Grains;

public interface ITodoListGrain
{
    Task PushAsync(TodoItem item);
    Task<TodoList?> GetAsync();
}