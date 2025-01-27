namespace CL8.Interfaces
{
    public interface IResponse<T> where T : class, new()
    {
        public T Value { get; set; }
        public string Message { get; set; }
    }
}