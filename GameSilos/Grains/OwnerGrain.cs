using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using Shared.Grains;
using Shared.Models;

namespace GameSilos.Grains;

public class OwnerGrain : Grain, IOwnerGrain
{
    private readonly ILogger<OwnerGrain> _logger;
    private readonly IPersistentState<State> _state;

    public OwnerGrain(ILogger<OwnerGrain> logger, [PersistentState("State")] IPersistentState<State> state)
    {
        _logger = logger;
        _state = state;
    }

    private static string GrainType => nameof(OwnerGrain);
    private Guid GrainKey => this.GetPrimaryKey();

    public async Task AddTodoAsync(TodoItem todo)
    {
        if (!_state.RecordExists)
        {
            _state.State.Item = new Owner()
            {
                Guid = todo.OwnerKey,
            };
        }

        if (!_state.State.Item.TodoKeys.Contains(todo.Key))
        {
            _state.State.Item?.TodoKeys.Add(todo.Key);
        }

        await _state.WriteStateAsync();
        
        _logger.LogInformation(
            "{@GrainType} {@GrainKey} now contains {@Todo}",
            GrainType, GrainKey, todo);
    }

    public async Task<IEnumerable<TodoItem>> GetTodosAsync()
    {
        var todoList = new List<TodoItem>();
        if (_state.RecordExists && _state.State.Item?.TodoKeys.Count > 0)
        {
            foreach (var todo in _state.State.Item.TodoKeys)
            {
                var t = await GrainFactory.GetGrain<ITodoGrain>(todo).GetAsync();
                if (t != null)
                {
                    todoList.Add(t);
                }
            }
        }
        
        _logger.LogInformation("GetTodosAsync called on {@GrainType} {@GrainKey} and returned {@TodoList}",
            GrainType, GrainKey, todoList);
        return todoList;
    }

    public async Task RemoveTodoAsync(Guid todoKey)
    {
        await GrainFactory.GetGrain<ITodoGrain>(todoKey).ClearAsync();
        _state.State.Item?.TodoKeys.Remove(todoKey);
        await _state.WriteStateAsync();
        
        _logger.LogInformation(
            "{@GrainType} {@GrainKey} no longer contains {@TodoKey}",
            GrainType, GrainKey, todoKey);
    }

    public async Task ClearAsync()
    {
        foreach(var todo in _state.State.Item?.TodoKeys)
        {
            await GrainFactory.GetGrain<ITodoGrain>(todo).ClearAsync();
        }
        _state.State.Item?.TodoKeys.Clear();
        await _state.WriteStateAsync();
        
        _logger.LogInformation("Cleared {@GrainType} {@GrainKey}", 
            GrainType, GrainKey);
    }

    [GenerateSerializer]
    public class State
    {
        [Id(0)] public Owner? Item { get; set; }
    }
}