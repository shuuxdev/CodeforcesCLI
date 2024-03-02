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
        //process.StartInfo.Arguments = "-arg1 -arg2"; 
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardInput = true;
        process.Start();
        process.StandardInput.Write(txt);
        string output = process.StandardOutput.ReadToEnd();

        using (StreamWriter stream = new StreamWriter(File.Open(Config.CURRENT_SELECT_OUTPUT_PATH, FileMode.OpenOrCreate, FileAccess.Write)))
        {
            stream.Write(output);
        }
        process.WaitForExit();
    }
}