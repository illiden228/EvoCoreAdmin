using EvoCoreAdmin.Identity.Core.Abstractions;
using EvoCoreAdmin.Identity.Core.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EvoCoreAdmin.Identity.WebHost.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register([FromBody] RegisterUserRequest request)
    {
        try
        {
            var dto = await _userService.RegisterAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("get-all")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDto>> GetById(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user is null)
            return NotFound();

        return Ok(user);
    }

    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] ChangeUserStatusRequest request)
    {
        try
        {
            await _userService.ChangeStatusAsync(id, request.IsActive);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost("{id:guid}/projects/{gameProjectKey}/roles")]
    public async Task<IActionResult> AddProjectRole(Guid id, string gameProjectKey, [FromBody] AddProjectRoleRequest request)
    {
        try
        {
            await _userService.AddProjectRoleAsync(id, gameProjectKey, request.RoleId);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:guid}/projects/{gameProjectKey}/roles/{roleName}")]
    public async Task<IActionResult> RemoveProjectRole(Guid id, string gameProjectKey, int roleId)
    {
        try
        {
            await _userService.RemoveProjectRoleAsync(id, gameProjectKey, roleId);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
