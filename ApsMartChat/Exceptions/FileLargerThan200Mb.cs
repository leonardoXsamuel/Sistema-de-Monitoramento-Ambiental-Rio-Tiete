namespace ApsMartChat.Exceptions;

public class FileLargerThan200Mb : Exception
{
    public FileLargerThan200Mb() : base ("O arquivo é maior do que 200MB") {}
    public FileLargerThan200Mb(string msg) : base(msg) { }
}
