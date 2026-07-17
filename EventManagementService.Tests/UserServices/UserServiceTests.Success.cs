using EventManagementService.Application.Repositories;
using EventManagementService.Application.Services;
using EventManagementService.Domain.Exceptions;
using EventManagementService.Domain.Models;
using EventManagementService.Domain.Models.Auth;
using EventManagementService.Domain.Services;
using EventManagementService.Infrastructure.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace EventManagementService.Tests.UserServices;

public partial class UserServiceTests
{
    // Регистрация пользователя
    [Fact]
    [Trait("Category", "Success")]
    public async Task CreateUser()
    {
        // Arrange
        using var scope = _serviceProvider.CreateScope();
        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
        var encService = scope.ServiceProvider.GetRequiredService<IEncryptionService>();

        var login = "user";
        var password = "password";
        var role = Role.User;

        var expectedUser = new User(id: Guid.Empty, login, encService.CalcHash(password), role);

        // Act
        var user = await userService.CreateUserAsync(login, password, role);
        expectedUser.Id = user.Id;

        // Assert
        user.Should().BeEquivalentTo(expectedUser);
    }

    // Логин пользователя
    [Fact]
    [Trait("Category", "Success")]
    public async Task LoginUser()
    {
        // Arrange
        using var scope = _serviceProvider.CreateScope();
        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
        var jwtSettings = scope.ServiceProvider.GetRequiredService<IOptions<JWTSettings>>().Value;

        var login = "user";
        var password = "password";
        var role = Role.User;

        var handler = new JwtSecurityTokenHandler();

        var user = await userService.CreateUserAsync(login, password, role);

        // Act
        var token = await userService.LoginAsync(login, password);
        var jwtToken = handler.ReadJwtToken(token);

        // Assert
        jwtToken.Issuer.Should().BeEquivalentTo(jwtSettings.Issuer);
        jwtToken.Audiences.Should().BeEquivalentTo(jwtSettings.Audience);

        var userId = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        userId.Should().BeEquivalentTo(user.Id.ToString());

        var userRole = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
        userRole.Should().BeEquivalentTo(user.Role.ToString());
    }
}
