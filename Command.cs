namespace CodeforcesCLI;

public class StateUpdatingCommand
{
    //Fetch contest, problems and generate input,output files
    public static string pull
    {
        set =>  ConfigHelper.Pull(value);
    }
    //Problem
    public static string p
    {
        set => ConfigHelper.SelectProblem(value);
    }
    
    //Contest
    public static string c
    {
        set => Config.CURRENT_SELECT_CONTEST = value;
    }
}

//Commands that do not change  any of the property in Config
public static class FunctionalCommand
{
    //List all current contests
    public static bool list { get; set; }

    public static bool run
    {
        set => TestingHelper.Run();
    }
    
}