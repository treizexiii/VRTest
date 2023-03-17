﻿using Microsoft.Extensions.Logging;
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
        _state.State.Item ??= new List<TodoItem>();
        if (_state.State.Item.Any(x => x.Key == item.Key))
        {
            var removedItem = _state.State.Item.First(x => x.Key == item.Key);
            _state.State.Item.Remove(removedItem);
        }
        _state.State.Item.Add(item);
        await _state.WriteStateAsync();

        _logger.LogInformation(
            "{@GrainType} {@GrainKey} now contains {@Todo}",
            GrainType, GrainKey, item);
    }

    public Task<TodoItem?> GetAsync(Guid key)
    {
        var list = _state.State.Item;
        var item = list?.FirstOrDefault(x => x.Key == key);
        return Task.FromResult(item);
    }

    public Task<IEnumerable<TodoItem>?> GetAllAsync()
    {
        return Task.FromResult(_state.State.Item as IEnumerable<TodoItem>);
    }

    public Task<IEnumerable<TodoItem>> RemoveAsync(Guid key)
    {
        _state.State.Item ??= new List<TodoItem>();
        if (_state.State.Item.Any(x => x.Key == key))
        {
            var removedItem = _state.State.Item.First(x => x.Key == key);
            _state.State.Item.Remove(removedItem);
        }
        return Task.FromResult(_state.State.Item as IEnumerable<TodoItem>);
    }

    public Task<IEnumerable<TodoItem>> ClearAsync()
    {
        _state.State.Item = new List<TodoItem>();
        return Task.FromResult(_state.State.Item as IEnumerable<TodoItem>);
    }

    [GenerateSerializer]
    public class State
    {
        [Id(0)] public List<TodoItem>? Item { get; set; }
    }
}