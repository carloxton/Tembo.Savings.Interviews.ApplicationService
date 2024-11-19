namespace Services.Applications;

public class ProcessingException : Exception
{
    public ProcessingException(string message) : base(message) { }
}