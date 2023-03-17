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
            await _alertService.SendAlertAsync(GetType().Name + ".GetAll", e);
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
            await _client.GetGrain<ITodoGrain>(Guid.Empty).SetAsync(item);

            var todo = await _client.GetGrain<ITodoGrain>(Guid.Empty).GetAllAsync();
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
            await _client.GetGrain<ITodoGrain>(Guid.Empty).SetAsync(item);

            var todo = await _client.GetGrain<ITodoGrain>(Guid.Empty).GetAsync(key);
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
    public async Task<IActionResult> Remove(Guid guid)
    {
        try
        {
            _logger.LogInformation("Removing todo item {@Guid}", guid);
            var list = await _client.GetGrain<ITodoGrain>(Guid.Empty).RemoveAsync(guid);

            return Ok(list);
        }
        catch (Exception e)
        {
            await _alertService.SendAlertAsync(GetType().Name + ".Remove", e);
            _logger.LogError(e, "Error removing todo item");
            return BadRequest("Error removing todo item");
        }
    }

    [HttpDelete()]
    public async Task<IActionResult> Clear()
    {
        try
        {
            _logger.LogInformation("Clear all todo item");
            var list = await _client.GetGrain<ITodoGrain>(Guid.Empty).ClearAsync();

            return Ok(list);
        }
        catch (Exception e)
        {
            await _alertService.SendAlertAsync(GetType().Name + ".Clear", e);
            _logger.LogError(e, "Error clearing todo item");
            return BadRequest("Error clearing todo item");
        }
    }
}