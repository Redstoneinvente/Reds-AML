using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaterialYouInput : MonoBehaviour
{
    [SerializeField] string focusedTrigger;
    [SerializeField] string unFocusedTrigger;
    [SerializeField] string focusLong = "FocusLong";
    [SerializeField] bool useLong;
    [SerializeField] string isEmpty;

    [SerializeField] Animator animation;

    [SerializeField] bool useTwining;
    [SerializeField] Image outlineBreak;
    [SerializeField] TMPro.TMP_Text placeholderText;

    Vector2 outlineBreakSizeStart;
    [SerializeField] Vector2 outlineBreakSizeEnd;

    Vector2 placeholderSizeStart;
    [SerializeField] Vector2 placeholderSizeEnd;

    [SerializeField] float waitRate;
    [SerializeField] float decrement;

    [SerializeField] bool noAnimation;

    bool selected;
    bool error;

    private void Awake()
    {
        if (noAnimation)
        {
            return;
        }

        if (useTwining)
        {
            outlineBreakSizeStart = outlineBreak.rectTransform.anchoredPosition;
            placeholderSizeStart = placeholderText.rectTransform.position;
        }

        if (useLong)
        {
            animation.SetBool(focusLong, true);
        }
    }

    public void EndEdit(string value)
    {
        if (noAnimation)
        {
            return;
        }

        if (useTwining)
        {
            error = value == string.Empty;
        }
        else
        {
            animation.SetBool(isEmpty, value == string.Empty);
        }
    }

    [ContextMenu("DeSelect")]
    public void OnDeselect()
    {
        if (noAnimation)
        {
            return;
        }

        if (useTwining)
        {
            selected = false;
        }
        else
        {
            animation.SetBool(unFocusedTrigger, false);
        }
    }

    [ContextMenu("Select")]
    public void OnSelect()
    {
        if (noAnimation)
        {
            return;
        }

        if (useTwining)
        {
            selected = true;
            StartCoroutine(AnimateSelect());
        }
        else
        {
            animation.SetBool(focusedTrigger, true);
        }
    }

    IEnumerator AnimateSelect()
    {
        if (!useTwining)
        {
            StopAllCoroutines();
        }

        yield return new WaitUntil(() => selected);

        bool checkGreater = placeholderSizeStart.y > placeholderSizeEnd.y;

        Rect r = placeholderText.rectTransform.rect;
        r.position = placeholderSizeEnd;

        while (Check(checkGreater, placeholderText.rectTransform.anchoredPosition.y, placeholderSizeEnd.y))
        {
            if (placeholderText.rectTransform.anchoredPosition.y > placeholderSizeEnd.y)
            {
                placeholderText.rectTransform.anchoredPosition += new Vector2(decrement, decrement) * (checkGreater ? -1 : 1);
            }
            else
            {
                placeholderText.rectTransform.anchoredPosition = placeholderSizeEnd;
            }

            if (outlineBreak.rectTransform.anchoredPosition.y > outlineBreakSizeEnd.y)
            {
                outlineBreak.rectTransform.anchoredPosition += new Vector2(decrement, decrement);
            }
            else
            {
                outlineBreak.rectTransform.anchoredPosition = outlineBreakSizeEnd;
            }

            yield return new WaitForSeconds(waitRate);
        }

        yield return new WaitUntil(() => !selected);

        checkGreater = placeholderSizeStart.y < placeholderSizeEnd.y;

        while (Check(checkGreater, placeholderText.rectTransform.anchoredPosition.y, placeholderSizeEnd.y))
        {
            if (placeholderText.rectTransform.anchoredPosition.y < placeholderSizeStart.y)
            {
                placeholderText.rectTransform.anchoredPosition += new Vector2(decrement, decrement);
            }
            else
            {
                placeholderText.rectTransform.anchoredPosition = placeholderSizeStart;
            }

            if (outlineBreak.rectTransform.anchoredPosition.y < outlineBreakSizeStart.y)
            {
                outlineBreak.rectTransform.anchoredPosition += new Vector2(decrement, decrement) * (checkGreater ? -1 : 1);
            }
            else
            {
                outlineBreak.rectTransform.anchoredPosition = outlineBreakSizeStart;
            }

            yield return new WaitForSeconds(waitRate);
        }
    }

    public bool Check(bool checkGreater)
    {
        if (checkGreater)
        {
            return placeholderText.rectTransform.sizeDelta.magnitude > placeholderSizeEnd.magnitude || outlineBreak.rectTransform.sizeDelta.magnitude > outlineBreakSizeEnd.magnitude;
        }
        else
        {
            return placeholderText.rectTransform.sizeDelta.magnitude < placeholderSizeEnd.magnitude || outlineBreak.rectTransform.sizeDelta.magnitude < outlineBreakSizeEnd.magnitude;
        }
    }

    public bool Check(bool checkGreater, float float1, float float2)
    {
        if (checkGreater)
        {
            return float1 > float2;
        }
        else
        {
            return float1 < float2;
        }
    }
}
