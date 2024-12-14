
namespace MekanikApi.Application.DTOs.Auth.Responses
{
    public record UserLoginResponseDTO(Guid UserId, string FirstName, string? MiddleName, string LastName, string PhoneNumber, string Email,string BusinessName, DateOnly? DateOfBirth,string HomeAddress, string StateOfOrigin, string LGA,string ImageUrl,   bool AccountPinSet,bool Kyc, string AccessToken, string RefreshToken);

    public record GetUserResponseDTO(Guid UserId, string FirstName, string? MiddleName, string LastName, string PhoneNumber, string Email, string BusinessName, DateOnly? DateOfBirth, string HomeAddress, string StateOfOrigin, string LGA, string ImageUrl, bool AccountPinSet, bool Kyc, string AccountName);

    public record UserUpdateResponseDTO(Guid UserId, string FirstName, string? MiddleName, string LastName, string PhoneNumber, string Email, string BusinessName, DateOnly? DateOfBirth, string HomeAddress, string StateOfOrigin, string LGA, string ImageUrl);

    

    public record UserNotVerifiedResponseDTO(string Firstname, string Email, string PhoneNumber);
}