using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using UsersService.Application.Services;
using UsersService.Domain.Exceptions;
using UsersService.Domain.Models.Auth;
using UsersService.Infrastructure.Models;

namespace UsersService.Tests.UserServices;

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
