using System.Net.Security;
using System.Text;

namespace CodeforcesCLI;

public static class Generator
{
    public static void GenerateProblem(string pathToProblem, string fileExtension)
    {
        File.Create($"{pathToProblem}.{fileExtension}");
    }
    public static string GenerateContestName(string contestId, string currentName)
    {
        return currentName.StartsWith("Dashboard - ") ? contestId + " - " + currentName.Split(" - ")[1] : currentName;
    }
    public static string? GenerateContestDirectory(string contestName)
    {
        //Tạo thư mục cho contest hoặc tái sử dụng   
        var currentExecutePath = Environment.CurrentDirectory;
        if (!currentExecutePath.EndsWith(contestName))
        {
            try
            {
                string path = $"{currentExecutePath}/{contestName}";
                Directory.CreateDirectory(path);
                return path;
            }
            catch (DirectoryNotFoundException exception)
            {
                ConfigHelper.Log("Error while retrieving the current console path: {0} \n StackTrace: {1}", ConsoleColor.Red, currentExecutePath, exception.StackTrace);
            }
        }
        return null;
    }

    public static string GenerateTemplate(string problemName)
    {
        string template = File.ReadAllText(Config.TEMPLATE_PATH);
        problemName = ConfigHelper.FormatProblemName(problemName);
        template = template.Replace("TemplateGoesHere", $"{problemName} : IProblem");
        return template;
    }

    
}