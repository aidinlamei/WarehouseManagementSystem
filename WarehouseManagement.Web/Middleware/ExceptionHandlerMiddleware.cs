using System.Net;
using System.Text.Json;
using WarehouseManagement.Core.Exceptions;

namespace WarehouseManagement.Web.Middleware
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;
        private readonly IWebHostEnvironment _environment;

        public ExceptionHandlerMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlerMiddleware> logger,
            IWebHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var errorResponse = new ErrorResponse
            {
                Timestamp = DateTime.UtcNow
            };

            switch (exception)
            {
                case ValidationException validationEx:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.ErrorCode = validationEx.ErrorCode ?? ErrorCodes.VALIDATION_ERROR;
                    errorResponse.Message = validationEx.Message;
                    errorResponse.Details = validationEx.Errors;
                    break;

                case NotFoundException notFoundEx:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorResponse.ErrorCode = notFoundEx.ErrorCode ?? ErrorCodes.NOT_FOUND;
                    errorResponse.Message = notFoundEx.Message;
                    break;

                case BusinessException businessEx:
                    response.StatusCode = (int)HttpStatusCode.Conflict;
                    errorResponse.ErrorCode = businessEx.ErrorCode ?? ErrorCodes.BUSINESS_RULE_VIOLATION;
                    errorResponse.Message = businessEx.Message;
                    break;

                case AccessDeniedException accessDeniedEx:
                    response.StatusCode = (int)HttpStatusCode.Forbidden;
                    errorResponse.ErrorCode = accessDeniedEx.ErrorCode ?? ErrorCodes.ACCESS_DENIED;
                    errorResponse.Message = accessDeniedEx.Message;
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse.ErrorCode = ErrorCodes.UNEXPECTED_ERROR;
                    errorResponse.Message = _environment.IsDevelopment()
                        ? exception.Message
                        : "An unexpected error occurred";

                    if (_environment.IsDevelopment())
                    {
                        errorResponse.StackTrace = exception.StackTrace;
                    }
                    break;
            }

            // Log the exception
            _logger.LogError(exception, "An error occurred: {Message}", exception.Message);

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(errorResponse, options);

            await response.WriteAsync(json);
        }
    }

    public class ErrorResponse
    {
        public string ErrorCode { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public Dictionary<string, string[]> Details { get; set; }
        public string StackTrace { get; set; }
    }
}
