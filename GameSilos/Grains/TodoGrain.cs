using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using Shared.Grains;
using Shared.Models;

namespace GameSilos.Grains;

public class TodoGrain : Grain, ITodoGrain
{
    private readonly ILogger<TodoGrain> _logger;
    private readonly IPersistentState<State> _state;

    private static string GrainType => nameof(TodoGrain);
    private Guid GrainKey => this.GetPrimaryKey();
    
    public TodoGrain(ILogger<TodoGrain> logger, [PersistentState("State")] IPersistentState<State> state)
    {
        _logger = logger;
        _state = state;
    }
    
    public async Task SetAsync(TodoItem item)
    {
        _state.State.Item = item;
        await _state.WriteStateAsync();

        _logger.LogInformation(
            "{@GrainType} {@GrainKey} now contains {@Todo}",
            GrainType, GrainKey, item);
    }

    public Task<TodoItem?> GetAsync()
    {
        return Task.FromResult(_state.State.Item);
    }
    
    [GenerateSerializer]
    public class State
    {
        [Id(0)] public List<TodoItem>? Item { get; set; }
    }
}