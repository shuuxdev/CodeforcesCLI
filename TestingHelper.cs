using System.Diagnostics;

namespace CodeforcesCLI;

public class TestingHelper
{
    public static void Run(string path = null)
    {
        if (path == null) path = Config.EXECUTE_PATH;
        string txt = File.ReadAllText(Config.CURRENT_SELECT_INPUT_PATH);
        Process process = new Process();
        process.StartInfo.FileName = path;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardInput = true;
        process.OutputDataReceived += IncomingData;
        process.ErrorDataReceived += ErrorDataReceived; // Handle errors
        process.Start();
        process.BeginOutputReadLine();
        process.StandardInput.WriteLine(txt);
        process.StandardInput.Flush();
        ConfigHelper.Log("Input sent {0}", ConsoleColor.Yellow, txt);
        process.WaitForExit();
        
    }

    static void IncomingData(object sender, DataReceivedEventArgs args)
    {
        ConfigHelper.Log("Output received {0}", ConsoleColor.DarkMagenta, args.Data);
        File.WriteAllText(Config.CURRENT_SELECT_ANSWER_PATH, args.Data);
    }
    static void ErrorDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (!string.IsNullOrEmpty(e.Data))
        {
            Console.WriteLine($"Error: {e.Data}");
        }
    }
}