using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static UnityEngine.UIElements.UxmlAttributeDescription;
using System;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class AMLDataImporter : MonoBehaviour
{
    [SerializeField] int lineToRead;
    [SerializeField] AMLSystem amlSystem;

    [SerializeField] float waitTime;

    [SerializeField] TMP_Text performanceDisplay;
    [SerializeField] string csvFilePath;

    [SerializeField] TMP_InputField fromAcc;
    [SerializeField] TMP_InputField toAcc;
    [SerializeField] TMP_InputField amount;

    [SerializeField] TMP_Text reportText;
    [SerializeField] List<GameObject> report;

    public PieChart pie;

    public RedsUser a;
    public RedsUser b;

    public int count = 0;

    public void Pay()
    {
        bool isIllegal = IsIllegal(new RedsTransaction(double.Parse(amount.text), a, b, "", "", "", "", "", false));

        if (!isIllegal)
        {
            report[count].SetActive(true);
            reportText.text = reportText.text.Replace("[NUM]", amount.text);

            pie.SetParts(pie.max++, pie.part1, pie.part2++);

            count++;
        }
        else
        {
            pie.SetParts(pie.max++, pie.part1++, pie.part2);
        }
    }

    [ContextMenu("Send To AML")]
    public void SendToAML()
    {
        performanceDisplay.text = "Starting...";

        StartCoroutine(SendData());
    }

    void DisplayPerf(Vector2Int aiValues, Vector2Int givenValues, int count)
    {
        double percAccuracy = count / (float)(givenValues.x + givenValues.y);
        percAccuracy *= 100;

        performanceDisplay.text = $"Percentage Performance : {percAccuracy}%, {aiValues.x} ai Legal, {aiValues.y} ai ILLegal, {givenValues.x} given legal, {givenValues.y} given illegal, count {count}";
    }

    IEnumerator SendData()
    {
        while (true)
        {
            Vector2Int countAI = new Vector2Int();
            Vector2Int countGet = new Vector2Int();

            int correct = 0;

            foreach (var line in System.IO.File.ReadAllLines(csvFilePath))
            {
                amlSystem.AddToTrain(line);
                KeyValuePair<double, double> weightPair = amlSystem.GetRawTransactionLegality();

                double weight = Math.Max(1 - weightPair.Key, weightPair.Value);
                bool isLegal = weight == 1 - weightPair.Key;
                bool isCorrect = (!isLegal && amlSystem.transaction.flagAsIllegal) || (isLegal && !amlSystem.transaction.flagAsIllegal);

                countAI.x += isCorrect && isLegal ? 1 : 0;
                countAI.y += isCorrect && !isLegal ? 1 : 0;

                correct += isCorrect ? 1 : 0;

                countGet.x += !amlSystem.transaction.flagAsIllegal ? 1 : 0;
                countGet.y += amlSystem.transaction.flagAsIllegal ? 1 : 0;

                Debug.Log("Done!" + weight);
                yield return new WaitForSeconds(waitTime);
            }

            DisplayPerf(countAI, countGet, correct);

            yield return new WaitForSeconds(waitTime);
        }
    }

    public bool IsIllegal(RedsTransaction redsTransaction)
    {
        amlSystem.transaction = redsTransaction;
        KeyValuePair<double, double> weightPair = amlSystem.GetRawTransactionLegality();

        double weight = Math.Max(1 - weightPair.Key, weightPair.Value);
        bool isLegal = weight == 1 - weightPair.Key;

        Debug.Log("Is Legal!" + isLegal);

        return isLegal;
    }
}
