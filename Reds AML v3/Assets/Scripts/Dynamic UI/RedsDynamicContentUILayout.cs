using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RedsDynamicContentUILayout : MonoBehaviour
{
    [SerializeField] FitType fitType;
    [SerializeField] float aspectRatio;
    [SerializeField] RectTransform rectTransform;

    [SerializeField] RedsDynamicGridLayout redsDynamicGridLayout;

    [Tooltip("Applies only when fitType is set to fill. This allows the content to change aspect ration in the given range")]
    [SerializeField] Vector2 aspectRatioBias;

    [SerializeField] float biasDecrementAccuracyPerc = 0.1f;

    private void Awake()
    {
        CheckRect();
        ChangeSize();
    }

    [ContextMenu("Get Current Aspect Ratio")]
    public void GetCurrentAspectRatio()
    {
        CheckRect();
        aspectRatio = rectTransform.rect.width / rectTransform.rect.height;
    }

    void OnRenderObject()
    {
        if (redsDynamicGridLayout != default)
        {
            return;
        }

        ChangeSize();
    }

    [ContextMenu("Assign Rect")]
    public void AssignRect()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    [ContextMenu("Change Size")]
    void ChangeSize()
    {
        float sizeX = rectTransform.rect.width;
        float sizeY = rectTransform.rect.height;

        if (sizeX / sizeY == aspectRatio)
        {
            return;
        }

        switch (fitType)
        {
            case FitType.NoScaling:
                break;

            case FitType.Horizontal:

                float width = transform.parent.GetComponent<RectTransform>().rect.width;
                float height = width / aspectRatio;

                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

                break;

            case FitType.Vertical:

                height = transform.parent.GetComponent<RectTransform>().rect.height;
                width = height * aspectRatio;

                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

                break;

            case FitType.Auto_Crop:

                RectTransform parentRect = transform.parent.GetComponent<RectTransform>();

                height = parentRect.rect.width / aspectRatio;
                width = parentRect.rect.height * aspectRatio;

                if (height < parentRect.rect.height)
                {
                    //Match height
                    height = parentRect.rect.height;
                }
                else
                {
                    width = parentRect.rect.width;
                }

                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

                break;

            case FitType.Auto_Fit:

                parentRect = transform.parent.GetComponent<RectTransform>();

                height = parentRect.rect.width / aspectRatio;
                width = parentRect.rect.height * aspectRatio;

                if (height >= parentRect.rect.height)
                {
                    //Match height
                    height = parentRect.rect.height;
                }
                else
                {
                    width = parentRect.rect.width;
                }

                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

                break;

            case FitType.Fill:

                height = sizeX / aspectRatio;
                width = sizeY * aspectRatio;

                if (height < sizeY)
                {
                    //Match width
                    width = sizeX;

                    float decr = aspectRatioBias.x * biasDecrementAccuracyPerc;

                    for (float i = aspectRatioBias.x; i >= 0; i -= decr)
                    {
                        height = sizeX / (aspectRatio - i);

                        if (height <= sizeY)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    height = sizeY;

                    float decr = aspectRatioBias.x * biasDecrementAccuracyPerc;

                    for (float i = aspectRatioBias.y; i >= 0; i -= decr)
                    {
                        width = sizeY * (aspectRatio + i);

                        if (width <= sizeX)
                        {
                            break;
                        }
                    }
                }

                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

                break;

            default:
                break;
        }
    }

    public Vector2 GetSize(float slotWidth, float slotHeight)
    {
        float sizeX = slotWidth;
        float sizeY = slotHeight;

        if (sizeX / sizeY == aspectRatio)
        {
            return new Vector2(sizeX, sizeY);
        }

        switch (fitType)
        {
            case FitType.NoScaling:
                break;

            case FitType.Horizontal:

                float width = slotWidth;
                float height = width / aspectRatio;

                return new Vector2(width, height);

                break;

            case FitType.Vertical:

                height = slotHeight;
                width = height * aspectRatio;

                return new Vector2(width, height);

                break;

            case FitType.Auto_Crop:

                height = slotWidth / aspectRatio;
                width = slotHeight * aspectRatio;

                if (height < slotHeight)
                {
                    //Match height
                    height = slotHeight;
                }
                else
                {
                    width = slotWidth;
                }

                return new Vector2(width, height);

                break;

            case FitType.Auto_Fit:

                height = slotWidth / aspectRatio;
                width = slotHeight * aspectRatio;

                if (height < slotHeight)
                {
                    //Match width
                    width = slotWidth;
                }
                else
                {
                    height = slotHeight;
                }

                return new Vector2(width, height);

                break;

            case FitType.Fill:

                height = slotWidth / aspectRatio;
                width = slotHeight * aspectRatio;

                if (height < slotHeight)
                {
                    //Match width
                    width = slotWidth;

                    float decr = aspectRatioBias.x * biasDecrementAccuracyPerc;

                    for (float i = aspectRatioBias.x; i >= 0; i -= decr)
                    {
                        height = slotWidth / (aspectRatio - i);

                        if (height <= slotHeight)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    height = slotHeight;

                    float decr = aspectRatioBias.x * biasDecrementAccuracyPerc;

                    for (float i = aspectRatioBias.y; i >= 0; i -= decr)
                    {
                        width = slotHeight * (aspectRatio + i);

                        if (width <= slotWidth)
                        {
                            break;
                        }
                    }
                }

                return new Vector2(width, height);

                break;

            default:
                break;
        }

        return new Vector2(slotWidth, slotHeight);
    }

    void CheckRect()
    {
        if (rectTransform == default)
        {
            rectTransform = this.GetComponent<RectTransform>();
        }
    }

    enum FitType 
    {
        NoScaling,
        Horizontal,
        Vertical,
        [Tooltip("Matches the longest length")] Auto_Crop,
        [Tooltip("Matches the shortest length")] Auto_Fit,
        Fill
    }
}
