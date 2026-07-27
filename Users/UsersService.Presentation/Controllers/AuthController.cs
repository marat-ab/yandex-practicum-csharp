using Microsoft.AspNetCore.Mvc;
using UsersService.Application.Models.Dto;
using UsersService.Application.Services;
using UsersService.Domain.Models.Auth;

namespace UsersService.Presentation.Controllers;

[ApiController]
[Route("/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] UserRegisterRequestDto userRequest)
    {
        var role = userRequest.Role is null
            ? Role.User
            : userRequest.Role.Value;

        var _ = await _userService.CreateUserAsync(userRequest.Login, userRequest.Password, role);

        return NoContent();
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login([FromBody] UserLoginRequestDto userRequest)
    {
        var accessToken = await _userService.LoginAsync(userRequest.Login, userRequest.Password);

        return Ok(new { Token = accessToken });
    }
}
