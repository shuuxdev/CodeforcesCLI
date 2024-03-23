namespace CodeforcesCLI.Models;

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