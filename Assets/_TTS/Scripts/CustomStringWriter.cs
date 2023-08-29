using System.IO;
using System.Text;

public class CustomTextWriter : TextWriter
{
    private StringWriter _stringWriter;

    public CustomTextWriter(StringWriter stringWriter)
    {
        _stringWriter = stringWriter;
    }

    public override Encoding Encoding => _stringWriter.Encoding;

    public override void Write(char value)
    {
        _stringWriter.Write(value);
    }

    public override void WriteLine(string value)
    {
        _stringWriter.WriteLine(value);
    }

    // Implement the write method for the Python runtime
    public void write(string value)
    {
        _stringWriter.Write(value);
    }
}