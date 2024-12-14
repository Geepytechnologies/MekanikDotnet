namespace MekanikApi.Application.DTOs.Common
{
    public class GenericResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public object? Result { get; set; }
    }
}