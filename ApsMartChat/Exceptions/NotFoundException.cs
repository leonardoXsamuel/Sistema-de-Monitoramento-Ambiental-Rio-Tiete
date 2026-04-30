namespace ApsMartChat.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string msg) : base(msg){}
    public NotFoundException() : base("Recurso não encontrado"){ }

}
