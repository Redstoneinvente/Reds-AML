using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedsCheckBoxGroupManager : MonoBehaviour
{
    [SerializeField] List<RedsMaterialYouCheckBox> redsMaterialYouCheckBoxes;

    public void AddCheckBoxToGroup(RedsMaterialYouCheckBox checkBox)
    {
        if (redsMaterialYouCheckBoxes.Contains(checkBox))
        {
            return;
        }

        redsMaterialYouCheckBoxes.Add(checkBox);
        checkBox.onToggled += ToggleCheck;
    }

    public void ToggleCheck(RedsMaterialYouCheckBox checkbox, bool value)
    {
        if (!value)
        {
            return;
        }

        foreach (var item in redsMaterialYouCheckBoxes)
        {
            item.Toggle(checkbox == item, true);
        }
    }

    public RedsMaterialYouCheckBox GetChecked()
    {
        foreach (var item in redsMaterialYouCheckBoxes)
        {
            if (item.isChecked)
            {
                return item;
            }
        }

        return default;
    }

    public List<RedsMaterialYouCheckBox> GetCheckBoxes()
    {
        return redsMaterialYouCheckBoxes;
    }
}
