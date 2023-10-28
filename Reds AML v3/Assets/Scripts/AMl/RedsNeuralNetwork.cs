using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;

[System.Serializable]
public class RedsNeuralNetwork<T>
{
    /// <summary>
    /// How many input nodes
    /// </summary>
    [SerializeField] int inputsCount;

    /// <summary>
    /// Refers to all input passed in the neural network
    /// </summary>
    [SerializeField] List<RedsInputData<T>> inputDatas;

    /// <summary>
    /// How many layers
    /// </summary>
    [SerializeField] List<RedsNeuralLayer> layers;

    public RedsNeuralNetwork(int inputCount, List<RedsInputData<T>> inputDatas, List<RedsNeuralLayer> layers)
    {
        this.inputsCount = inputCount;
        this.inputDatas = inputDatas;
        this.layers = layers;

        CheckArray();
    }

    public void SetInputDatas(List<RedsInputData<T>> inputDatas)
    {
        this.inputDatas = inputDatas; 
    }

    void CheckArray()
    {
        if (inputDatas == default)
        {
            inputDatas = new List<RedsInputData<T>>();
        }

        if (layers == default)
        {
            layers = new List<RedsNeuralLayer>();
        }
    }

    /// <summary>
    /// Sums all weights for all layers
    /// </summary>
    /// <returns></returns>
    public KeyValuePair<double, double> CalculateNetworkWeigth()
    {
        CheckArray();

        List<double> dataPoints = new List<double>();

        foreach (var inp in inputDatas)
        {
            dataPoints.Add(inp.GetNeuralData());
        }

        //double sum = -10000;

        foreach (var layer in layers)
        {
            dataPoints = layer.GetLayerWeight(dataPoints);
        }

        //foreach (var dataPt in dataPoints)
        //{
        //    sum = Math.Max(sum, dataPt);
        //}

        return new KeyValuePair<double, double>(dataPoints[0], dataPoints[1]);
    }

    public List<RedsNeuralLayer> GetLayers()
    {
        return layers;
    }

    public double GetCost(List<List<RedsInputData<T>>> allInputs)
    {
        CheckArray();

        double cost = 0;

        foreach (var input in allInputs)
        {
            foreach (var inputData in input)
            {
                List<double> dataPoints = new List<double>();

                foreach (var inp in inputDatas)
                {
                    dataPoints.Add(inp.GetNeuralData());
                }

                double sum = 0;

                foreach (var layer in layers)
                {
                    dataPoints = layer.GetLayerWeight(dataPoints);
                }

                foreach (var dataPt in dataPoints)
                {
                    sum = Math.Max(sum, dataPt);
                }

                cost += CalculateCost(sum, inputData.expectedOutput);
            }
        }

        return cost;
    }

    public double CalculateCost(double weight, double expected)
    {
        double diff = expected - weight;
        return diff * diff;
    }
}

/// <summary>
/// Conatins 1 input data or node
/// </summary>
[System.Serializable]
public class RedsInputData<T>
{
    /// <summary>
    /// The input data;
    /// </summary>
    [SerializeField] T inputData;

    public double expectedOutput;

    /// <summary>
    /// How to convet the data to double
    /// </summary>
    Action<T, RedsInputData<T>> processAction;

    /// <summary>
    /// The data that the network understands
    /// </summary>
    [SerializeField] double processedData = double.NegativeInfinity;

    public RedsInputData(T inputData,double expectedOutput, Action<T, RedsInputData<T>> processAction)
    {
        this.expectedOutput = expectedOutput;
        this.inputData = inputData;
        this.processAction = processAction;

        processAction(inputData, this);
    }

    public RedsInputData(T inputData, Action<T, RedsInputData<T>> processAction)
    {
        this.expectedOutput = double.NegativeInfinity;
        this.inputData = inputData;
        this.processAction = processAction;

        processAction(inputData, this);
    }

    public void SetProcessedData(double value)
    {
        this.processedData = value;
    }

    /// <summary>
    /// Gets the data that the network understands
    /// </summary>
    /// <returns></returns>
    public double GetNeuralData()
    {
        if (processedData == double.NegativeInfinity)
        {
            //Data not processed
            processAction(inputData, this);
        }

        return processedData;
    }

    public T GetStoredData()
    {
        return inputData;
    }
}

/// <summary>
/// Represents 1 layer in the neural network
/// </summary>
[System.Serializable]
public class RedsNeuralLayer
{
    [SerializeField] int amountInputs;
    public List<RedsNode> nodes;
    [Range(-10, 10)]
    public List<double> biasses;

    public RedsNeuralLayer(int amountInputs, double learnRate)
    {
        this.amountInputs = amountInputs;

        if (nodes == default)
        {
            nodes = new List<RedsNode>();
        }

        foreach (var node in nodes)
        {
            node.SetInputCount(amountInputs);
        }

        int biasCount = nodes.Count;

        if (biasses.Count != biasCount)
        {
            biasses = new List<double>(biasCount);
        }
    }

    /// <summary>
    /// Goes through all <see cref="nodes"/>, calls and sums their <see cref="RedsNode.GetNodeWeight()"/>
    /// </summary>
    /// <returns></returns>
    public List<double> GetLayerWeight(List<double> dataPoints)
    {
        CheckArray();

        List<double> sum = new List<double>();

        foreach (var node in nodes)
        {
            node.SetInputCount(amountInputs);

            double thisWeight = 0;

            node.SetInputs(dataPoints);
            thisWeight = node.GetNodeWeight(biasses[nodes.IndexOf(node)]);

            sum.Add(thisWeight);
        }

        return sum;
    }

    /// <summary>
    /// Makes sure all arrays/lists are != default
    /// </summary>
    void CheckArray()
    {
        if (nodes == default)
        {
            nodes = new List<RedsNode>();
        }
    }
}

/// <summary>
/// Represents 1 node in the given layer
/// </summary>
[System.Serializable]
public class RedsNode
{
    int inputs;
    [SerializeField] List<double> inputData;
    [Range(-10, 10)]
    public List<double> weights;

    public void SetInputCount(int input)
    {
        this.inputs = input;
        CheckArray();
    }

    public RedsNode(int inputs)
    {
        this.inputs = inputs;
        CheckArray();
    }

    public RedsNode(List<double> inputData)
    {
        this.inputData = inputData;
    }

    public void SetInputs(List<double> inputData)
    {
        this.inputData = inputData;
    }

    /// <summary>
    /// Processes this node's data
    /// </summary>
    /// <returns></returns>
    public double GetNodeWeight(double bias)
    {
        CheckArray();

        double weightCalc = 0;

        for (int i = 0; i < inputData.Count; i++)
        {
            weightCalc += (inputData[i] * weights[i]);
        }

        //return (weightCalc + bias).Sigmoid();
        return (weightCalc + bias);
    }

    void CheckArray()
    {
        if (weights.Count != inputs)
        {
            weights = new List<double>(inputs);
        }
    }
}

public static class RedsMathf
{
    public static float Sigmoid(this float a)
    {
        return 1 / (1 + Mathf.Exp(-a));
    }

    public static double Sigmoid(this double a)
    {
        return 1 / (1 + Math.Exp(-a));
    }
}
