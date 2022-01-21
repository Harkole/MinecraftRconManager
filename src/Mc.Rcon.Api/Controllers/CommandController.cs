using Mc.Rcon.Api.Interfaces;
using Mc.Rcon.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Mc.Rcon.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CommandController : ControllerBase
{
    private readonly ILogger<CommandController> _logger;
    private readonly IRconService _rconService;

    public CommandController(ILogger<CommandController> logger, IRconService rconService)
    {
        _logger = logger;
        _rconService = rconService;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Post(CommandRequest commandRequest)
    {
        _logger.LogInformation($"Get called on {nameof(CommandController)}");
        
        if (null == commandRequest.Password || null == commandRequest.Instruction)
        {
            return BadRequest();
        }

        _rconService.Authenticate(commandRequest.Password);
        _rconService.SendCommand(commandRequest.Instruction, out string response);
        _rconService.CloseConnection();

        return Ok(response);
    }
}
