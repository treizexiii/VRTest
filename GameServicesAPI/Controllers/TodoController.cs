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
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var todo = await _client.GetGrain<ITodoGrain>(Guid.Empty).GetAllAsync();
            return Ok(todo);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting todo items");
            return BadRequest("Error getting todo items");
        }
    }

    [HttpGet("{key:guid}")]
    public async Task<IActionResult> Get(Guid key)
    {
        try
        {
            var todo = await _client.GetGrain<ITodoGrain>(Guid.Empty).GetAsync(key);
            if (todo == null)
            {
                return NotFound();
            }
            return Ok(todo);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting todo item");
            return BadRequest("Error getting todo item");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create(TodoItemDto itemDto)
    {
        try
        {
            var item = new TodoItem(
                Guid.NewGuid(),
                itemDto.Title,
                itemDto.IsDone,
                itemDto.OwnerKey,
                DateTime.UtcNow);
            _logger.LogInformation("Adding {@Item}", item);
            await _client.GetGrain<ITodoGrain>(Guid.Empty).SetAsync(item);

            var todo = await _client.GetGrain<ITodoGrain>(Guid.Empty).GetAllAsync();

            return Ok(todo);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error creating todo item");
            return BadRequest("Error creating todo item");
        }
    }

    [HttpPut("{key:guid}")]
    public async Task<IActionResult> Update(Guid key, TodoItemDto itemDto)
    {
        try
        {
            var item = new TodoItem(
                key,
                itemDto.Title,
                itemDto.IsDone,
                itemDto.OwnerKey,
                DateTime.UtcNow);
            _logger.LogInformation("Updating {@Item}", item);
            await _client.GetGrain<ITodoGrain>(Guid.Empty).SetAsync(item);

            var todo = await _client.GetGrain<ITodoGrain>(Guid.Empty).GetAsync(key);

            return Ok(todo);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating todo item");
            return BadRequest("Error updating todo item");
        }
    }
}