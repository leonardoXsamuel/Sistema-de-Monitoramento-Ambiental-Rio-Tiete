namespace ApsMartChat.Exceptions;

public class JwtKeyNotConfiguredException : Exception
{
    public JwtKeyNotConfiguredException (string msg) : base(msg) { }
    public JwtKeyNotConfiguredException() : base("JWT Key não configurada") { }
}
