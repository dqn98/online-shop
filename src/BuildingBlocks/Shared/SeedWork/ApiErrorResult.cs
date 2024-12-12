namespace Shared.SeedWork;

public class ApiErrorResult<T> : ApiResult<T>
{
    public ApiErrorResult() : this("Some thing wrong happened. Please try again.") 
    { }
    
    public ApiErrorResult(string messsage) : base(false, messsage) {}

    public ApiErrorResult(List<string> errors) : base(false)
    {
        Errors = errors;
    }
    
    public  List<string> Errors { get; set; } = new List<string>();
}