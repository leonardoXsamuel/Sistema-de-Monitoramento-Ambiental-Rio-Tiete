namespace ApsMartChat.Exceptions;

public class UserExistsException : Exception
{
    public UserExistsException() { }
    public UserExistsException(string msg) : base(msg) { }
}
