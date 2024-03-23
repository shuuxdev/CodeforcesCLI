namespace CodeforcesCLI.Exceptions;

public class NoWorkingDirectorySelectedException : Exception
{
    public NoWorkingDirectorySelectedException() : base("No working directory was selected")
    {
        
    }
}