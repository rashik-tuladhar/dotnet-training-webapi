using DlmsWebApi.Shared.AuthorData;

namespace DlmsWebApi.Shared;

/// <summary>
/// Wraps all API responses in consistent format.
/// WHY: Client always knows response structure; easier parsing
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }
    
    public static ApiResponse<T> SuccessMessage(T? data = default, string message = "Success")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            Errors = null
        };
    }

    // // Factory methods for common responses
    // public static ApiResponse<T> Ok(T? data = default, string message = "Success")
    // {
    //     return new ApiResponse<T>
    //     {
    //         Success = true,
    //         Message = message,
    //         Data = data,
    //         Errors = null
    //     };
    // }
    //
    // public static ApiResponse<T> Error(string message, List<string>? errors = null)
    // {
    //     return new ApiResponse<T>
    //     {
    //         Success = false,
    //         Message = message,
    //         Data = default,
    //         Errors = errors ?? new List<string> { message }
    //     };
    // }
    //
    // public static ApiResponse<T> NotFound(string message = "Resource not found")
    // {
    //     return new ApiResponse<T>
    //     {
    //         Success = false,
    //         Message = message,
    //         Data = default,
    //         Errors = new List<string> { message }
    //     };
    // }
}


// return NotFound(ApiResponse<BookResponse>.NotFound("Book not found"));