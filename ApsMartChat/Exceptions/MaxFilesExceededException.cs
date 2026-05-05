namespace ApsMartChat.Exceptions;

public class MaxFilesExceededException : Exception
{
    public MaxFilesExceededException() : base("O tipo do entrada é inválida."){}
    public MaxFilesExceededException(string msg) : base(msg){}
}
