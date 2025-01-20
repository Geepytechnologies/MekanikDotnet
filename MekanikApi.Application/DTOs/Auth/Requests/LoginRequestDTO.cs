using FluentValidation;

using System.ComponentModel.DataAnnotations;

namespace MekanikApi.Application.DTOs.Auth.Requests
{
    public class LoginRequestDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }

        [Required]
        public string? PushToken { get; set; }

    }

    public class LoginRequestDTOValidator : AbstractValidator<LoginRequestDTO>
    {
        public LoginRequestDTOValidator()
        {
            RuleFor(user => user.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid Email")
                .Must(NotContainWhitespace).WithMessage("Email cannot contain whitespace.");

            

            RuleFor(user => user.PushToken)
                .NotEmpty().WithMessage("PushToken is required");
        }

        private bool NotContainWhitespace(string name)
        {
            return !string.IsNullOrWhiteSpace(name) && !name.Any(char.IsWhiteSpace);
        }
    }
}