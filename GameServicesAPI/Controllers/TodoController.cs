﻿using Microsoft.AspNetCore.Mvc;
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
    
    [HttpPost("create")]
    public async Task<IActionResult> Create(TodoItem item)
    {
        _logger.LogInformation("Adding {@item}.", item);
        await _client.GetGrain<ITodoGrain>(item.Key).SetAsync(item);

        return Ok();
    }
}