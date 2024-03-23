namespace CodeforcesCLI.Exceptions;

public class NoProblemSelectedException : Exception
{
    public NoProblemSelectedException() : base("Please select a problem")
    {
    }
}