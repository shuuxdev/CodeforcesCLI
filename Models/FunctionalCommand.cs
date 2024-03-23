namespace CodeforcesCLI.Models;

public static class FunctionalCommand
{
    //List all current contests
    public static bool list { get; set; }

    public static bool run
    {
        set => TestingHelper.Run();
    }
    
}