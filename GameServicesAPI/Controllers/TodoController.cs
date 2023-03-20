using GameServicesAPI.Dto;
using GameServicesAPI.Services;
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

    private readonly IAlertService _alertService;

    public TodoController(ILogger<TodoController> logger, IClusterClient client, IAlertService alertService)
    {
        _logger = logger;
        _client = client;
        _alertService = alertService;
    }
    
    [HttpGet("all/{ownerKey:guid}")]
    public async Task<IActionResult> GetAll(Guid ownerKey)
    {
        try
        {
            var todoList = await _client.GetGrain<IOwnerGrain>(ownerKey).GetTodosAsync();
            return Ok(todoList);
        }
        catch (Exception e)
        {
            await _alertService.SendAlertAsync(GetType().Name + ".Get", e);
            _logger.LogError(e, "Error getting todo items");
            return BadRequest("Error getting todo items");
        }
    }

    [HttpGet("{key:guid}")]
    public async Task<IActionResult> Get(Guid key)
    {
        try
        {
            var todo = await _client.GetGrain<ITodoGrain>(key).GetAsync();
            if (todo == null)
            {
                return NotFound();
            }
            return Ok(todo);
        }
        catch (Exception e)
        {
            await _alertService.SendAlertAsync(GetType().Name + ".Get", e);
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
            await _client.GetGrain<ITodoGrain>(item.Key).SetAsync(item);

            var todo = await _client.GetGrain<IOwnerGrain>(item.OwnerKey).GetTodosAsync();
            
            return Ok(todo);
        }
        catch (Exception e)
        {
            await _alertService.SendAlertAsync(GetType().Name + ".Create", e);
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
            await _client.GetGrain<ITodoGrain>(item.Key).SetAsync(item);

            var todo = await _client.GetGrain<ITodoGrain>(item.Key).GetAsync();
            return Ok(todo);
        }
        catch (Exception e)
        {
            await _alertService.SendAlertAsync(GetType().Name + ".Update", e);
            _logger.LogError(e, "Error updating todo item");
            return BadRequest("Error updating todo item");
        }
    }
    
    [HttpDelete("{key:guid}")]
    public async Task<IActionResult> Remove(Guid key)
    {
        try
        {
            _logger.LogInformation("Clear all todo item");
            await _client.GetGrain<IOwnerGrain>(key).RemoveTodoAsync(key);

            return Ok(new
            {
                Success = true,
                Message = "Todo item removed",
            });
        }
        catch (Exception e)
        {
            await _alertService.SendAlertAsync(GetType().Name + ".Clear", e);
            _logger.LogError(e, "Error clearing todo item");
            return BadRequest("Error clearing todo item");
        }
    }

    [HttpDelete("all/{ownerKey:guid}")]
    public async Task<IActionResult> Clear(Guid ownerKey)
    {
        try
        {
            _logger.LogInformation("Clear all todo item");
            await _client.GetGrain<IOwnerGrain>(ownerKey).ClearAsync();

            return Ok(new
            {
                Success = true,
                Message = "Todo items cleared",
            });
        }
        catch (Exception e)
        {
            await _alertService.SendAlertAsync(GetType().Name + ".Clear", e);
            _logger.LogError(e, "Error clearing todo item");
            return BadRequest("Error clearing todo item");
        }
    }
}