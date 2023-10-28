using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class RedsMaterialYouChip : MonoBehaviour
{
    [SerializeField] string chipGroupName = "Chip Group 1";
    [SerializeField] List<RedsMaterialYouChip> linkedChips;

    [SerializeField] Image outlineThemeTag;
    [SerializeField] Image innerThemeTag;
    [SerializeField] TMP_Text labelThemeTag;

    [SerializeField] int thisIndex;

    protected static Dictionary<string, int> indexSelected;

    ThemeManagerMaterialYou.LayerTypeYou outlineUnselected = ThemeManagerMaterialYou.LayerTypeYou.Outline;
    ThemeManagerMaterialYou.LayerTypeYou outlineSelected = ThemeManagerMaterialYou.LayerTypeYou.SecondaryContainer;

    ThemeManagerMaterialYou.LayerTypeYou innerUnselected = ThemeManagerMaterialYou.LayerTypeYou.SurfaceContainerLow;
    ThemeManagerMaterialYou.LayerTypeYou innerSelected = ThemeManagerMaterialYou.LayerTypeYou.SecondaryContainer;

    ThemeManagerMaterialYou.LayerTypeYou labelUnSelected = ThemeManagerMaterialYou.LayerTypeYou.OnSurfaceVariant;
    ThemeManagerMaterialYou.LayerTypeYou labelSelected = ThemeManagerMaterialYou.LayerTypeYou.OnSecondaryContainer;

    public UnityEvent<string> onSelected;

    public UnityEvent<CustomChipData> onSelectedCustomData;

    public CustomChipData customData;

    [Multiline]
    public string textToSend;

    private void Awake()
    {
        if (indexSelected == default)
        {
            indexSelected = new Dictionary<string, int>();  
        }
    }

    public void DeselectAll()
    {
        if (!indexSelected.ContainsKey(chipGroupName))
        {
            indexSelected.Add(chipGroupName, -1);
        }
        else
        {
            if (indexSelected[chipGroupName] != -1)
            {
                linkedChips[indexSelected[chipGroupName]].Deselect();
            }
        }
    }

    public void DeselectAllNoMatch(string match)
    {
        DeselectAll();

        foreach (var item in linkedChips)
        {
            if (item.labelThemeTag.text.ToLower() == match.ToLower())
            {
                item.Select();
                return;
            }
        }
    }

    public void Select() 
    {
        if (!indexSelected.ContainsKey(chipGroupName))
        {
            indexSelected.Add(chipGroupName, -1);
        }
        
        if (indexSelected[chipGroupName] != -1)
        {
            if (indexSelected[chipGroupName] == thisIndex)
            {
                if (onSelected != null)
                {
                    onSelected.Invoke("");
                }

                if (onSelectedCustomData != null)
                {
                    onSelectedCustomData.Invoke(default);
                }

                linkedChips[indexSelected[chipGroupName]].Deselect();
                return;
            }
            else
            {
                linkedChips[indexSelected[chipGroupName]].Deselect();
            }
        }

        indexSelected[chipGroupName] = thisIndex;

        Debug.Log("Selected");

        ChangeColorState(true);

        if (onSelected != null)
        {
            onSelected.Invoke(textToSend.Length <= 0 ? labelThemeTag.text : textToSend);
        }

        if (onSelectedCustomData != null)
        {
            onSelectedCustomData.Invoke(customData);
        }
    }

    public void Initialize(string name)
    {
        labelThemeTag.text = name;
    }

    public void Initialize(string name, CustomChipData customData)
    {
        labelThemeTag.text = name;
        this.customData = customData;
    }

    public void Deselect()
    {
        if (!indexSelected.ContainsKey(chipGroupName))
        {
            indexSelected.Add(chipGroupName, -1);
        }

        indexSelected[chipGroupName] = thisIndex == indexSelected[chipGroupName] ? -1 : indexSelected[chipGroupName];

        ChangeColorState(false);
    }

    void ChangeColorState(bool selected)
    {
        outlineThemeTag.color = selected ? GetColor(outlineSelected) : GetColor(outlineUnselected);

        innerThemeTag.color = selected ? GetColor(innerSelected) : GetColor(innerUnselected);
        labelThemeTag.color = selected ? GetColor(labelSelected) : GetColor(labelUnSelected);
    }

    Color GetColor(ThemeManagerMaterialYou.LayerTypeYou layerType)
    {
        return ThemeManagerMaterialYou.matYouThemeManager.GetColor(layerType);
    }

    [ContextMenu("Get References")]
    public void GetReferences()
    {
        if (this.outlineThemeTag == default)
        {
            this.outlineThemeTag = GetComponent<Image>();

            this.innerThemeTag = transform.GetChild(0).GetComponent<Image>();
            this.labelThemeTag = transform.GetChild(1).GetComponent<TMP_Text>();
        }

        thisIndex = linkedChips.IndexOf(this);
    }

    public void AddLink(RedsMaterialYouChip chip)
    {
        chip.linkedChips.Add(this);
        linkedChips.Add(chip);

        chip.GetReferences();
        GetReferences();
    }

    public void AddLink(List<RedsMaterialYouChip> chip)
    {
        linkedChips = new List<RedsMaterialYouChip>();
        linkedChips.AddRange(chip);
        GetReferences();
    }
}

[System.Serializable]
public class CustomChipData
{
    public virtual void Initialize(CustomChipData customChipData)
    {
        Debug.LogWarning("Not Implemented!");
    }
}
