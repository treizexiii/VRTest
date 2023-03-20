using Shared.Models;

namespace Shared.Grains;

public interface IOwnerGrain : IGrainWithGuidKey
{
    Task AddTodoAsync(TodoItem todo);
    Task<IEnumerable<TodoItem>> GetTodosAsync();
    Task RemoveTodoAsync(Guid todoKey);
    Task ClearAsync();
}