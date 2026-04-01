using System.ComponentModel.DataAnnotations;

namespace EventManagementServices.Models;

public class EventRequestDto : IValidatableObject
{
    [Required(ErrorMessage = "Заголовок обязателен для заполнения")]
    public string Title { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    [Required(ErrorMessage = "Дата/время начала события обязателено для заполнения")]
    public DateTime StartAt { get; init; }

    [Required(ErrorMessage = "Дата/время окончания события обязателено для заполнения")]
    public DateTime EndAt { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndAt < StartAt)
        {
            var result = new ValidationResult("EndAt должно быть позже StartAt.");
            yield return result;
        }
    }
}
