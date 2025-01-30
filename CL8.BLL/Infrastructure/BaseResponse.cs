using CL8.Interfaces;

namespace CL8.BLL.Infrastructure
{
    public class BaseResponse<T> : IResponse<T> where T : class, new()
    {
        public T? Value { get; set; }
        public string? Message { get; set; }
    }
}
