using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Unity.Sentis;
using UnityEngine;

public class TextToSpeech : MonoBehaviour
{
    [Header("Model Settings")]
    [SerializeField] private ModelAsset modelAsset;
    [SerializeField] private BackendType backendType;

    [Header("Input")]
    [SerializeField] private TMP_InputField inputField;

    [Header("Output")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private bool saveToStreamingAssets = true;

    private TokenizerRunner tokenizerRunner;

    private void Awake()
    {
        InitializeComponents();
    }

    /// <summary>
    /// Initializes necessary components on awake.
    /// </summary>
    private void InitializeComponents()
    {
        tokenizerRunner = new TokenizerRunner();
        inputField.onEndEdit.AddListener(delegate { ConvertTextToSpeech(); });
    }

    /// <summary>
    /// Converts text from the input field into speech, plays it, and optionally saves it.
    /// </summary>
    private void ConvertTextToSpeech()
    {
        if (string.IsNullOrWhiteSpace(inputField.text)) return;

        var model = ModelLoader.Load(modelAsset);
        RemoveLayersAfterLayer(model, "/generator/generator/output_conv/output_conv.2/Tanh_output_0");

        var inputValues = TokenizeInput(inputField.text);
        if (inputValues == null) return;

        var audioClip = GenerateSpeech(model, inputValues);
        if (audioClip == null) return;

        PlayAudioClip(audioClip);
        if (saveToStreamingAssets) SaveToStreamingAssets(audioClip);
    }

    /// <summary>
    /// Tokenizes the input text and converts it to an array of integers.
    /// </summary>
    /// <param name="inputText">The text to tokenize.</param>
    /// <returns>An array of integer tokens.</returns>
    private int[] TokenizeInput(string inputText)
    {
        var tokenizedOutput = tokenizerRunner.ExecuteTokenizer(inputText).Split(' ').Where(token => !string.IsNullOrEmpty(token)).ToList();
        return tokenizedOutput.Select(int.Parse).ToArray();
    }

    /// <summary>
    /// Generates speech from tokenized input using the specified model.
    /// </summary>
    /// <param name="model">The loaded model for generating speech.</param>
    /// <param name="inputValues">The tokenized input values.</param>
    /// <returns>An AudioClip containing the generated speech.</returns>
    private AudioClip GenerateSpeech(Model model, int[] inputValues)
    {
        using var input = new TensorInt(new TensorShape(inputValues.Length), inputValues);
        using var engine = WorkerFactory.CreateWorker(backendType, model);
        engine.SetInput("text", input);
        engine.Execute();
        var output = engine.PeekOutput() as TensorFloat;
        return output != null ? ConvertToAudioClip(output) : null;
    }

    /// <summary>
    /// Plays the provided AudioClip.
    /// </summary>
    /// <param name="audioClip">The AudioClip to play.</param>
    private void PlayAudioClip(AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    /// <summary>
    /// Removes model layers after a specified layer.
    /// </summary>
    /// <param name="model">The model to modify.</param>
    /// <param name="layerName">The name of the layer after which to remove all layers.</param>
    private void RemoveLayersAfterLayer(Model model, string layerName)
    {
        int index = model.layers.FindIndex(layer => layer.name == layerName);
        if (index != -1)
        {
            model.layers = model.layers.GetRange(0, index + 1);
            model.outputs = new List<string> { layerName };
        }
        else Debug.LogError("Layer not found.");
    }

    /// <summary>
    /// Converts a TensorFloat to an AudioClip.
    /// </summary>
    /// <param name="output">The TensorFloat containing the audio data.</param>
    /// <returns>An AudioClip generated from the TensorFloat data.</returns>
    private AudioClip ConvertToAudioClip(TensorFloat output)
    {
        output.MakeReadable();
        float[] audioData = output.ToReadOnlyArray();
        int sampleRate = 22050; // Sample rate may vary depending on the model
        AudioClip audioClip = AudioClip.Create("TTSOutput", audioData.Length, 1, sampleRate, false);
        audioClip.SetData(audioData, 0);
        return audioClip;
    }

    /// <summary>
    /// Saves the AudioClip to the StreamingAssets folder.
    /// </summary>
    /// <param name="audioClip">The AudioClip to save.</param>
    private void SaveToStreamingAssets(AudioClip audioClip)
    {
        string outputPath = Path.Combine(Application.streamingAssetsPath, "output.wav");
        EnsureDirectoryExists(Application.streamingAssetsPath);
        SaveAudioClipToWav(audioClip, outputPath);
    }

    /// <summary>
    /// Ensures that a directory exists, creating it if it does not.
    /// </summary>
    /// <param name="path">The directory path to check and potentially create.</param>
    private void EnsureDirectoryExists(string path)
    {
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
    }

    /// <summary>
    /// Saves an AudioClip as a WAV file at the specified path.
    /// </summary>
    /// <param name="clip">The AudioClip to save.</param>
    /// <param name="filePath">The file path to save the WAV file.</param>
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