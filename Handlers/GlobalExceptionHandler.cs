using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Unitic_BE.Exceptions;

namespace Unitic_BE.Handlers;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    //valuetask ở đây xài hiệu năng hơn task, đừng quan tâm làm gì tại vì đây là method được implement từ interface
    //hàm này sẽ được gọi khi có exception xảy ra trong quá trình xử lý request
    //nó là hàm duy nhất của interface IExceptionHandler
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        //tuple deconstruction, gán nhanh value cho 2 biến
        var (statusCode, message) = GetExceptionDetails(exception);
        
        _logger.LogError(exception, exception.Message);

        //gán mã lỗi thủ công cho response và viết vào body dưới dạng json
        //gán cancellationToken để có thể hủy bỏ quá trình khi user hủy request ko tốn tài nguyên
        httpContext.Response.StatusCode = (int)statusCode;
        await httpContext.Response.WriteAsJsonAsync(message, cancellationToken);

        return true;
    }


    //hàm tuple
    private (HttpStatusCode statusCode, string message) GetExceptionDetails(Exception exception)
    {
        //sử dụng switch expression để trả về mã lỗi và message tương ứng với từng loại exception
        return exception switch
        {
            LoginFailedException => (HttpStatusCode.Unauthorized, exception.Message),
            UserAlreadyExistsException => (HttpStatusCode.Conflict, exception.Message),
            RegistrationFailedException => (HttpStatusCode.BadRequest, exception.Message),
            TokenException => (HttpStatusCode.Unauthorized, exception.Message),
            // _ là default
            _ => (HttpStatusCode.InternalServerError, $"An unexpected error occurred: {exception.Message}")
        };
    }
}