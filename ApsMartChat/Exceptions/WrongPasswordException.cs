namespace ApsMartChat.Exceptions;

public class WrongPasswordException : Exception
{
    public WrongPasswordException() { }
    public WrongPasswordException(string msg) : base(msg) { }
}
