using Python.Runtime;
using System.IO;
using UnityEditor.Scripting.Python;
using UnityEngine;

public class TokenizerRunner
{
    private string pythonScriptsPath = Application.dataPath + "/_TTS/PythonScripts/";

    /// <summary>
    /// Executes the tokenizer script with the provided input text.
    /// </summary>
    /// <param name="inputText">The input text to tokenize.</param>
    /// <returns>The tokenized string.</returns>
    public string ExecuteTokenizer(string inputText)
    {
        PythonRunner.EnsureInitialized();
        inputText = EscapeInputText(inputText);
        string pythonScript = GeneratePythonScript(inputText);
        return RunPythonScriptAndCaptureOutput(pythonScript);
    }

    /// <summary>
    /// Escapes special characters in the input text for Python.
    /// </summary>
    /// <param name="inputText">The input text to escape.</param>
    /// <returns>The escaped input text.</returns>
    private string EscapeInputText(string inputText)
    {
        return inputText.Replace("'", "\\'");
    }

    /// <summary>
    /// Generates the Python script to execute, incorporating the escaped input text and path adjustments.
    /// </summary>
    /// <param name="inputText">The escaped input text to tokenize.</param>
    /// <returns>The Python script as a string.</returns>
    /// <remarks>
    /// Using raw string literals (r, like r"my\path".), which do not treat backslashes as escape characters.
    /// </remarks>
    private string GeneratePythonScript(string inputText)
    {
        return $@"
import sys
sys.path.append(r'{pythonScriptsPath}')
import yaml
import tokenizer
from tokenizer import TTSTokenizer

with open(r'{pythonScriptsPath}config.yaml', 'r', encoding='utf-8') as f:
   config = yaml.safe_load(f)

tokenizer = TTSTokenizer(config['token']['list'])
tokenized = tokenizer(r'{inputText}')
print(tokenized)
";
    }

    /// <summary>
    /// Runs the provided Python script and captures its output.
    /// </summary>
    /// <param name="pythonScript">The Python script to execute.</param>
    /// <returns>The output from the Python script.</returns>
    private string RunPythonScriptAndCaptureOutput(string pythonScript)
    {
        // Redirect the standard output of the Python engine
        using (StringWriter stringWriter = new StringWriter())
        {
            using (Py.GIL())
            {
                dynamic sys = Py.Import("sys");
                sys.stdout = new CustomTextWriter(stringWriter);
                PythonRunner.RunString(pythonScript);
            }

            // Get the output from the StringWriter
            string output = stringWriter.ToString();
            output = output.Substring(1, output.Length - 3);
            return output;
        }
    }
}