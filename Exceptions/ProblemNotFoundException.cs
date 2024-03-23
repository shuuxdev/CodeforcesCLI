namespace CodeforcesCLI.Exceptions;

public class ProblemNotFoundException: Exception
{
    public ProblemNotFoundException() : base("No such problem was found in the working directory")
    {
    }
}