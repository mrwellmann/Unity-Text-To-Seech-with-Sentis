---
tags:
- audio
- text-to-speech
- onnx
inference: false
language: en
datasets:
- ljspeech
license: apache-2.0
library_name: txtai
---

# ESPnet JETS Text-to-Speech (TTS) Model for ONNX

[imdanboy/jets](https://huggingface.co/imdanboy/jets) exported to ONNX. This model is an ONNX export using the [espnet_onnx](https://github.com/espnet/espnet_onnx) library.

## Usage with txtai

[txtai](https://github.com/neuml/txtai) has a built in Text to Speech (TTS) pipeline that makes using this model easy.

```python
import soundfile as sf

from txtai.pipeline import TextToSpeech

# Build pipeline
tts = TextToSpeech("NeuML/ljspeech-jets-onnx")

# Generate speech
speech = tts("Say something here")

# Write to file
sf.write("out.wav", speech, 22050)
```

## Usage with ONNX

This model can also be run directly with ONNX provided the input text is tokenized. Tokenization can be done with [ttstokenizer](https://github.com/neuml/ttstokenizer).

Note that the txtai pipeline has additional functionality such as batching large inputs together that would need to be duplicated with this method.

```python
import onnxruntime
import soundfile as sf
import yaml

from ttstokenizer import TTSTokenizer

# This example assumes the files have been downloaded locally
with open("ljspeech-jets-onnx/config.yaml", "r", encoding="utf-8") as f:
    config = yaml.safe_load(f)

# Create model
model = onnxruntime.InferenceSession(
    "ljspeech-jets-onnx/model.onnx",
    providers=["CPUExecutionProvider"]
)

# Create tokenizer
tokenizer = TTSTokenizer(config["token"]["list"])

# Tokenize inputs
inputs = tokenizer("Say something here")

# Generate speech
outputs = model.run(None, {"text": inputs})

# Write to file
sf.write("out.wav", outputs[0], 22050)
```

## How to export

More information on how to export ESPnet models to ONNX can be [found here](https://github.com/espnet/espnet_onnx#text2speech-inference).
