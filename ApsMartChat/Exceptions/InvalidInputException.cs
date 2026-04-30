namespace ApsMartChat.Exceptions;

public class InvalidInputException : Exception
{
    public InvalidInputException() : base("O tipo do entrada é inválida.") { }

    public InvalidInputException(string msg) : base(msg) { }
}
