using FluentValidation;

namespace MekanikApi.Application.DTOs.Sms
{
    public class ForgotPasswordOtpRequestDTO
    {
        public string? MobileNumber { get; set; }

    }
    public class OtpRequestDTO
    {
        public string? Name { get; set; }
        public string? MobileNumber { get; set; }
    }

    public class OtpRequestDTOValidator : AbstractValidator<OtpRequestDTO>
    {
        public OtpRequestDTOValidator()
        {
            RuleFor(user => user.Name)
                .NotEmpty().WithMessage("Name is required.")
                .Must(NotContainWhitespace).WithMessage("Name cannot contain whitespace.");

            RuleFor(user => user.MobileNumber)
                .NotEmpty().WithMessage("MobileNumber is required.")
                .Length(13).WithMessage("MobileNumber must be between 13 characters.")
                .Matches(@"^234").WithMessage("MobileNumber must start with '234'.")
                .Must(NotContainWhitespace).WithMessage("MobileNumber cannot contain whitespace.");
        }

        private bool NotContainWhitespace(string? name)
        {
            return !string.IsNullOrWhiteSpace(name) && !name.Any(char.IsWhiteSpace);
        }
    }

    public class ForgotPasswordOtpRequestDTOValidator : AbstractValidator<ForgotPasswordOtpRequestDTO>
    {
        public ForgotPasswordOtpRequestDTOValidator()
        {
            

            RuleFor(user => user.MobileNumber)
                .NotEmpty().WithMessage("MobileNumber is required.")
                .Length(13).WithMessage("MobileNumber must be between 13 characters.")
                .Matches(@"^234").WithMessage("MobileNumber must start with '234'.")
                .Must(NotContainWhitespace).WithMessage("MobileNumber cannot contain whitespace.");
        }

        private bool NotContainWhitespace(string? name)
        {
            return !string.IsNullOrWhiteSpace(name) && !name.Any(char.IsWhiteSpace);
        }
    }

    public class ConfirmOtpDTO
    {
        public string? Email { get; set; }
        public string? Otp { get; set; }
        
        
    }
    public class ConfirmOtpDTOValidator : AbstractValidator<ConfirmOtpDTO>
    {
        public ConfirmOtpDTOValidator()
        {
            RuleFor(user => user.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email is invalid")
                .Must(NotContainWhitespace).WithMessage("PinId cannot contain whitespace.");
            RuleFor(user => user.Otp)
                .NotEmpty().WithMessage("Otp is required.")
                .Length(4).WithMessage("Otp is invalid")
                .Must(NotContainWhitespace).WithMessage("Otp cannot contain whitespace.");
        }

        private bool NotContainWhitespace(string name)
        {
            return !string.IsNullOrWhiteSpace(name) && !name.Any(char.IsWhiteSpace);
        }
    }

}