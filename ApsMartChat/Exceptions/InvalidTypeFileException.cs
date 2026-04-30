namespace ApsMartChat.Exceptions;

public class InvalidTypeFileException : Exception
{
    public InvalidTypeFileException(string msg) : base(msg) { }
    public InvalidTypeFileException() : base("O tipo do arquivo é inválido.") { }
}
