using Shared.Models;

namespace Shared.Grains;

public interface ITodoGrain : IGrainWithGuidKey
{
    Task SetAsync(TodoItem item);
    Task<TodoItem?> GetAsync();
}