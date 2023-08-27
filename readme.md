# Creating a TTS system with Unity 3D Sentis

- [Abstract](docs/#abstract)
- [The Story](docs/#the-story)
- [How to run NeuML/ljspeech-jets-onnx into Unity 3D with Sentis](docs/#how-to-run-neumlljspeech-jets-onnx-into-unity-3d-with-sentis)
- [Muse Chat](docs/#muse-chat)
  
## Abstract
This repo contains a partially working TTS import into Unity 3D with Sentis. It will create an output.wav, but it's gibberish. This may be due to missing tokenization of the input. However, i think there is more than this wrong.

## The Story
This is my first attempt at using Sentis over a weekend and I have no prior experience working with AI or ML code.

I've spent several hours searching for other ONNX models without the “If” operator but didn't find any. I then got more help at https://discussions.unity.com/t/model-didnt-import-ljspeech-jets-onnx/265609/13.

As pointed out in the forum, modifying the ONNX outside of Unity might have been faster. Especially with the assistance of Chat GPT-4 providing me with step-by-step guidance. What wasn't feasible was using the code Interpreter as it lacked access to the ONNX library.

Another option would have been to learn how to create an ONNX myself. However, did not want to that route this time.

A highly beneficial tool was https://netron.app/. It's a handy tool to understand what the ONNX is doing. Here is a screenshot of the sections with the “If” operator as well as the input and outputs of ljspeech-jets-onnx:
![Alt text](docs/original-model-inpout-output-1.jpg)


## How to run NeuML/ljspeech-jets-onnx into Unity 3D with Sentis

1. I've added the model.onnx Assets/[TTS]/Data Models/ljspeech-jets-onnx/model.onnx to .gitignore to save LFS space. You need to download the ONNX file from https://huggingface.co/NeuML/ljspeech-jets-onnx/tree/main.
2. You might need to reimport the model using the link: https://discussions.unity.com/t/binarizer-sample-add-a-custom-layer-only-needs-a-reimport/279200.
3. The import will still display an error, but it can be disregarded. We will be removing the final layers. ![Alt text](docs/import-error.jpg)
4. You can execute the scene "TTS Test", which will produce Assets\StreamingAssets\output.wav for the string "Hello World! I wish I could speak."

The generated output.wav will not sound like English. One potential reason is the missing tokenization of the input text, as discussed at https://huggingface.co/NeuML/ljspeech-jets-onnx.

## Muse Chat
I posed questions that I could have answered myself, but I thought it would be fun to see where the conversation led.

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
