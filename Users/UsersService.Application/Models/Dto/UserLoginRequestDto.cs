using System.ComponentModel.DataAnnotations;

namespace UsersService.Application.Models.Dto;

public class UserLoginRequestDto
{
    [Required(ErrorMessage = "Логин (Login) обязателен для заполнения")]
    public string Login { get; init; } = string.Empty;

    [Required(ErrorMessage = "Пароль (Password) обязателен для заполнения")]
    public string Password { get; init; } = string.Empty;
}
