namespace ApsMartChat.Exceptions;

public class UnauthorizedException : Exception
{
    public UnauthorizedException(string msg) : base (msg){}
    public UnauthorizedException(){}
}
