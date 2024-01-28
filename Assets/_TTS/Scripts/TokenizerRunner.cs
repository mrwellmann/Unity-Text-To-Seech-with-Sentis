using Python.Runtime;
using System.IO;
using UnityEditor.Scripting.Python;
using UnityEngine;

public class TokenizerRunner
{
    private string pythonScriptsPath = Application.dataPath + "/_TTS/PythonScripts/";

    public string ExecuteTokenizer(string inputText)
    {
        Debug.Log(pythonScriptsPath);

        PythonRunner.EnsureInitialized();

        string pythonScript = $@"
import sys
sys.path.append('{pythonScriptsPath}')
import yaml
import tokenizer
from tokenizer import TTSTokenizer

with open('{pythonScriptsPath}/config.yaml', 'r', encoding='utf-8') as f:
    config = yaml.safe_load(f)

tokenizer = TTSTokenizer(config['token']['list'])
tokenized = tokenizer('{inputText}')
print(tokenized)
";

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