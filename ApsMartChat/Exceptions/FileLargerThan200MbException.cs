namespace ApsMartChat.Exceptions;

public class FileLargerThan200MbException : Exception
{
    public FileLargerThan200MbException() : base ("O arquivo é maior do que 200MB") {}
    public FileLargerThan200MbException(string msg) : base(msg) { }
}
