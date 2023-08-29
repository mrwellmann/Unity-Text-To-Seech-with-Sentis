using UnityEngine;
using UnityEditor;
using UnityEditor.Scripting.Python;
using System.IO;
using Python.Runtime;

public class PythonRunnerExample : MonoBehaviour
{
    [MenuItem("Python/Run Python Example")]
    public static void RunPythonExample()
    {
        int one = 1;
        int two = 2;
        string pythonScript = $"result = {one} + {two}\nprint(result)";

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

            // Display the output in the Unity Console
            Debug.Log(output);
        }
    }
}