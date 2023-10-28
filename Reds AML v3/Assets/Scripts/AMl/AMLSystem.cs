using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.ObjectModel;
using UnityEditor.MemoryProfiler;
using UnityEngine.Windows;
using UnityEditor.Experimental.GraphView;

using TMPro;
using static UnityEditor.Timeline.TimelinePlaybackControls;
using System.Linq;

public class AMLSystem : MonoBehaviour
{
    [SerializeField] public RedsNeuralNetwork<double> neuralNetwork;

    List<RedsUser> users;
    List<RedsTransaction> allTrans;

    /// <summary>
    /// For indexing <see cref="users"/>
    /// </summary>
    Dictionary<string, int> usersToObj;

    [SerializeField] double learnRate = 0.01;

    [SerializeField] int iterations = 10; 

    [SerializeField] TMP_Text costText;

    public RedsTransaction transaction;

    public string csvPath;

    public bool canRun;

    public int packetSize = 1000;
    public int currLine;

    void Awake()
    {
        usersToObj = new Dictionary<string, int>();
        users = new List<RedsUser>();
        allTrans = new List<RedsTransaction>();
    }

	private void Start()
	{
		StartCoroutine(TrainAICor());
	}

    [ContextMenu("Check Trans")]
    public void CallCheckTrans()
    {
        CheckTransaction(transaction);
    }

    /// <summary>
    /// Pass a transaction, flag as illegal/legal
    /// </summary>
    /// <param name="transaction"></param>
	public void CheckTransaction(RedsTransaction transaction)
    {
        PassToNeuralCheck(transaction);

        KeyValuePair<double, double> temp = neuralNetwork.CalculateNetworkWeigth();

        double weight = Math.Max(1 - temp.Key, temp.Value);
        bool isLegal = weight == 1 - temp.Key;
        bool isCorrect = (!isLegal && transaction.flagAsIllegal) || (isLegal && !transaction.flagAsIllegal);

        //Debug.Log($"Transaction legal = {isLegal}, is data correct {isCorrect}, {1 - temp.Key} > {temp.Value}");
        Debug.Log($"Transaction legal = {isLegal}, is data correct {isCorrect}");
        Debug.Log($"is data correct {isCorrect}");
    }

    public KeyValuePair<double, double> GetRawTransactionLegality()
    {
        PassToNeuralCheck(transaction);

        KeyValuePair<double, double> temp = neuralNetwork.CalculateNetworkWeigth();

        return temp;
    }

    [ContextMenu("Train")]
    public void Train()
    {
        StartCoroutine(Feed());
    }

    public void AddToTrain(string line)
    {
        transaction = new KaggleAMLDataSource(line, 1).GetRedsTransaction(ref users, ref usersToObj);
    }

    IEnumerator Feed()
    {
		usersToObj = new Dictionary<string, int>();
		users = new List<RedsUser>();
        int countL = System.IO.File.ReadAllLines(csvPath).Length;
		int i = 0;
        //foreach (var line in System.IO.File.ReadAllLines(csvPath))
        //{
        //	//for each lines, make a transaction
        //	RedsTransaction trans = new KaggleAMLDataSource(line, i).GetRedsTransaction(ref users, ref usersToObj);

        //	allTrans.Add(trans);

        //	i++;
        //	Debug.Log($"{i} out of {countL}");
        //	yield return new WaitForEndOfFrame();
        //}

        List<string> linesAll = new List<string>(System.IO.File.ReadAllLines(csvPath));

        ///Faster Load Times
        while (linesAll.Count > 0)
        {
            foreach (var line in linesAll.Take(packetSize >= linesAll.Count ? linesAll.Count - 1 : packetSize))
            {
                //for each lines, make a transaction
                RedsTransaction trans = new KaggleAMLDataSource(line, i).GetRedsTransaction(ref users, ref usersToObj);

                allTrans.Add(trans);
                currLine++;
                i++;
            }

            linesAll.RemoveRange(0, packetSize >= linesAll.Count ? linesAll.Count - 1 : packetSize);
            linesAll.TrimExcess();
            yield return new WaitForEndOfFrame();
        }

        //Debug.Log($"{i} out of {countL}");

        Debug.Log("Done Feeding!");

		Vector2 legalToIllegal = Vector2.zero;
		double cost = 0;

		foreach (var item in allTrans)
		{
			PassToNeural(item);

            KeyValuePair<double, double> temp = neuralNetwork.CalculateNetworkWeigth();

            double weight = Math.Max(temp.Key, temp.Value);

            bool isLegal = weight == temp.Key;
            bool isCorrect = (!isLegal && item.flagAsIllegal) || (isLegal && !item.flagAsIllegal);

            //cost += neuralNetwork.CalculateCost(weight, item.flagAsIllegal ? 1 : 0);
            //cost += neuralNetwork.CalculateCost(isCorrect ? weight : 0, 1);
            cost += neuralNetwork.CalculateCost(weight,isCorrect ? 1 : 0);

            legalToIllegal = new Vector2(weight < 0.5 ? legalToIllegal.x + 1 : legalToIllegal.x, weight > 0.5 ? legalToIllegal.y + 1 : legalToIllegal.y);
			Debug.Log($"cost {cost}");

			yield return new WaitForEndOfFrame();
		}

        cost /= allTrans.Count;

		Debug.Log($"Done! Legal = {legalToIllegal.x} , illegal = {legalToIllegal.y}");
		//Debug.Log($"Done! Cost = {cost}");
		costText.text = $"cost = {cost}";
		TrainAI();
        canRun = true;
	}

	[ContextMenu("Randomize Weights")]
    public void RandomWeights()
    {
        foreach (var layer in neuralNetwork.GetLayers())
        {
            foreach (var node in layer.nodes)
            {
                for (int i = 0; i < node.weights.Count; i++)
                {
                    node.weights[i] = UnityEngine.Random.Range(0f, 1f);
                }
            }

            for (int i = 0; i < layer.biasses.Count; i++)
            {
                layer.biasses[i] = UnityEngine.Random.Range(-1f, 1f);
            }
        }
    }

    [ContextMenu("Train AI")]
    public void TrainAI()
    {
        //RandomWeights();

        //StartCoroutine(TrainAICor());
    }

    IEnumerator TrainAICor()
    {
        while (true)
        {
            if (canRun)
            {
				double orginalCost = GetNeuralCost();
				double h = 0.0001;

				foreach (var layer in neuralNetwork.GetLayers())
				{
					foreach (var node in layer.nodes)
					{
						for (int i = 0; i < node.weights.Count; i++)
						{
							node.weights[i] -= h;
							double deltaCost = GetNeuralCost() - orginalCost;
							node.weights[i] += h;

							double costGradient = deltaCost / h;

							if (deltaCost < 0)
							{
								double og = node.weights[i];
								double grad = GradientDescent(node.weights[i] - (deltaCost / h) * learnRate);
								node.weights[i] = grad;
								deltaCost = GetNeuralCost();

								if (Math.Min(deltaCost, orginalCost) != deltaCost)
								{
									node.weights[i] = og;
								}
							}
						}

						yield return new WaitForEndOfFrame();
					}

					for (int i = 0; i < layer.biasses.Count; i++)
					{
						layer.biasses[i] -= h;
						double deltaCost = GetNeuralCost() - orginalCost;
						layer.biasses[i] += h;

						double costGradient = deltaCost / h;

						if (deltaCost < 0)
						{
							double og = layer.biasses[i];
							double grad = GradientDescent(layer.biasses[i] - (deltaCost / h) * learnRate);
							layer.biasses[i] = grad;
							deltaCost = GetNeuralCost();

							if (Math.Min(deltaCost, orginalCost) != deltaCost)
							{
								layer.biasses[i] = og;
							}
						}
					}

					yield return new WaitForEndOfFrame();
				}

				yield return new WaitForEndOfFrame();
				costText.text = $"cost = {GetNeuralCost()}";

			}
            else
            {
                CallCheckTrans();
				yield return new WaitForEndOfFrame();
				costText.text = $"cost = {GetNeuralCost()}";
			}

			yield return new WaitForEndOfFrame();
		}
	}

    public double GradientDescent(double weight)
    {
        double result = weight - learnRate * (2 * Math.Sqrt(weight));

        //Debug.Log(result > 1 || result == double.NaN ? weight.Sigmoid() : result);
        return result;
    }

    /// <summary>
    /// Gets the cost for all the data points
    /// </summary>
    /// <returns></returns>
    public double GetNeuralCost()
    {
        double cost = 0;

        //usersToObj = new Dictionary<string, int>();
        //users = new List<RedsUser>();

        int i = 0;
        //foreach (var line in System.IO.File.ReadAllLines(csvPath))
        //{
        //    //for each lines, make a transaction
        //    RedsTransaction trans = new KaggleAMLDataSource(line, i).GetRedsTransaction(ref users, ref usersToObj);

        //    allTrans.Add(trans);

        //    i++;
        //}

        foreach (var item in allTrans)
        {
            PassToNeural(item);

            KeyValuePair<double, double> temp = neuralNetwork.CalculateNetworkWeigth();

            double weight = Math.Max(temp.Key, temp.Value);

            bool isLegal = temp.Key > temp.Value;
            bool isCorrect = (!isLegal && item.flagAsIllegal) || (isLegal && !item.flagAsIllegal);

            //cost += neuralNetwork.CalculateCost(weight, item.flagAsIllegal ? 1 : 0);
            cost += neuralNetwork.CalculateCost(weight, isCorrect ? 1 : 0);
        }

        return cost / allTrans.Count == double.NaN ? (cost / allTrans.Count).Sigmoid() : cost / allTrans.Count;
    }

    /// <summary>
    /// Take 1 data point, passes it through neural network. For training only
    /// </summary>
    /// <param name="item"></param>
    public void PassToNeural(RedsTransaction item)
    {
        Action<double, RedsInputData<double>> processInput = (data, inpData) =>
        {
            inpData.SetProcessedData(data);
        };

        List<RedsInputData<double>> inputs = new List<RedsInputData<double>>();

        inputs.Add(new RedsInputData<double>(item.amnt, item.flagAsIllegal ? 1 : 0, processInput));
        inputs.Add(new RedsInputData<double>(item.amnt - item.sendingUser.averageEarnings, item.flagAsIllegal ? 1 : 0, processInput));
        inputs.Add(new RedsInputData<double>(item.amnt - item.sendingUser.averageSpendings, item.flagAsIllegal ? 1 : 0, processInput));

        inputs.Add(new RedsInputData<double>(item.amnt - item.receivingUser.averageEarnings, item.flagAsIllegal ? 1 : 0, processInput));
        inputs.Add(new RedsInputData<double>(item.amnt - item.receivingUser.averageSpendings, item.flagAsIllegal ? 1 : 0, processInput));
        inputs.Add(new RedsInputData<double>(item.receivingUser.hadIllegals || item.sendingUser.hadIllegals ? 1 : 0, item.flagAsIllegal ? 1 : 0, processInput));

        neuralNetwork.SetInputDatas(inputs);
    }

    /// <summary>
    /// Take 1 data point, passes it through neural network
    /// </summary>
    /// <param name="item"></param>
    public void PassToNeuralCheck(RedsTransaction item)
    {
        Action<double, RedsInputData<double>> processInput = (data, inpData) =>
        {
            inpData.SetProcessedData(data);
        };

        List<RedsInputData<double>> inputs = new List<RedsInputData<double>>();

        inputs.Add(new RedsInputData<double>(item.amnt * item.amnt, processInput));
        inputs.Add(new RedsInputData<double>((item.amnt - item.sendingUser.averageEarnings), processInput));
        inputs.Add(new RedsInputData<double>(item.amnt - item.sendingUser.averageSpendings, processInput));

        inputs.Add(new RedsInputData<double>(item.amnt - item.receivingUser.averageEarnings, processInput));
        inputs.Add(new RedsInputData<double>(item.amnt - item.receivingUser.averageSpendings, processInput));
        inputs.Add(new RedsInputData<double>(item.receivingUser.hadIllegals || item.sendingUser.hadIllegals ? 1 : 0, processInput));

        neuralNetwork.SetInputDatas(inputs);
    }

    public double Slope(double x)
    {
        return 0.2 * Math.Pow(x, 4) + 0.1 * Math.Pow(x, 3) - Math.Pow(x, 2) + 2;
    }
}

[System.Serializable]
public class RedsTransaction
{
    public double amnt;

    public RedsUser sendingUser;
    public RedsUser receivingUser;

    public string sendingCurrency;
    public string receivingCurrency;

    public string transactionID;
    public bool flagAsIllegal;

    public string timeStamp;
    public string transactionReason;

    public RedsTransaction(double amnt, RedsUser sendingUser, RedsUser receivingUser, string currencySend, string currencyRec, string transID, string timeStamp, string reasin, bool isIllegal)
    {
        this.amnt = amnt;
        this.sendingCurrency = currencySend;
        this.receivingCurrency = currencyRec;

        this.sendingUser = sendingUser;
        this.receivingUser = receivingUser;

        this.transactionID = transID;
        this.transactionReason = reasin;

        this.timeStamp = timeStamp;
        this.flagAsIllegal = isIllegal;
    }
}

[System.Serializable]
public class RedsUser
{
    public string name;
    public List<RedsTransaction> pastTransactions;

    public double averageSpendings;
    public double averageEarnings;

    public bool hadIllegals;

    public RedsUser(string name)
    {
        this.name = name;
        pastTransactions = new List<RedsTransaction>();
    }

    public void AddNewTransaction(RedsTransaction redsTransaction)
    {
        hadIllegals = hadIllegals || redsTransaction.flagAsIllegal;

        pastTransactions.Add(redsTransaction);

        if (redsTransaction.sendingUser == this)
        {
            //spend
            averageSpendings = (averageSpendings + redsTransaction.amnt) / 2;
        }
        else
        {
            //earn
            averageEarnings = (averageEarnings + redsTransaction.amnt) / 2;
        }
    }
}

[System.Serializable]
public class KaggleAMLDataSource
{
    public string timestamp;
    public string fromBank;
    public string fromAccount;

    public string toBank;
    public string toAccount;
    public float amntRec;

    public string recCurr;
    public float amntPaid;
    public string paymentCurr;

    public string paymentFormat;

    public bool isLaundering;

    public int id;

    public KaggleAMLDataSource(string thisLine, int id)
    {
        List<string> data = new List<string>(thisLine.Split(","));
        this.timestamp = data[0];
        this.fromBank = data[1];
        this.fromAccount = data[2];

        this.toBank = data[3];
        this.toAccount = data[4];
        this.amntRec = float.Parse(data[5]);

        this.recCurr = data[6];
        this.amntPaid = float.Parse(data[7]);
        this.paymentCurr = data[8];

        this.paymentFormat = data[9];

        this.isLaundering = data[10].Contains("1");

        this.id = id;
    }

    public RedsTransaction GetRedsTransaction(RedsUser userFrom, RedsUser userTo)
    {
        return new RedsTransaction(amntPaid, userFrom, userTo, paymentCurr, recCurr, id + "", timestamp, paymentFormat, isLaundering);
    }

    public RedsTransaction GetRedsTransaction(ref List<RedsUser> userList,ref Dictionary<string, int> indexingList)
    {
        string fromName = fromBank + "," + fromAccount;
        string toName = toBank + "," + toAccount;

        if (!indexingList.ContainsKey(fromName))
        {
            indexingList.Add(fromName, userList.Count);
            userList.Add(new RedsUser(fromName));
        }

        if (!indexingList.ContainsKey(toName))
        {
            indexingList.Add(toName, userList.Count);
            userList.Add(new RedsUser(toName));
        }

        return new RedsTransaction(amntPaid, userList[indexingList[fromName]], userList[indexingList[toName]], paymentCurr, recCurr, id + "", timestamp, paymentFormat, isLaundering);
    }
}
