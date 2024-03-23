namespace CodeforcesCLI.Exceptions;

public class ContestNotFoundException : Exception
{
    public ContestNotFoundException() : base("No such contest was found in the working directory")
    {
    }
}