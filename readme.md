# Text to speech implementation for Unity 3D

- [Overview](#overview)
	- [Model](#model)
	- [Tokenizer](#tokenizer)
- [How to run ljspeech-jets-onnx in Unity 3D with Sentis](#how-to-run-ljspeech-jets-onnx-in-unity-3d-with-sentis)
- [The Story](#the-story)
	- [Muse Chat history of importing ljspeech-jets-onnx into Unity 3D](#muse-chat-history-of-importing-ljspeech-jets-onnx-into-unity-3d)

## Overview
This repo contains a text to speech implementation for Unity 3D. Because it's using the python.scripting package, it's not possible to make a build. If you run it in the editor, it will output the audio and put an output.wav file into the StreamingAssets folder. See a sample output here [here](/Assets/StreamingAssets/output.wav).

### Model
I'm using the Unity Sentis Package together with ljspeech-jets-onnx. The ONNX model is not included in this repo and has to be downloaded separately. You can download it from Hugging Face [ljspeech-jets-onnx](https://huggingface.co/NeuML/ljspeech-jets-onnx/tree/main).

### Tokenizer
The [tokenization](https://github.com/neuml/ttstokenizer) is done with the help of the [python.scripting package](https://docs.unity3d.com/Packages/com.unity.scripting.python@7.0/manual/index.html). Note: Note: The package can only be used in the Editor. To perform tokenization into phonemes without Python, have a look at [this post](https://discussions.unity.com/t/model-didnt-import-ljspeech-jets-onnx/265609/29).

![Alt text](docs/TTS-scene.png)

## How to run ljspeech-jets-onnx in Unity 3D with Sentis

1. You need to download the ONNX file from https://huggingface.co/NeuML/ljspeech-jets-onnx/tree/main. The model.onnx Assets/[TTS]/Data Models/ljspeech-jets-onnx/model.onnx not part of the repository and ignored via the .gitignore to save LFS space. 
2. You might need to reimport the model, just right click the model asset and reimport it. (Explanation https://discussions.unity.com/t/binarizer-sample-add-a-custom-layer-only-needs-a-reimport/279200).
3. The import will still display an error, but it can be disregarded. We will be removing the final layers. ![Alt text](docs/import-error.jpg)
4. You can execute the scene "TTS Test", which will produce Assets\StreamingAssets\output.wav for the string "Hello World! I wish I could speak."

## The Story
This was my first attempt at using Sentis over a weekend, and I had no prior experience working with AI or ML code.

I spent several hours searching for other ONNX models without the "If" operator but didn't find any. I then got more help at https://discussions.unity.com/t/model-didnt-import-ljspeech-jets-onnx/265609/13.

As pointed out in the forum, modifying the ONNX outside of Unity might have been faster, especially with the assistance of Chat GPT-4 providing me with step-by-step guidance. What wasn't feasible was using the code interpreter, as it lacked access to the ONNX library.

Another option would have been to learn how to create an ONNX myself; however, I did not want to take that route this time.

A highly beneficial tool was https://netron.app/. It's a handy tool to understand what the ONNX is doing. Here is a screenshot of the sections with the "If" operator as well as the input and outputs of ljspeech-jets-onnx:
![Alt text](docs/original-model-inpout-output-1.jpg)

### Muse Chat history of importing ljspeech-jets-onnx into Unity 3D
I posted some questions that I could have answered myself, but I thought it would be fun to see where the conversation led.

- You can find a text copy of the conversation at [Copy Of Caht](<docs/Text-Copy-Of Chat.pdf>)
- And here are the better readable images

![Alt text](docs/image.png)
![Alt text](docs/image.png)
![Alt text](docs/image-1.png)
![Alt text](docs/image-2.png)
![Alt text](docs/image-3.png)
![Alt text](docs/image-4.png)
![Alt text](docs/image-5.png)
![Alt text](docs/image-6.png)
![Alt text](docs/image-7.png)
![Alt text](docs/image-8.png)
![Alt text](docs/image-9.png)
![Alt text](docs/image-10.png)
