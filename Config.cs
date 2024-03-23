using System.Reflection;

namespace CodeforcesCLI;

public static class Config
{
    private static string AssemblyPath   = Assembly.GetAssembly(typeof(Program)).Location +  "\\Template.cs";
    public static string CURRENT_SELECT_PROBLEM_PATH = null;

    public static string CURRENT_SELECT_INPUT_PATH = null;

    public static string CURRENT_SELECT_ANSWER_PATH = null;

    public static string CURRENT_SELECT_CONTEST = null;

    public static string TEMPLATE_PATH = PathHelper.GetRootPath();

    public static string WORKING_DIRECTORY_PATH = null;

    public static string EXECUTE_PATH = PathHelper.GetExcutablePath();
}