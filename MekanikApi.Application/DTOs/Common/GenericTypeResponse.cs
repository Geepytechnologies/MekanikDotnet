namespace MekanikApi.Application.DTOs.Common
{
    public class GenericTypeResponse<T>
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public T? Result { get; set; }
    }
}