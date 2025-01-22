using FluentValidation;
using MekanikApi.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace MekanikApi.Application.DTOs.Auth.Requests
{
    public class RegisterRequestDTO
    {
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? Middlename { get; set; }

        public string? PhoneNumber { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public string? Password { get; set; }

        public string? PushToken { get; set; }

        public ApplicationProfile Profile { get; set; }

    
    }

    public class RegisterRequestDTOValidator : AbstractValidator<RegisterRequestDTO>
    {
        public RegisterRequestDTOValidator()
        {
            RuleFor(user => user.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Length(11).WithMessage("Phone number must be between 11 characters.")
                //.Matches(@"^234").WithMessage("Phone number must start with '234'.")
                .Must(NotContainWhitespace).WithMessage("Phone number cannot contain whitespace.");

            RuleFor(user => user.Firstname)
                .NotEmpty().WithMessage("firstname is required.")
                .Must(NotContainWhitespace).WithMessage("firstname cannot contain whitespace.");

            RuleFor(user => user.Lastname)
                .NotEmpty().WithMessage("lastname is required.")
                .Must(NotContainWhitespace).WithMessage("lastname cannot contain whitespace.");

            RuleFor(user => user.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(4).WithMessage("Password must be more than 4 characters.")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches(@"\d").WithMessage("Password must contain at least one number.")
            .Matches(@"[\W_]").WithMessage("Password must contain at least one special character.");

            
        }

        private bool NotContainWhitespace(string? name)
        {
            return !string.IsNullOrWhiteSpace(name) && !name.Any(char.IsWhiteSpace);
        }
        private bool BeAValidDate(DateOnly? date)
        {
            if (!date.HasValue)
            {
                return false;
            }

            return true;
        }
    }
}