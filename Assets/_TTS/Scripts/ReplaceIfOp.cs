using System;
using System.Linq;
using Unity.Sentis;
using Unity.Sentis.Layers;
using Unity.Sentis.ONNX;

// Define onnx op importer for the If operator.
[OpImport("If")]
public class ReplaceIfOp : IOpImporter
{
    public void Import(Model model, OperatorNode node)
    {
        // Add custom no-op layer to the model with the node name (output) and node inputs.
        model.AddLayer(new IdentityLayer(node.Name, node.Inputs.ToArray()));
    }
}

// Define custom no-op layer.
[Serializable]
public class IdentityLayer : CustomLayer
{
    // Constructor is called from the importer with the output and input names.
    public IdentityLayer(string name, string[] inputs)
    {
        this.name = name;
        this.inputs = inputs;
    }

    // Infer the output data types from the input data types at import time.
    public override DataType[] InferOutputDataTypes(DataType[] inputDataTypes)
    {
        return inputDataTypes;
    }

    // Execute the no-op layer, returning the input tensor(s) without performing any operations.
    public override Tensor Execute(Tensor[] inputTensors, ExecutionContext ctx)
    {
        return inputTensors[0];
    }
}