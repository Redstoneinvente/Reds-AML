using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GraphManager : MonoBehaviour
{
    public GraphObject graphObject;

    [SerializeField] TMP_Text title;
    [SerializeField] List<RectTransform> bars;
    [SerializeField] List<TMP_Text> barsText;

    public List<string> showOnDirectory;

    [SerializeField] Canvas thisCanvas;

    public List<float> values;

    void Awake()
    {
        CreateGraphObject();

        title.text = graphObject.title;
        ShowValues();
    }

    [ContextMenu("Show Values")]
    public void ShowValues()
    {
        for (int i = 0; i < bars.Count; i++)
        {
            graphObject.DisplayValue(bars[i], barsText[i], i);
        }
    }

    public void ShowValues(GraphObject graphObject)
    {
        for (int i = 0; i < bars.Count; i++)
        {
            graphObject.DisplayValue(bars[i], barsText[i], i);
        }
    }

    public GraphObject CreateGraphObject()
    {
        GraphObject graph = new GraphObject(graphObject, new GraphData(values));

        return graph;
    }
}

[System.Serializable]
public class GraphObject
{
    public string title;
    public List<float> values;

    public float minValue;
    public float maxValue;

    public Vector2 size;
    public float sizeSpacing;

    public string valueSffix;
 
    public void DisplayValue(RectTransform bar, TMP_Text disp, int valueIndex)
    {
        float val = values[valueIndex];

        Vector2 s = bar.sizeDelta;

        float perc = (val / maxValue);

        s.y = (Mathf.Clamp01(perc) * size.y) - sizeSpacing;

        bar.sizeDelta = s;

        bar.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, s.y);

        disp.text = val + valueSffix;
    }

    public GraphObject(GraphObject template,  GraphData graphData)
    {
        title = template.title;
        values = graphData.values;

        minValue = template.minValue;
        maxValue = template.maxValue;

        size = template.size;
        sizeSpacing = template.sizeSpacing;

        valueSffix = template.valueSffix;
    }
}

[System.Serializable]
public class GraphData
{
    public List<float> values;

    public GraphData(List<float> values)
    {
        this.values = values;
    }
}
