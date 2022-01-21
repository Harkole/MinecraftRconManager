using Microsoft.AspNetCore.Mvc;

namespace Mc.Rcon.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CommandController : ControllerBase
{
    private readonly ILogger<CommandController> _logger;

    public CommandController(ILogger<CommandController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public string Get()
    {
        _logger.LogInformation($"Get called on {nameof(CommandController)}");
        return "Hello World!";
    }
}
