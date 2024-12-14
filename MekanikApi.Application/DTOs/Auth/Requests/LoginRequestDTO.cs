using FluentValidation;

using System.ComponentModel.DataAnnotations;

namespace MekanikApi.Application.DTOs.Auth.Requests
{
    public class LoginRequestDTO
    {
        public string Phone { get; set; }
        public string Password { get; set; }

        [Required]
        public string? PushToken { get; set; }

    }

    public class LoginRequestDTOValidator : AbstractValidator<LoginRequestDTO>
    {
        public LoginRequestDTOValidator()
        {
            RuleFor(user => user.Phone)
                .NotEmpty().WithMessage("Phone number is required.")
                .Length(13).WithMessage("Phone number must be between 13 characters.")
                .Matches(@"^234").WithMessage("Phone number must start with '234'.")
                .Must(NotContainWhitespace).WithMessage("Phone number cannot contain whitespace.");

            RuleFor(user => user.Password)
                .NotEmpty().WithMessage("Password is required.")
                .Length(6).WithMessage("Password must be 6 characters.")
                .Matches(@"^\d{6}$").WithMessage("Password must contain only 6 numeric characters.")
                .Must(NotContainWhitespace).WithMessage("Password cannot contain whitespace.");

            RuleFor(user => user.PushToken)
                .NotEmpty().WithMessage("PushToken is required");
        }

        private bool NotContainWhitespace(string name)
        {
            return !string.IsNullOrWhiteSpace(name) && !name.Any(char.IsWhiteSpace);
        }
    }
}