namespace MovieSearchBackend.Exceptions;

public class NotFoundException(string message, string errorCode = "NOT_FOUND") : ApiException(message, 404, errorCode)
{
}
