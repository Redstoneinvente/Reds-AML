using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using LayerTypeYou = ThemeManagerMaterialYou.LayerTypeYou;

public class MaterialYouInputField : MonoBehaviour
{
    [SerializeField] Image outlineImage;
    public LayerTypeYou layerTypeYouNormal;
    public LayerTypeYou layerTypeYouSelected;
    public LayerTypeYou layerTypeYouError;

    [SerializeField] TMP_InputField inputField;
    [SerializeField] TMP_Text placeHolderMainText;
    [SerializeField] TMP_Text placeHolderSecondaryText;
    [SerializeField] TMP_Text errorText;

    [SerializeField] float refreshRateOfLerp = 0.1f;

    string placeholderText;
    string supportingText;

    public Color mainPlaceholderColor; 
    public Color normalColor;
    public Color selectedColor;
    public Color errorColor;

    bool wasError;

    public Button continueButton;

    private void Start()
    {
        supportingText = errorText.text;
        placeholderText = placeHolderMainText.text;
        mainPlaceholderColor = placeHolderMainText.color;
    }

    public void ApplyColor(Color nrml, Color selected, Color error)
    {
        normalColor = nrml;
        selectedColor = selected;
        errorColor = error;

        Deselect("");
    }

    public void CheckEmptyInput(string value)
    {
        if (continueButton != default)
        {
            continueButton.interactable = value.Length > 0;
            continueButton.GetComponent<ThemeTag>().Disable(value.Length > 0);
        }

        if (value.Length <= 0)
        {
            SetError("Field Cannot Be Empty!");
        }
        else
        {
            UnSetError();
        }
    }

    public void SetError(string errorMessage)
    {
        wasError = true;

        //placeHolderMainText.color = errorColor;
        //errorText.color = errorColor;
        //outlineImage.color = errorColor;
        if (!this.gameObject.activeInHierarchy)
        {
            return;
        }

        try
        {
            StopAllCoroutines();

            StartCoroutine(LerpColor(placeHolderMainText, errorColor));
            StartCoroutine(LerpColor(errorText, errorColor));
            StartCoroutine(LerpColor(outlineImage, errorColor));
        }
        catch (System.Exception)
        {

        }

        errorText.text = errorMessage;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void UnSetError()
    {
        wasError = false;

        if (!this.gameObject.activeInHierarchy)
        {
            return;
        }

        try
        {
            StopAllCoroutines();

            //placeHolderMainText.color = mainPlaceholderColor;
            //outlineImage.color = normalColor;

            StartCoroutine(LerpColor(placeHolderMainText, normalColor));
            StartCoroutine(LerpColor(outlineImage, normalColor));
            StartCoroutine(LerpColor(errorText, normalColor));
        }
        catch (System.Exception)
        {

        }

        errorText.text = supportingText;
    }

    public void Select(string value)
    {
        //placeHolderMainText.color = selectedColor;

        //inputField.caretColor = selectedColor;

        //outlineImage.color = selectedColor;
        if (!this.gameObject.activeInHierarchy)
        {
            return;
        }

        try
        {
            StopAllCoroutines();

            StartCoroutine(LerpColor(placeHolderMainText, selectedColor));
            StartCoroutine(LerpColor(inputField, selectedColor));
            StartCoroutine(LerpColor(outlineImage, selectedColor));
        }
        catch (System.Exception)
        {

        }
    }

    public void Deselect(string value)
    {
        UnSetError();

        if (!this.gameObject.activeInHierarchy)
        {
            return;
        }

        try
        {
            StopAllCoroutines();

            StartCoroutine(LerpColor(outlineImage, normalColor));
            StartCoroutine(LerpColor(placeHolderMainText, normalColor));

            if (errorText.text == supportingText)
            {
                StartCoroutine(LerpColor(errorText, normalColor));
            }
        }
        catch (System.Exception)
        {

        }

        //placeHolderMainText.color = normalColor;
    }

    IEnumerator LerpColor(Image image, Color color)
    {
        float value = 0;

        while (image.color != color)
        {
            image.color = Color.Lerp(image.color, color, value);

            value += 0.1f;

            yield return new WaitForSeconds(refreshRateOfLerp / 10);
        }
    }

    IEnumerator LerpColor(TMP_Text image, Color color)
    {
        float value = 0;

        while (image.color != color)
        {
            image.color = Color.Lerp(image.color, color, value);

            value += 0.1f;

            yield return new WaitForSeconds(refreshRateOfLerp);
        }
    }

    IEnumerator LerpColor(TMP_InputField image, Color color)
    {
        float value = 0;

        while (image.caretColor != color)
        {
            image.caretColor = Color.Lerp(image.caretColor, color, value);

            value += 0.1f;

            yield return new WaitForSeconds(refreshRateOfLerp);
        }
    }
}
