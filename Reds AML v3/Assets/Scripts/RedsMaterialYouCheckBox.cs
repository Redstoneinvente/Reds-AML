using System.Collections;
using System.Collections.Generic;
//using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class RedsMaterialYouCheckBox : MonoBehaviour
{
    [SerializeField] Image checkbox;

//    [SerializeField]
    //public delegate void OnResultReceived(bool state);
    [SerializeField] Sprite checkedSprite;
    [SerializeField] Sprite unCheckedSprite;
    [SerializeField] Sprite intermediateSprite;

    [SerializeField] ThemeManagerMaterialYou.LayerTypeYou unSelected = ThemeManagerMaterialYou.LayerTypeYou.OnSurface;
    [SerializeField] ThemeManagerMaterialYou.LayerTypeYou selected = ThemeManagerMaterialYou.LayerTypeYou.Primary;
    [SerializeField] ThemeManagerMaterialYou.LayerTypeYou intermediate = ThemeManagerMaterialYou.LayerTypeYou.Primary;

    public IntermediateAction intermediateAction;

    public bool isChecked;
    public bool isIntermediate;

    public bool haveIntermediateAfterChecked;
    public bool setAfterFirst;

    [SerializeField]
    public OnResultReceived onResultReceived;

    [SerializeField]
    public OnResultReceived onIntermediate;

    [SerializeField]
    public OnResultReceived onAfterIntermediate;

    public delegate void OnToggled(RedsMaterialYouCheckBox checkBox, bool value);
    public OnToggled onToggled;

    public void Toggle()
    {
        if (isIntermediate)
        {
            Toggle(intermediateAction == IntermediateAction.SetChecked);
        }
        else
        {
            Toggle(!isChecked);
        }

        if (setAfterFirst && !haveIntermediateAfterChecked)
        {
            SetIntermediateAfter();
        }
    }

    public void ToggleNoCheck()
    {
        if (isIntermediate)
        {
            //isChecked = intermediateAction == IntermediateAction.SetChecked;
            //isIntermediate = false;

            onResultReceived.Invoke(true);

            onAfterIntermediate.Invoke(false);

            isChecked = intermediateAction == IntermediateAction.SetChecked;
            isIntermediate = false;
        }
        else
        {
            isChecked = !isChecked;
            isIntermediate = false;
            onResultReceived.Invoke(isChecked);

            if (onToggled != default)
            {
                onToggled(this, isChecked);
            }

        }
    }

    public void Toggle(bool value, bool silent = false)
    {
        isChecked = value;
        isIntermediate = false;

        checkbox.sprite = isChecked ? checkedSprite : unCheckedSprite;

        checkbox.GetComponent<ThemeTag>().ChangeLayer(!isChecked ? unSelected : selected);

        if (!silent)
        { 
            onResultReceived.Invoke(value);

            if (onToggled != default)
            {
                onToggled(this, value);
            }
        }
    }

    public void SetIntermediateAfter()
    {
        haveIntermediateAfterChecked = true;
    }

    public void SetIntermediate(bool silent = false)
    {
        isChecked = false;
        isIntermediate = true;

        checkbox.sprite = intermediateSprite;

        checkbox.GetComponent<ThemeTag>().ChangeLayer(intermediate);

        if (!silent)
        {
            onIntermediate.Invoke(isChecked);
        }
    }

    [System.Serializable]
    public class OnResultReceived : UnityEvent<bool>
    {

    };

    public enum IntermediateAction
    {
        SetChecked,
        SetUnchecked
    }
}