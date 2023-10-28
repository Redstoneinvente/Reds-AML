using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarMaterialYou : MonoBehaviour
{
    [SerializeField] Slider indicator;

    public float ratePerSecond;
    public float wait;

    public bool hide;
    public bool definite;
    public bool useIncrements;

    public float sec;

    //private void Start()
    //{
    //    //StartCoroutine(LoadTween());
    //}

    [ContextMenu("Hide")]
    public void Hide()
    {
        hide = true;
    }

    [ContextMenu("Show")]
    public void Show()
    {
        indicator.value = 0;

        this.gameObject.SetActive(true);
        hide = false;
        StartCoroutine(LoadTween());
    }

    public void SetSeconds(float seconds)
    {
        definite = true;
        sec = seconds;
    }

    IEnumerator LoadTween()
    {
        indicator.direction = Slider.Direction.LeftToRight;

        if (definite)
        {
            indicator.maxValue = sec;

            if (!useIncrements)
            {
                for (float i = 0; i < sec; i += ratePerSecond)
                {
                    indicator.value = i;
                    yield return new WaitForSeconds(wait);
                }
            }
        }
        else
        {
            indicator.maxValue = 1;

            for (float i = 0; i < 1; i += ratePerSecond)
            {
                indicator.value = i;
                yield return new WaitForSeconds(wait);
            }

            indicator.direction = Slider.Direction.RightToLeft;

            for (float i = 1; i > 0; i -= ratePerSecond)
            {
                indicator.value = i;
                yield return new WaitForSeconds(wait);
            }
        }

        if (!useIncrements)
        {
            if (!hide && !definite)
            {
                StartCoroutine(LoadTween());
            }
            else
            {
                this.gameObject.SetActive(false);
            }
        }
        else
        {
            indicator.direction = Slider.Direction.LeftToRight;
        }
    }

    public void Increment(float val)
    {
        indicator.direction = Slider.Direction.LeftToRight;

        indicator.value += val;

        if (indicator.value >= indicator.maxValue)
        {
            this.gameObject.SetActive(false);
            Hide();
        }
    }

    public void StopTween()
    {
        StopAllCoroutines();
        Hide();
        this.gameObject.SetActive(false);
    }
}
