using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MekanikApi.Application.DTOs.Auth.Requests
{
    public class ForgotPasswordDTO
    {
        [Required]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }
    }

    public class ForgotPasswordDTOValidator : AbstractValidator<ForgotPasswordDTO>
    {
        public ForgotPasswordDTOValidator()
        {
            RuleFor(pin => pin.Password)
                .NotEmpty().WithMessage("Password is required.")
                .Length(6).WithMessage("Password must be 6 characters.")
                .Matches(@"^\d+$").WithMessage("Password must be digits.")
                .Must(NotContainWhitespace).WithMessage("Password cannot contain whitespace.");
        }

        private bool NotContainWhitespace(string name)
        {
            return !string.IsNullOrWhiteSpace(name) && !name.Any(char.IsWhiteSpace);
        }
    }


    public class ResetPasswordDTO
    {
        [Required]
        public string? OldPassword { get; set; }
        
        [Required]
        public string? NewPassword { get; set; }
    }

    public class ResetPasswordDTOValidator : AbstractValidator<ResetPasswordDTO>
    {
        public ResetPasswordDTOValidator()
        {
            RuleFor(pin => pin.OldPassword)
                .NotEmpty().WithMessage("Old Password is required.")
                .Length(6).WithMessage("Old Password must be 6 characters.")
                .Matches(@"^\d+$").WithMessage("Old Password must be digits.")
                .Must(NotContainWhitespace).WithMessage("Old Password cannot contain whitespace.");

            RuleFor(pin => pin.NewPassword)
               .NotEmpty().WithMessage("New Password is required.")
               .Length(6).WithMessage("New Password must be 6 characters.")
               .Matches(@"^\d+$").WithMessage("New Password must be digits.")
               .Must(NotContainWhitespace).WithMessage("New Password cannot contain whitespace.");
        }

        private bool NotContainWhitespace(string name)
        {
            return !string.IsNullOrWhiteSpace(name) && !name.Any(char.IsWhiteSpace);
        }
    }
}
