using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using Shared.Grains;
using Shared.Models;

namespace GameSilos.Grains;

public class TodoListGrain : Grain, ITodoListGrain
{
    private readonly ILogger<TodoListGrain> _logger;
    private readonly IPersistentState<State> _state;

    public Guid GrainKey => this.GetPrimaryKey();
    private static string GrainType => nameof(TodoListGrain);

    public TodoListGrain(ILogger<TodoListGrain> logger, [PersistentState("State")] IPersistentState<State> state)
    {
        _logger = logger;
        _state = state;
    }

    public async Task PushAsync(TodoItem item)
    {
        _state.State.Item ??= new TodoList();
        _state.State.Item.Items.Add(item);

        _logger.LogInformation(
            "{@GrainType} {@GrainKey} now contains {@Todo}",
            GrainType, GrainKey, item);

        await _state.WriteStateAsync();
    }

    public Task<TodoList?> GetAsync()
    {
        return Task.FromResult(_state.State.Item);
    }

    [GenerateSerializer]
    public class State
    {
        [Id(0)] public TodoList? Item { get; set; }
    }
}