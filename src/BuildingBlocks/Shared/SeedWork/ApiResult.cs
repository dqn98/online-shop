﻿using System.Text.Json.Serialization;

namespace Shared.SeedWork;

public class ApiResult<T>
{
    public ApiResult()
    { }

    [JsonConstructor]
    public ApiResult(bool isSucceeded, string message = null)
    {
        Message = message;
        IsSucceeded = isSucceeded;
    }

    public ApiResult(bool isSucceeded, T data, string message = null)
    {
        Data = data;
        IsSucceeded = isSucceeded;
        Message = message;
    }

    public bool IsSucceeded { get; set; }
    public string Message { get; set; }
    public T Data { get; }
}