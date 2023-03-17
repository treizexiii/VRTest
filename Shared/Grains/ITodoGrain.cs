using Shared.Models;

namespace Shared.Grains;

public interface ITodoGrain : IGrainWithGuidKey
{
    Task SetAsync(TodoItem item);
    Task<TodoItem?> GetAsync(Guid key);
    Task<IEnumerable<TodoItem>?> GetAllAsync();
    Task<IEnumerable<TodoItem>> RemoveAsync(Guid key);
    Task<IEnumerable<TodoItem>> ClearAsync();
}