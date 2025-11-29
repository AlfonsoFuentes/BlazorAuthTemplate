namespace Shared.Dtos.General
{
    public class GeneralDto
    {
        public bool Suceeded { get; set; }
        public string Message { get; set; } = string.Empty;
    }
    public class GeneralDto<T> : GeneralDto
    {
        public T Data { get; set; }=default(T)!; 
    }
}
