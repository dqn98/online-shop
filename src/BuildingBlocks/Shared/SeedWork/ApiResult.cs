namespace Shared.SeedWork;

public class ApiResult<T>
{
    public ApiResult()
    {
    }
    
    public ApiResult(bool isSuccess)
    {
        IsSuccess = isSuccess;
    }
    
    public ApiResult(bool isSuccess, T data)
    {
        IsSuccess = isSuccess;
        Data = data;
    }

    public ApiResult(bool isSuccess, string message)
    {   
        IsSuccess = isSuccess;
        Message = message;
    }

    public ApiResult(bool isSuccess, T data, string message)
    {
        Data = data;
        IsSuccess = isSuccess;
        Message = message;
    }
    
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public T Data { get; }
}