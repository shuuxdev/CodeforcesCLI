using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using CodeforcesCLI.Exceptions;
using CodeforcesCLI.Models;
using HtmlAgilityPack;

namespace CodeforcesCLI;


public class ConfigHelper
{
    private const string CODEFORCES = "https://codeforces.com/";
    private const string CONTEST = "contest";
    private const string XPATH_FIND_CONTEST_NAME = @"//title";
    private const string XPATH_FIND_PROBLEMS = "//option[@data-problem-name and not(string-length(@value) > 1)]";
    private const string XPATH_FIND_INPUT = "//div[@class='input']//div[contains(@class,'test-example-line')]";
    private const string XPATH_FIND_INPUT_1 = "//div[@class='input']//pre";
    private const string XPATH_FIND_OUTPUT = "//div[@class='output']//pre";

   
    private static HttpClient client = new HttpClient()
    {
        BaseAddress = new Uri(CODEFORCES),
    };
    public static void InitConfig(string[] args)
    {
        for (int i = 0; i < args.Length; i+=2)
        {
            PropertyInfo? property = typeof(StateUpdatingCommand).GetProperty(args[i]) ?? typeof(FunctionalCommand).GetProperty(args[i]); 
            if (property == null)
            {
                Console.WriteLine("Command [{0}] not exists", args[i]);
                return;
            }
            
            Type propertyType = property.PropertyType;
            if (i + 1 >= args.Length && propertyType != typeof(bool))
            {
                Console.WriteLine("No value was given for command [{0}]", propertyType.Name);
                return;
            }
            if (propertyType == typeof(bool))
            {
                property.SetValue(null, true);
                --i;
            }
            else if (propertyType == typeof(string))
            {
                property.SetValue(null,args[i+1]);
            }

            else if (propertyType == typeof(int))
            {
                bool success = Int32.TryParse(args[i + 1], out int value);
                if (!success)
                {
                    Console.WriteLine("Command only [{0}] accept an numeric data type as value", propertyType.Name);
                    return;
                }
                property.SetValue(null, value);
            }
            
        }
    }
    public static void Log([StringSyntax(StringSyntaxAttribute.CompositeFormat)]string text, ConsoleColor color = ConsoleColor.White, params object?[]? arg)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text, arg);
        Console.ResetColor();
    }
    
    public static string FormatProblemName(string problemName)
    {
        string validChars = @"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_0123456789";
        problemName = problemName.Replace(" - ", "_");
        problemName = problemName.Replace("-", "_");
        problemName = problemName.Replace(" ", "_");
        StringBuilder sb = new StringBuilder(problemName);
        
        for (int i = 0, j = 0; i < problemName.Length; ++i , ++j)
        {
            if (!validChars.Contains(problemName[i]))
            {
                sb.Remove(j,1);
                --j;
            }
        }
        problemName = sb.ToString();
        return problemName;
    }

    public static string ToValidName(string str)
    {
        string validChars = @"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_-0123456789 ";
        StringBuilder sb = new StringBuilder(str);

        for (int i = 0, j = 0; i < str.Length; ++i , ++j)
        {
            if (!validChars.Contains(str[i]))
            {
                sb.Remove(j,1);
                --j;
            }
        }
        str = sb.ToString();
        return str;
    }

    public static void SelectProblem(string problemName)
    {
        if (Config.CURRENT_SELECT_CONTEST == null)
        {
            Log("Please select a contest", ConsoleColor.Red);
            throw new NoContestSelectedException();
        }

        if (Config.WORKING_DIRECTORY_PATH == null)
        {
            Log("Please select a working directory", ConsoleColor.Red);
            throw new NoWorkingDirectorySelectedException();
        }
        string[] allContestDirectories = Directory.GetDirectories(Config.WORKING_DIRECTORY_PATH);
        string pathToContestDirectory = allContestDirectories.Where(dir => dir.Contains(Config.CURRENT_SELECT_CONTEST)).FirstOrDefault();
        if (pathToContestDirectory == null)
        {
            Log("Contest [{0}] doesn't exists in the directory {1}", ConsoleColor.Red, Config.CURRENT_SELECT_CONTEST, Config.WORKING_DIRECTORY_PATH);
            throw new ContestNotFoundException();
        }
        
        
        string[] allProblemDirectories = Directory.GetDirectories(pathToContestDirectory);
        string pathToProblemDirectory = allProblemDirectories.Where(file => file.Split(@"\").LastOrDefault().StartsWith(problemName)).FirstOrDefault();

        if (pathToProblemDirectory == null)
        {
            Log("Problem [{0}] doesn't exists in the directory {1}", ConsoleColor.Red, problemName, pathToContestDirectory);
            throw new ProblemNotFoundException();
        }
        string _problemName = pathToProblemDirectory.Split(@"\").LastOrDefault();
        string[] allFiles = Directory.GetFiles(pathToProblemDirectory);
        if (allFiles.Length != 3)
        {
            Log("Missing files, make sure that you have cs,input,output files exists in the directory", ConsoleColor.Red);
            throw new InvalidProblemStructureException();
        }

        Config.CURRENT_SELECT_PROBLEM_PATH = allFiles[0];
        Config.CURRENT_SELECT_INPUT_PATH = allFiles[1];
        Config.CURRENT_SELECT_ANSWER_PATH = allFiles[2];


        

        string appSetting = JsonSerializer.Serialize(new AppSetting() { CurrentProblem = FormatProblemName(_problemName) });

        File.WriteAllText($"{Config.WORKING_DIRECTORY_PATH}/appsettings.json", appSetting);
        
        Log("Successfully switched to problem {0}", ConsoleColor.Green, problemName);

    }
    
    public static void Pull(string contestId)
    {
        
        HttpResponseMessage response =  client.GetAsync(requestUri: $"{CONTEST}/{contestId}", completionOption:HttpCompletionOption.ResponseContentRead).Result;
        if (response.StatusCode != HttpStatusCode.OK)
        {
            Log("Invalid contest [{0}]", ConsoleColor.Red, contestId);
        }

        Config.CURRENT_SELECT_CONTEST = contestId;
        
        string res =  response.Content.ReadAsStringAsync().Result;
        var doc = new HtmlDocument();
        doc.LoadHtml(res);

        var contestNameNode = doc.DocumentNode.SelectSingleNode(XPATH_FIND_CONTEST_NAME);
        var problemNodes = doc.DocumentNode.SelectNodes(XPATH_FIND_PROBLEMS);
        
        
        
        //Tạo thư mục cho contest
        string contestName = Generator.GenerateContestName(contestId, contestNameNode.InnerHtml);
        string generatedContestPath = Generator.GenerateContestDirectory(contestName);
        
        if (problemNodes == null || problemNodes.Count == 0)
        {
            Log("No problems was found for contest {0}", ConsoleColor.Red, contestId);
            return;
        }
        
        foreach (var problem in problemNodes)
        {
           
            
            string problemName = ToValidName(problem.InnerHtml);
            
            HttpResponseMessage problemPage =  client.GetAsync(requestUri: $"{CONTEST}/{contestId}/problem/${problemName.Trim()[0]}", completionOption:HttpCompletionOption.ResponseContentRead).Result;
            string p =  problemPage.Content.ReadAsStringAsync().Result;
            var page = new HtmlDocument();
            page.LoadHtml(p);
            var inputNodes = page.DocumentNode.SelectNodes(XPATH_FIND_INPUT) ?? page.DocumentNode.SelectNodes(XPATH_FIND_INPUT_1);
            var outputNodes = page.DocumentNode.SelectNodes(XPATH_FIND_OUTPUT);
            
            string pathToProblemDirectory = $"{generatedContestPath}/{problemName}";
            string pathToCsFile = $"{pathToProblemDirectory}/{problemName}.cs";
            string pathToInputFile = $"{pathToProblemDirectory}/[INPUT] {problemName}.txt";
            string pathToOutputFile = $"{pathToProblemDirectory}/[ANSWER] {problemName}.txt";
            Directory.CreateDirectory(pathToProblemDirectory);

            string templateContent = Generator.GenerateTemplate(problemName);
            string inputContent = string.Join(Environment.NewLine, inputNodes.Select(node => node.InnerHtml)).Trim();
            string outputContent = string.Join(Environment.NewLine, outputNodes.Select(node => node.InnerHtml)).Trim();
            // Create a new file (or overwrite existing one)
            using (StreamWriter writer = new StreamWriter( File.Create(pathToCsFile)))
            {
                // Write the content to the file
                writer.Write(templateContent);
            }
            Log("[PROBLEM] File {0} created and content written successfully!",ConsoleColor.Green, problemName);
            using (StreamWriter writer = new StreamWriter(File.Create(pathToInputFile)))
            {
                // Write the content to the file
                writer.Write(inputContent);
            }
            Log("[INPUT] {0} created and content written successfully!",ConsoleColor.Green, problemName);
            using (StreamWriter writer = new StreamWriter(File.Create(pathToOutputFile)))
            {
                // Write the content to the file
                writer.Write(outputContent);
            }
            Log("[ANSWER]{0} created and content written successfully!",ConsoleColor.Green, problemName);
           

        }
        
    }
    
    

}