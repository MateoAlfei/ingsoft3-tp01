namespace AppGastos.Api.Common;

public static class ExceptionResults
{
    public static IResult ToProblem(this AppException ex) => ex switch
    {
        ValidationException => Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status400BadRequest),
        UnauthorizedException => Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status401Unauthorized),
        NotFoundException => Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status404NotFound),
        ConflictException => Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status409Conflict),
        _ => Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError)
    };
}
