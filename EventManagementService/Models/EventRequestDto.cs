using System.ComponentModel.DataAnnotations;

namespace EventManagementService.Models;

public class EventRequestDto : IValidatableObject
{
    [Required(ErrorMessage = "Заголовок (Title) обязателен для заполнения")]
    public string Title { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    [Required(ErrorMessage = "Дата/время начала события (StartAt) обязателено для заполнения")]
    public DateTime? StartAt { get; init; }

    [Required(ErrorMessage = "Дата/время окончания события (EndAt) обязателено для заполнения")]
    public DateTime? EndAt { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndAt < StartAt)
        {
            var result = new ValidationResult("EndAt должно быть позже StartAt.");
            yield return result;
        }
    }
}
