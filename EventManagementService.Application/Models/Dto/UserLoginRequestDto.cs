using EventManagementService.Domain.Models.Auth;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EventManagementService.Application.Models.Dto;

public class UserLoginRequestDto
{
    [Required(ErrorMessage = "Логин (Login) обязателен для заполнения")]
    public string Login { get; init; } = string.Empty;

    [Required(ErrorMessage = "Пароль (Password) обязателен для заполнения")]
    public string Password { get; init; } = string.Empty;
}
