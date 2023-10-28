using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThemeManager : MonoBehaviour
{
    public Color mainColor;

    public Color lightAccent;
    public Color darkAccent;
    public Color faintAccent;
    public Color whiteAccent;

    public Color textAccentOnLight;
    public Color textAccentOnDark;
    public Color textAccentOnFaint;
    public Color textAccentOnWhite;

    public float lightFactor;
    public float darkFactor;
    public Vector3 faintFactor;
    public Vector3 whiteFactor;

    [Range(0f, 1f)]
    public float darkFactorToleranceForText;

    public bool enableDynamicUIUpdates;
    public bool enableDynamicUIStartUpdate;
    public static bool canDynamicUpdate;

    public static bool generate;

    [ExecuteAlways, ContextMenu("Generate Color")]
    public void GenerateColorPalette()
    {
        lightAccent = ApplyColorValues(lightFactor);
        darkAccent = ApplyColorValues(darkFactor);
        faintAccent = ApplyColorValues(faintFactor);
        whiteAccent = ApplyColorValues(whiteFactor);

        textAccentOnLight = GenerateReadableTextColor(lightAccent);
        textAccentOnDark = GenerateReadableTextColor(darkAccent);
        textAccentOnFaint = GenerateReadableTextColor(faintAccent);
        textAccentOnWhite = GenerateReadableTextColor(whiteAccent);
    }
     
    public void FixedUpdate()
    {
        if (enableDynamicUIStartUpdate)
        {
            if (generate)
            {
                generate = false;

                canDynamicUpdate = true;
                Debug.Log("Dynamic Update");

                List<ThemeTag> allLayers = new List<ThemeTag>(GameObject.FindObjectsOfType<ThemeTag>());

                GenerateAndApply();
            }
        }

        if (enableDynamicUIUpdates)
        {
            canDynamicUpdate = true;
            Debug.Log("Dynamic Update");

            List<ThemeTag> allLayers = new List<ThemeTag>(GameObject.FindObjectsOfType<ThemeTag>());

            foreach (var item in allLayers)
            {
                item.CheckUpdate();
            }

            if (generate)
            {
                generate = false;
                GenerateAndApply();
            }

        }
        else
        {
            canDynamicUpdate = false;
        }
    }

    Color ApplyColorValues(float factor)
    {
        Color color = mainColor;
        color.r = color.r - factor <= 0 ? 0 : color.r - factor;
        color.g = color.g - factor <= 0 ? 0 : color.g - factor;
        color.b = color.b - factor <= 0 ? 0 : color.b - factor;

        return color;
    }

    Color ApplyColorValues(Vector3 factor)
    {
        Color color = mainColor;
        color.r = color.r - factor.x <= 0 ? 0 : color.r - factor.x;
        color.g = color.g - factor.y <= 0 ? 0 : color.g - factor.y;
        color.b = color.b - factor.z <= 0 ? 0 : color.b - factor.z;

        return color;
    }

    Color GenerateReadableTextColor(Color colorOnTop)
    {
        Color color = mainColor;

        float luminancePercentage = CalculateLuminace(colorOnTop);

        if (luminancePercentage > darkFactorToleranceForText)
        {
            //Light Color
            color = ApplyColorValues(0.8f);
        }
        else
        {
            //Dark Color
            color = ApplyColorValues(-0.8f);
        }

        return color;
    }

    public float CalculateLuminace(Color color)
    {
        float r = color.r;
        float g = color.g;
        float b = color.b;

        return (r * 0.299f + g * 0.587f + b * 0.114f) / 1;
    }

    [ExecuteAlways, ContextMenu("Apply Color")]
    public void ApplyColor()
    {
        List<ThemeTag> allLayers = new List<ThemeTag>(GameObject.FindObjectsOfType<ThemeTag>());

        foreach (var item in allLayers)
        {
            if (!item.isOnLayer)
            {
                switch (item.thisLayerType)
                {
                    case LayerType.LightAccent:
                        item.ApplyColor(lightAccent);
                        break;
                    case LayerType.DarkAccent:
                        item.ApplyColor(darkAccent);
                        break;
                    case LayerType.FaintAccent:
                        item.ApplyColor(faintAccent);
                        break;
                    case LayerType.WhiteAccent:
                        item.ApplyColor(whiteAccent);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                if (item.textOnLayer != default)
                {
                    switch (item.textOnLayer.thisLayerType)
                    {
                        case LayerType.LightAccent:
                            item.ApplyColor(textAccentOnLight);
                            break;
                        case LayerType.DarkAccent:
                            item.ApplyColor(textAccentOnDark);
                            break;
                        case LayerType.FaintAccent:
                            item.ApplyColor(textAccentOnFaint);
                            break;
                        case LayerType.WhiteAccent:
                            item.ApplyColor(textAccentOnWhite);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch (item.textOnLayerType)
                    {
                        case LayerType.LightAccent:
                            item.ApplyColor(textAccentOnLight);
                            break;
                        case LayerType.DarkAccent:
                            item.ApplyColor(textAccentOnDark);
                            break;
                        case LayerType.FaintAccent:
                            item.ApplyColor(textAccentOnFaint);
                            break;
                        case LayerType.WhiteAccent:
                            item.ApplyColor(textAccentOnWhite);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        //switch (pageManager.selectedLayerType)
        //{
        //    case LayerType.LightAccent:
        //        pageManager.ApplyColor(lightAccent);
        //        break;
        //    case LayerType.DarkAccent:
        //        pageManager.ApplyColor(darkAccent);
        //        break;
        //    case LayerType.FaintAccent:
        //        pageManager.ApplyColor(faintAccent);
        //        break;
        //    case LayerType.WhiteAccent:
        //        pageManager.ApplyColor(whiteAccent);
        //        break;
        //    default:
        //        break;
        //}
    }

    [ExecuteAlways, ContextMenu("Generate And Apply Color")]
    public void GenerateAndApply()
    {
        GenerateColorPalette();
        ApplyColor();
    }

    public enum LayerType
    {
        LightAccent,
        DarkAccent,
        FaintAccent,
        WhiteAccent
    }
}
