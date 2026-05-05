namespace ApsMartChat.Exceptions.Handlers;

public class ExceptionHandler
{
    private readonly RequestDelegate _next;

    public ExceptionHandler(RequestDelegate next) => _next = next;


    private static async Task Handle(HttpContext context, int status, string message)
    {
        context.Response.StatusCode = status;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(new
        {
            status,
            message
        });
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (UserExistsException ex)
        {
            await Handle(context, 409, ex.Message);
        }
        catch (WrongPasswordException ex)
        {
            await Handle(context, 401, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            await Handle(context, 400, ex.Message);
        }
        catch (UnauthorizedException ex)
        {
            await Handle(context, 401, ex.Message);
        }
        catch (NotFoundException ex)
        {
            await Handle(context, 404, ex.Message);
        }
        catch (InvalidTypeFileException ex)
        {
            await Handle(context, 400, ex.Message);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            await Handle(context, 400, ex.Message);
        }
        catch (JwtKeyNotConfiguredException ex)
        {
            await Handle(context, 401, ex.Message);
        }
        catch (InvalidInputException ex)
        {
            await Handle(context, 400, ex.Message);
        }
        catch (FileLargerThan200MbException ex)
        {
            await Handle(context, 400, ex.Message);
        }
        catch (MaxFilesExceededException ex)
        {
            await Handle(context, 400, ex.Message);
        }

        // sempre manter em ultimo. por ser a generic exception, cobre tudo
        catch (Exception ex)
        {
            await Handle(context, 500, ex.Message); 
        }
    }
}
