using System.Reflection;

namespace CodeforcesCLI;

public class PathHelper
{
    private static string PROGRAM_LOCATION = Assembly.GetAssembly(typeof(Program)).Location;
    public  static string GetRootPath()
    {
        int indexOfBinFolder = PROGRAM_LOCATION.IndexOf("\\bin", 0);
        return PROGRAM_LOCATION.Substring(0, indexOfBinFolder);
    }

    public static string GetExcutablePath()
    {
        int indexOfCodeforceDll = PROGRAM_LOCATION.IndexOf("\\CodeforcesCLI.dll", 0);
        return PROGRAM_LOCATION.Substring(0, indexOfCodeforceDll);
    }
    
}