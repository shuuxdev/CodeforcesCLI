namespace CodeforcesCLI.Exceptions;

public class NoContestSelectedException : Exception
{
    
    public NoContestSelectedException() : base("Please selected a contest")
    {
    }
}