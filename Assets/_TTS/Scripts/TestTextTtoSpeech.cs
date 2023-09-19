using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Sentis;
using UnityEngine;

public class TestTextTtoSpeech : MonoBehaviour
{
    [SerializeField]
    private ModelAsset modelAsset;

    [SerializeField]
    private BackendType backendType;

    [SerializeField]
    private string inputText = "Hello World! I wish I could speak.";

    private TokenizerRunner tokenizerRunner;

    private void Awake()
    {
        tokenizerRunner = new TokenizerRunner();
    }

    private void Start()
    {
        // Load model for inference.
        var model = ModelLoader.Load(modelAsset);

        RemoveLayersAfterLayer(model, "/generator/generator/output_conv/output_conv.2/Tanh_output_0");

        // Convert input text to tensor.
        var tokenizedOutput = tokenizerRunner.ExecuteTokenizer(inputText);
        string[] tokens = tokenizedOutput.Split(' ');
        for (int i = 0; i < tokens.Length; i++)
        {
            if (tokens[i] == "")
            {
                tokens[i] = "0";
            };
        }
        int[] inputValues = tokens.Select(int.Parse).ToArray();
        var inputShape = new TensorShape(inputValues.Length);
        using var input = new TensorInt(inputShape, inputValues);

        // Setup engine of given worker type and model.
        using var engine = WorkerFactory.CreateWorker(backendType, model);
        engine.SetInput("text", input);
        engine.Execute();

        // Get output and cast to the appropriate tensor type (e.g. TensorFloat).
        var output = engine.PeekOutput() as TensorFloat;

        AudioClip audioClip = CovertToAudioClip(output);
        SaveToStreamingAssets(audioClip);

        Debug.Log("Success!");
    }

    private void RemoveLayersAfterLayer(Model model, string layerName)
    {
        int index = model.layers.FindIndex(layer => layer.name == layerName);
        if (index != -1)
        {
            var newLayers = model.layers.GetRange(0, index + 1);
            model.layers = newLayers;

            // Set the output of the model to the output of the last layer we want to keep.
            model.outputs = new List<string> { layerName };

            Debug.Log("Layers Removed!");
        }
        else
        {
            Debug.LogError("Layer not found.");
        }
    }

    private AudioClip CovertToAudioClip(TensorFloat output)
    {
        output.MakeReadable();

        // Convert TensorFloat to AudioClip and save as WAV file.
        float[] audioData = output.ToReadOnlyArray();
        // Set the sample rate according to your Text To Speech model.
        int sampleRate = 22050;
        AudioClip audioClip = AudioClip.Create("TTSOutput", audioData.Length, 1, sampleRate, false);
        audioClip.SetData(audioData, 0);
        return audioClip;
    }

    private void SaveToStreamingAssets(AudioClip audioClip)
    {
        // Ensure the StreamingAssets folder exists.
        string outputFolder = Application.streamingAssetsPath;
        if (!Directory.Exists(outputFolder))
        {
            Directory.CreateDirectory(outputFolder);
        }

        SaveAudioClipToWav(audioClip, Path.Combine(Application.streamingAssetsPath, "output.wav"));
    }

    private void SaveAudioClipToWav(AudioClip clip, string filePath)
    {
        using var fileStream = new FileStream(filePath, FileMode.Create);
        using var writer = new BinaryWriter(fileStream);

        // Write WAV header.
        writer.Write("RIFF".ToCharArray());
        writer.Write(36 + clip.samples * 2);
        writer.Write("WAVE".ToCharArray());
        writer.Write("fmt ".ToCharArray());
        writer.Write(16);
        writer.Write((short)1);
        writer.Write((short)clip.channels);
        writer.Write(clip.frequency);
        writer.Write(clip.frequency * clip.channels * 2);
        writer.Write((short)(clip.channels * 2));
        writer.Write((short)16);
        writer.Write("data".ToCharArray());
        writer.Write(clip.samples * clip.channels * 2);

        // Write audio data.
        float[] samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);
        for (int i = 0; i < samples.Length; i++)
        {
            writer.Write((short)(samples[i] * short.MaxValue));
        }
    }
}