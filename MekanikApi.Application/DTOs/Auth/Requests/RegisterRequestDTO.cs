using FluentValidation;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace MekanikApi.Application.DTOs.Auth.Requests
{
    public class RegisterRequestDTO
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string? Middlename { get; set; }

        //[RegularExpression(@"^\234\d*$", ErrorMessage = "Phone number is invalid")]
        public string PhoneNumber { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string Password { get; set; }

        public string? BusinessName { get; set; }

        public string? Homeaddress { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
        public DateOnly? DateOfBirth { get; set; }
        public string? StateOfOrigin { get; set; }

        public string? LGA { get; set; }
    }

    public class RegisterRequestDTOValidator : AbstractValidator<RegisterRequestDTO>
    {
        public RegisterRequestDTOValidator()
        {
            RuleFor(user => user.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Length(13).WithMessage("Phone number must be between 13 characters.")
                .Matches(@"^234").WithMessage("Phone number must start with '234'.")
                .Must(NotContainWhitespace).WithMessage("Phone number cannot contain whitespace.");

            RuleFor(user => user.Firstname)
                .NotEmpty().WithMessage("firstname is required.")
                .Must(NotContainWhitespace).WithMessage("firstname cannot contain whitespace.");

            RuleFor(user => user.Lastname)
                .NotEmpty().WithMessage("lastname is required.")
                .Must(NotContainWhitespace).WithMessage("lastname cannot contain whitespace.");

            RuleFor(user => user.Password)
                .NotEmpty().WithMessage("Password is required.")
                .Length(6).WithMessage("Password must be 6 characters.")
                .Matches(@"^\d{6}$").WithMessage("Password must contain only 6 numeric characters.")
                .Must(NotContainWhitespace).WithMessage("Password cannot contain whitespace.");
            
            RuleFor(user => user.Homeaddress)
                .NotEmpty().WithMessage("Home Address is Required");
            RuleFor(user => user.DateOfBirth)
                .NotEmpty().WithMessage("Date of Birth is required")
                .Must(BeAValidDate)
            .WithMessage("Date must be in the format yyyy-MM-dd.");
            RuleFor(user => user.StateOfOrigin)
                .NotEmpty().WithMessage("State of Origin is Required");
            RuleFor(user => user.LGA)
                .NotEmpty().WithMessage("LGA is Required");
        }

        private bool NotContainWhitespace(string name)
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