namespace MovieSearchBackend.Exceptions;

public class ExternalServiceException(string message, string errorCode = "EXTERNAL_SERVICE_ERROR") : ApiException(message, 502, errorCode)
{
}
