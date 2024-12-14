namespace MekanikApi.Application.DTOs.Common
{
    public class GenericBoolResponse
    {
        public bool Status { get; set; }
        public string? Message { get; set; }

        public object? Result { get; set; }
    }

    public class GenericTypeBoolResponse<T>
    {
        public bool Status { get; set; }
        public string? Message { get; set; }

        public T? Result { get; set; }
    }
}