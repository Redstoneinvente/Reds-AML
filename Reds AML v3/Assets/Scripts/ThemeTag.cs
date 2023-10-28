using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

using LayerType = ThemeManager.LayerType;
using LayerTypeYou = ThemeManagerMaterialYou.LayerTypeYou;

public class ThemeTag : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public LayerType thisLayerType;
    public LayerTypeYou thisLayerTypeYou;

    public GameObject primaryOverlay;
    public GameObject stateLayerOverlay;

    [Range(0, 5)]
    public int elevation;

    public Image targetImage;

    public Camera targetCamera;

    public TMP_Text targetText;

    public ThemeTag textOnLayer;

    public LayerType textOnLayerType;

    public LayerTypeYou textOnLayerTypeYou;

    public bool isOnLayer;

    LayerType lastLayerType;
    LayerTypeYou lastTextOnLayerTypeYou;

    public bool triggerUpdateWhenActivated;

    [Header("Buttons and Inputfields Parameters")]
    public bool useDynamicStateLayers;

    public LayerTypeYou onDisabledLayerType;
    [Space]
    public LayerTypeYou onHoverLayerType;
    public State onHoveredState = (State)0;
    [Space]
    public LayerTypeYou onClickLayerType;
    public State onClickedState = (State)2;
    [Space]
    public LayerTypeYou onFocusedLayerType;
    public State onFocusedState = (State)1;

    public void Start()
    {
        if (triggerUpdateWhenActivated)
        {
            ThemeManager.generate = true;
        }
    }

    public void ApplyColor(Color color)
    {
        if (targetImage != default)
        {
            targetImage.color = color;
        }

        if (targetCamera != default)
        {
            targetCamera.backgroundColor = color;
        }

        if (targetText != default)
        {
            targetText.color = color;
        }
    }

    public void ChanegLayer(int index)
    {
        ChangeLayer((LayerTypeYou)index);
    }

    public void ChangeLayer(LayerTypeYou newLayer)
    {
        isOnLayer = false;
        thisLayerTypeYou = newLayer;

        if (primaryOverlay != default)
        {
            Destroy(primaryOverlay);
            primaryOverlay = default;
        }

        if (stateLayerOverlay != default)
        {
            Destroy(stateLayerOverlay);
            stateLayerOverlay = default;
        }

        ApplyColor(ThemeManagerMaterialYou.matYouThemeManager.GetColor(newLayer));

        if (ThemeManagerMaterialYou.matYouThemeManager.RequirePrimaryOverlay(newLayer))
        {
            primaryOverlay = ThemeManagerMaterialYou.matYouThemeManager.CreatePrimaryColorOverlay(transform);
        }
    }

    public void ChangeLayer(LayerTypeYou newLayer, State state)
    {
        isOnLayer = false;

        if (stateLayerOverlay != default)
        {
            Destroy(stateLayerOverlay);
            stateLayerOverlay = default;
        }

        if (primaryOverlay != default)
        {
            Destroy(primaryOverlay);
            primaryOverlay = default;
        }

        //ApplyColor(ThemeManagerMaterialYou.matYouThemeManager.GetColor(newLayer));

        stateLayerOverlay = ThemeManagerMaterialYou.matYouThemeManager.CreateStateLayerOverlay(transform, newLayer, state);
    }

    public void ApplyColor(Color colorOriginal, float opacity)
    {
        Color color = colorOriginal;
        color.a = opacity;

        if (targetImage != default)
        {
            targetImage.color = color;
        }

        if (targetCamera != default)
        {
            targetCamera.backgroundColor = color;
        }

        if (targetText != default)
        {
            targetText.color = color;
        }
    }

    public void CheckUpdate()
    {
        if (lastLayerType != thisLayerType)
        {
            lastLayerType = thisLayerType;

            ThemeManager.generate = true;
        }
    }

    [ContextMenu("Apply")]
    public void UpdateColors()
    {
        ThemeManagerMaterialYou.apply = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("Hovering");

        if (!useDynamicStateLayers)
        {
            return;
        }

        if (!targetImage.GetComponent<Button>().enabled)
        {
            return;
        }

        ChangeLayer(onHoverLayerType, onHoveredState);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!useDynamicStateLayers)
        {
            return;
        }

        if (!targetImage.GetComponent<Button>().enabled)
        {
            return;
        }

        if (stateLayerOverlay != default)
        {
            Destroy(stateLayerOverlay);
            stateLayerOverlay = default;
        }

        ChangeLayer(thisLayerTypeYou);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!useDynamicStateLayers)
        {
            return;
        }

        if (!targetImage.GetComponent<Button>().enabled)
        {
            return;
        }

        ChangeLayer(onClickLayerType, onClickedState);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!useDynamicStateLayers)
        {
            return;
        }

        if (!targetImage.GetComponent<Button>().enabled)
        {
            return;
        }

        if (stateLayerOverlay != default)
        {
            Destroy(stateLayerOverlay);
            stateLayerOverlay = default;
        }

        ChangeLayer(thisLayerTypeYou);
    }

    public void Disable(bool enable = false)
    {
        if (!useDynamicStateLayers)
        {
            return;
        }

        if (enable)
        {
            Enable();
            return;
        }

        if (stateLayerOverlay != default)
        {
            Destroy(stateLayerOverlay);
            stateLayerOverlay = default;
        }

        ChangeLayer(onDisabledLayerType, State.Disable);
    }

    public void Enable()
    {
        if (!useDynamicStateLayers)
        {
            return;
        }

        if (stateLayerOverlay != default)
        {
            Destroy(stateLayerOverlay);
            stateLayerOverlay = default;
        }

        ChangeLayer(thisLayerTypeYou);
    }

    public void OnDisable()
    {
        if (!useDynamicStateLayers)
        {
            return;
        }

        if (stateLayerOverlay != default)
        {
            Destroy(stateLayerOverlay);
            stateLayerOverlay = default;
        }
    }

    public enum State
    {
        Hover = 0,
        Focus = 1,
        Press = 2,
        Drag = 3,
        Disable = 4
    }
}
