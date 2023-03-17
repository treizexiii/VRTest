using GameServicesAPI.Dto;
using Microsoft.AspNetCore.Mvc;
using Shared.Grains;
using Shared.Models;

namespace GameServicesAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class TodoController : Controller
{
    private readonly ILogger<TodoController> _logger;
    private readonly IClusterClient _client;

    public TodoController(ILogger<TodoController> logger, IClusterClient client)
    {
        _logger = logger;
        _client = client;
    }

    [HttpGet("{key:guid}")]
    public async Task<IActionResult> Get(Guid key)
    {
        var todo = await _client.GetGrain<ITodoGrain>(key).GetAsync();
        return Ok(todo);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var todo = await _client.GetGrain<ITodoGrain>(Guid.Empty).GetAsync();
        return Ok(todo);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(TodoItemDto itemDto)
    {
        var item = new TodoItem(
            Guid.NewGuid(),
            itemDto.Title,
            itemDto.IsDone,
            itemDto.OwnerKey,
            DateTime.UtcNow);
        _logger.LogInformation("Adding {@Item}", item);
        await _client.GetGrain<ITodoGrain>(Guid.Empty).SetAsync(item);

        var todo = await _client.GetGrain<ITodoGrain>(item.Key).GetAsync();

        return Ok(todo);
    }

    [HttpPut("{key:guid}")]
    public async Task<IActionResult> Update(Guid key, TodoItemDto itemDto)
    {
        var item = new TodoItem(
            key,
            itemDto.Title,
            itemDto.IsDone,
            itemDto.OwnerKey,
            DateTime.UtcNow);
        _logger.LogInformation("Updating {@Item}", item);
        await _client.GetGrain<ITodoGrain>(item.Key).SetAsync(item);

        var todo = await _client.GetGrain<ITodoGrain>(item.Key).GetAsync();

        return Ok(todo);
    }
}