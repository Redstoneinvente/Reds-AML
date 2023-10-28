using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PieChart : MonoBehaviour
{
    public float max;
    public float part1;
    public float part2;

    public Image circle;

    public TMP_Text primary;
    public TMP_Text secondary;

    [ContextMenu("Set Part")]
    public void SetPart()
    {
        SetParts(max, part1, part2);
    }

    public void SetParts(float max, float part1, float part2)
    {
        this.max = max;
        this.part1 = part1;
        this.part2 = part2;

        primary.text = "Legal : " + part1 + "";
        secondary.text = "Illegal : " + part2 + "";

        circle.fillAmount = part1 / max;
    }
}
