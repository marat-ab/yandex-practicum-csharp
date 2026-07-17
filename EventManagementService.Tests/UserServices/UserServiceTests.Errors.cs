using EventManagementService.Application.Repositories;
using EventManagementService.Application.Services;
using EventManagementService.Domain.Exceptions;
using EventManagementService.Domain.Models;
using EventManagementService.Domain.Models.Auth;
using EventManagementService.Infrastructure.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace EventManagementService.Tests.UserServices;

public partial class UserServiceTests
{
    // Логин пользователя. Пользователь не нейден
    [Fact]
    [Trait("Category", "Success")]
    public async Task LoginNotFoundUser()
    {
        // Arrange
        using var scope = _serviceProvider.CreateScope();
        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
        var jwtSettings = scope.ServiceProvider.GetRequiredService<IOptions<JWTSettings>>().Value;

        var login = "user";
        var password = "password";

        // Act
        Func<Task> act = async () => await userService.LoginAsync(login, password);

        // Assert
        await act.Should().ThrowAsync<UserNotFoundException>()
           .WithMessage($"User with login {login}, not found");
    }

    // Логин пользователя. Некоррекный пароль
    [Fact]
    [Trait("Category", "Success")]
    public async Task LoginUserBadPassword()
    {
        // Arrange
        using var scope = _serviceProvider.CreateScope();
        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
        var jwtSettings = scope.ServiceProvider.GetRequiredService<IOptions<JWTSettings>>().Value;

        var login = "user";
        var password = "password";
        var role = Role.User;

        var user = await userService.CreateUserAsync(login, password, role);

        // Act
        Func<Task> act = async () => await userService.LoginAsync(login, "123");

        // Assert
        await act.Should().ThrowAsync<UserBadPasswordException>()
           .WithMessage($"Bad password for {login}. Access denied");
    }
}
