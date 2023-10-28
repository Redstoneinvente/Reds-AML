using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RedsDynamicUISizing : MonoBehaviour
{
    public Vector2 curentSize;
    public Vector2 curentScreenSize;

    public RectTransform rectTransform;

    private void Awake()
    {
        CheckRect();
        ReSize();
    }

    void CheckRect()
    {
        if (rectTransform == default)
        {
            rectTransform = this.GetComponent<RectTransform>();
        }
    }

    [ContextMenu("Get Current Size")]
    public void GetCurrentAspectRatio()
    {
        CheckRect();
        curentSize = new Vector2(rectTransform.rect.width, rectTransform.rect.height);
        curentScreenSize = new Vector2(Screen.width, Screen.height);
    }

    [ContextMenu("ReSize")]
    public void ReSize()
    {
        Vector2 newScreen = new Vector2(Screen.width, Screen.height);

        if (newScreen == curentScreenSize)
        {
            return;
        }

        Vector2 newSize = (curentSize / curentScreenSize) * newScreen;

        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newSize.x);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newSize.y);
    }
}
