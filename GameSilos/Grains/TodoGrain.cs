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
        await GrainFactory.GetGrain<IOwnerGrain>(item.OwnerKey).AddTodoAsync(item);
        await _state.WriteStateAsync();
        _logger.LogInformation(
            "{@GrainType} {@GrainKey} now contains {@Todo}",
            GrainType, GrainKey, item);
    }

    public Task<TodoItem?> GetAsync()
    {
        var item = _state.State.Item;
        _logger.LogInformation("GetAsync called on {@GrainType} {@GrainKey} and returned {@Todo}",
            GrainType, GrainKey, item);
        return Task.FromResult(item);
    }

    public async Task ClearAsync()
    {
        await _state.ClearStateAsync();
        _logger.LogInformation("Cleared {@GrainType} {@GrainKey}", 
            GrainType, GrainKey);
    }

    [GenerateSerializer]
    public class State
    {
        [Id(0)] public TodoItem? Item { get; set; }
    }
}