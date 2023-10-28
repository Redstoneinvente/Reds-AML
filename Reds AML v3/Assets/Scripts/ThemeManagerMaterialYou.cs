using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using LayerType = ThemeManager.LayerType;

public class ThemeManagerMaterialYou : MonoBehaviour
{
    public GameObject primaryColorOverlayPrefab;

    public static ThemeManagerMaterialYou matYouThemeManager;

    public Color neutralKey;
    public Color neutralVariantKey;
    public Color primaryKey;
    public Color secondaryKey;
    public Color tertiaryKey;
    public Color errorKey;

    /// <summary>
    /// 40
    /// </summary>
    public Color primaryColor;

    /// <summary>
    /// 40
    /// </summary>
    public Color secondaryColor;

    /// <summary>
    /// 40
    /// </summary>
    public Color tertiaryColor;

    /// <summary>
    /// 99
    /// </summary>
    public Color background;

    /// <summary>
    /// 99
    /// </summary>
    public Color surface;

    /// <summary>
    /// 90
    /// </summary>
    public Color surfaceVariant;


    /// <summary>
    /// 100
    /// </summary>
    public Color onPrimary;

    /// <summary>
    /// 100
    /// </summary>
    public Color onSecondary;

    /// <summary>
    /// 100
    /// </summary>
    public Color onTertiary;

    /// <summary>
    /// 10
    /// </summary>
    public Color onBackground;

    /// <summary>
    /// 10
    /// </summary>
    public Color onSurface;

    /// <summary>
    /// 30
    /// </summary>
    public Color onSurfaceVariant;

    /// <summary>
    /// 50
    /// </summary>
    public Color outline;


    /// <summary>
    /// 90
    /// </summary>
    public Color primaryContainer;

    /// <summary>
    /// 90
    /// </summary>
    public Color secondaryContainer;

    /// <summary>
    /// 90
    /// </summary>
    public Color tertiaryContainer;


    /// <summary>
    /// 10
    /// </summary>
    public Color onPrimaryContainer;

    /// <summary>
    /// 10
    /// </summary>
    public Color onSecondaryContainer;

    /// <summary>
    /// 10
    /// </summary>
    public Color onTertiaryContainer;

    /// <summary>
    /// 87
    /// </summary>
    public Color surfaceDim;

    /// <summary>
    /// 98
    /// </summary>
    public Color surfaceBright;

    /// <summary>
    /// 100
    /// </summary>
    public Color surfaceContainerLowest;

    /// <summary>
    /// 96
    /// </summary>
    public Color surfaceContainerLow;

    /// <summary>
    /// 94
    /// </summary>
    public Color surfaceContainer;

    /// <summary>
    /// 92
    /// </summary>
    public Color surfaceContainerHigh;

    /// <summary>
    /// 90
    /// </summary>
    public Color surfaceContainerHighest;

    /// <summary>
    /// 40
    /// </summary>
    public Color error;

    /// <summary>
    /// 100
    /// </summary>
    public Color onError;

    /// <summary>
    /// 90
    /// </summary>
    public Color errorContainer;

    /// <summary>
    /// 10
    /// </summary>
    public Color onErrorContainer;

    //public Color textAccentOnLight;
    //public Color textAccentOnDark;
    //public Color textAccentOnFaint;
    //public Color textAccentOnWhite;

    //public float lightFactor;
    //public float darkFactor;
    //public Vector3 faintFactor;
    //public Vector3 whiteFactor;

    [Range(0f, 1f)]
    public float darkFactorToleranceForText;

    public bool enableDynamicUIUpdates;
    public bool enableDynamicUIStartUpdate;
    public static bool canDynamicUpdate;

    public static bool generate;
    public static bool apply;

    List<ThemeTag> allLayers;
    List<MaterialYouInputField> allInputs;

    [ExecuteAlways, ContextMenu("Generate Color")]
    public void GenerateColorPalette()
    {
        primaryColor = ApplyColorValues(40, primaryKey);
        secondaryColor = ApplyColorValues(40, secondaryKey);
        tertiaryColor = ApplyColorValues(40, tertiaryKey);
        error = ApplyColorValues(40, errorKey);

        //primaryColor = primaryKey;
        //secondaryColor = secondaryKey;
        //tertiaryColor = tertiaryKey;

        background = ApplyColorValues(99, neutralKey);
        surface = ApplyColorValues(99, neutralKey);

        onBackground = ApplyColorValues(10, neutralKey);
        onSurface = ApplyColorValues(10, neutralKey);

        surfaceVariant = ApplyColorValues(90, neutralVariantKey);
        onSurfaceVariant = ApplyColorValues(30, neutralVariantKey);

        outline = ApplyColorValues(50, neutralVariantKey);

        onPrimary = ApplyColorValues(100, primaryColor);
        primaryContainer = ApplyColorValues(90, primaryColor);
        onPrimaryContainer = ApplyColorValues(10, primaryColor);

        onSecondary = ApplyColorValues(100, secondaryColor);
        secondaryContainer = ApplyColorValues(90, secondaryColor);
        onSecondaryContainer = ApplyColorValues(10, secondaryColor);

        onTertiary = ApplyColorValues(100, tertiaryColor);
        tertiaryContainer = ApplyColorValues(90, tertiaryColor);
        onTertiaryContainer = ApplyColorValues(10, tertiaryColor);

        onError = ApplyColorValues(100, error);
        errorContainer = ApplyColorValues(90, error);
        onErrorContainer = ApplyColorValues(10, error);

        surfaceBright = ApplyColorValues(98, neutralKey);
        surfaceDim = ApplyColorValues(87, neutralKey);

        surfaceContainerLowest = ApplyColorValues(100, neutralKey);
        surfaceContainerLow = ApplyColorValues(96, neutralKey);

        surfaceContainer = ApplyColorValues(94, neutralKey);

        surfaceContainerHigh = ApplyColorValues(92, neutralKey);
        surfaceContainerHighest = ApplyColorValues(90, neutralKey);

        //textAccentOnLight = GenerateReadableTextColor(lightAccent);
        //textAccentOnDark = GenerateReadableTextColor(darkAccent);
        //textAccentOnFaint = GenerateReadableTextColor(faintAccent);
        //textAccentOnWhite = GenerateReadableTextColor(whiteAccent);
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

        if (apply)
        {
            apply = false;
            ApplyColor();
        }
    }

    Color ApplyColorValues(float factorO, Color mainColor)
    {
        float factor = factorO / 100;

        float H, S, V;

        Color.RGBToHSV(mainColor, out H, out S, out V);

        Color color = Color.HSVToRGB(H, S, factor);

        //Color color = mainColor + (Color.white * factor);
        //color.r = color.r - factor <= 0 ? 0 : color.r - factor;
        //color.g = color.g - factor <= 0 ? 0 : color.g - factor;
        //color.b = color.b - factor <= 0 ? 0 : color.b - factor;

        if (factor == 1)
        {
            return Color.white;
        }

        if (factor == 0)
        {
            return Color.black;
        }

        return color;
    }

    Color ApplyColorValues(Vector3 factor, Color mainColor)
    {
        Color color = mainColor;
        color.r = color.r - factor.x <= 0 ? 0 : color.r - factor.x;
        color.g = color.g - factor.y <= 0 ? 0 : color.g - factor.y;
        color.b = color.b - factor.z <= 0 ? 0 : color.b - factor.z;

        return color;
    }

    Color GenerateReadableTextColor(Color colorOnTop, Color mainColor)
    {
        Color color = mainColor;

        float luminancePercentage = CalculateLuminace(colorOnTop);

        if (luminancePercentage > darkFactorToleranceForText)
        {
            //Light Color
            color = ApplyColorValues(0.8f, mainColor);
        }
        else
        {
            //Dark Color
            color = ApplyColorValues(-0.8f, mainColor);
        }

        return color;
    }

    public static Color GenerateReadableTextColorGlobal(Color colorOnTop, Color mainColor)
    {
        Color color = mainColor;

        float luminancePercentage = CalculateLuminaceGlobal(colorOnTop);

        if (luminancePercentage > matYouThemeManager.darkFactorToleranceForText)
        {
            //Light Color
            color = matYouThemeManager.ApplyColorValues(0.8f, mainColor);
        }
        else
        {
            //Dark Color
            color = matYouThemeManager.ApplyColorValues(-0.8f, mainColor);
        }

        return color;
    }

    void Awake()
    {
        if (matYouThemeManager == default)
        {
            matYouThemeManager = this;
        }

        allLayers = new List<ThemeTag>(GameObject.FindObjectsOfType<ThemeTag>(false));
        allInputs = new List<MaterialYouInputField>(GameObject.FindObjectsOfType<MaterialYouInputField>(false));
    }

    public float CalculateLuminace(Color color)
    {
        float r = color.r;
        float g = color.g;
        float b = color.b;

        return (r * 0.299f + g * 0.587f + b * 0.114f) / 1;
    }

    public static float CalculateLuminaceGlobal(Color color)
    {
        float r = color.r;
        float g = color.g;
        float b = color.b;

        return (r * 0.299f + g * 0.587f + b * 0.114f) / 1;
    }

    [ExecuteAlways, ContextMenu("Apply Color")]
    public void ApplyColor()
    {
        if (allLayers == default)
        {
            allLayers = new List<ThemeTag>();
        }

        if (allInputs == default)
        {
            allInputs = new List<MaterialYouInputField>();
        }

        if (!Application.isPlaying && Application.isEditor)
        {
            allLayers = new List<ThemeTag>(GameObject.FindObjectsOfType<ThemeTag>(false));
            allInputs = new List<MaterialYouInputField>(GameObject.FindObjectsOfType<MaterialYouInputField>(false));
        }

        foreach (var item in allLayers)
        {
            if (item.primaryOverlay != default)
            {
                DestroyImmediate(item.primaryOverlay);
                item.primaryOverlay = default;
            }

            if (!item.isOnLayer)
            {
                switch (item.thisLayerTypeYou)
                {
                    case LayerTypeYou.Primary:
                        item.ApplyColor(primaryColor);
                        break;
                    case LayerTypeYou.Secondary:
                        item.ApplyColor(secondaryColor);
                        break;
                    case LayerTypeYou.Tertiary:
                        item.ApplyColor(tertiaryColor);
                        break;
                    case LayerTypeYou.PrimaryContainer:
                        item.ApplyColor(primaryContainer);
                        break;
                    case LayerTypeYou.SecondaryContainer:
                        item.ApplyColor(secondaryContainer);
                        break;
                    case LayerTypeYou.TertiaryContainer:
                        item.ApplyColor(tertiaryContainer);
                        break;
                    case LayerTypeYou.Background:
                        item.ApplyColor(background);
                        break;
                    case LayerTypeYou.Surface:
                        item.ApplyColor(surface);
                        try
                        {
                            item.primaryOverlay = CreatePrimaryColorOverlay(item.transform);
                        }
                        catch (System.Exception)
                        {

                        }
                        break;
                    case LayerTypeYou.SurfaceVariant:
                        item.ApplyColor(surfaceVariant);
                        break;
                    case LayerTypeYou.Outline:
                        item.ApplyColor(outline);
                        break;
                    case LayerTypeYou.Error:
                        item.ApplyColor(error);
                        break;
                    case LayerTypeYou.OnError:
                        item.ApplyColor(onError);
                        break;
                    case LayerTypeYou.ErrorContainer:
                        item.ApplyColor(errorContainer);
                        break;
                    case LayerTypeYou.OnErrorContainer:
                        item.ApplyColor(onErrorContainer);
                        break;
                    case LayerTypeYou.SurfaceDim:
                        item.ApplyColor(surfaceDim);
                        break;
                    case LayerTypeYou.SurfaceBright:
                        item.ApplyColor(surfaceBright);
                        break;
                    case LayerTypeYou.SurfaceContainer:
                        item.ApplyColor(surfaceContainer);
                        break;
                    case LayerTypeYou.SurfaceContainerHigh:
                        item.ApplyColor(surfaceContainerHigh);
                        break;
                    case LayerTypeYou.SurfaceContainerHighest:
                        item.ApplyColor(surfaceContainerHighest);
                        break;
                    case LayerTypeYou.SurfaceContainerLow:
                        item.ApplyColor(surfaceContainerLow);
                        break;
                    case LayerTypeYou.SurfaceContainerLowest:
                        item.ApplyColor(surfaceContainerLowest);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                if (item.textOnLayer != default)
                {
                    switch (item.textOnLayer.thisLayerTypeYou)
                    {
                        case LayerTypeYou.Primary:
                            item.ApplyColor(onPrimary);
                            break;
                        case LayerTypeYou.Secondary:
                            item.ApplyColor(onSecondary);
                            break;
                        case LayerTypeYou.Tertiary:
                            item.ApplyColor(onTertiary);
                            break;
                        case LayerTypeYou.PrimaryContainer:
                            item.ApplyColor(onPrimaryContainer);
                            break;
                        case LayerTypeYou.SecondaryContainer:
                            item.ApplyColor(onSecondaryContainer);
                            break;
                        case LayerTypeYou.TertiaryContainer:
                            item.ApplyColor(onTertiaryContainer);
                            break;
                        case LayerTypeYou.Background:
                            item.ApplyColor(onBackground);
                            break;
                        case LayerTypeYou.Surface:
                            item.ApplyColor(onSurface);
                            break;
                        case LayerTypeYou.SurfaceVariant:
                            item.ApplyColor(onSurfaceVariant);
                            break;
                        case LayerTypeYou.Outline:
                            item.ApplyColor(primaryColor);
                            break;
                        case LayerTypeYou.Error:
                            item.ApplyColor(error);
                            break;
                        case LayerTypeYou.OnError:
                            item.ApplyColor(onError);
                            break;
                        case LayerTypeYou.ErrorContainer:
                            item.ApplyColor(errorContainer);
                            break;
                        case LayerTypeYou.OnErrorContainer:
                            item.ApplyColor(onErrorContainer);
                            break;
                        case LayerTypeYou.SurfaceDim:
                            item.ApplyColor(onSurface);
                            break;
                        case LayerTypeYou.SurfaceBright:
                            item.ApplyColor(onSurface);
                            break;
                        case LayerTypeYou.SurfaceContainer:
                            item.ApplyColor(onSurface);
                            break;
                        case LayerTypeYou.SurfaceContainerHigh:
                            item.ApplyColor(onSurface);
                            break;
                        case LayerTypeYou.SurfaceContainerHighest:
                            item.ApplyColor(onSurface);
                            break;
                        case LayerTypeYou.SurfaceContainerLow:
                            item.ApplyColor(onSurface);
                            break;
                        case LayerTypeYou.SurfaceContainerLowest:
                            item.ApplyColor(onSurface);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch (item.textOnLayerTypeYou)
                    {
                        case LayerTypeYou.Primary:
                            item.ApplyColor(onPrimary);
                            break;
                        case LayerTypeYou.Secondary:
                            item.ApplyColor(onSecondary);
                            break;
                        case LayerTypeYou.Tertiary:
                            item.ApplyColor(onTertiary);
                            break;
                        case LayerTypeYou.PrimaryContainer:
                            item.ApplyColor(onPrimaryContainer);
                            break;
                        case LayerTypeYou.SecondaryContainer:
                            item.ApplyColor(onSecondaryContainer);
                            break;
                        case LayerTypeYou.TertiaryContainer:
                            item.ApplyColor(onTertiaryContainer);
                            break;
                        case LayerTypeYou.Background:
                            item.ApplyColor(onBackground);
                            break;
                        case LayerTypeYou.Surface:
                            item.ApplyColor(onSurface);
                            break;
                        case LayerTypeYou.SurfaceVariant:
                            item.ApplyColor(onSurfaceVariant);
                            break;
                        case LayerTypeYou.Outline:
                            item.ApplyColor(primaryColor);
                            break;
                        case LayerTypeYou.Error:
                            item.ApplyColor(onError);
                            break;
                        case LayerTypeYou.OnError:
                            item.ApplyColor(onError);
                            break;
                        case LayerTypeYou.ErrorContainer:
                            item.ApplyColor(onErrorContainer);
                            break;
                        case LayerTypeYou.OnErrorContainer:
                            item.ApplyColor(onErrorContainer);
                            break;
                        case LayerTypeYou.SurfaceDim:
                            item.ApplyColor(onSurface);
                            break;
                        case LayerTypeYou.SurfaceBright:
                            item.ApplyColor(onSurface);
                            break;
                        case LayerTypeYou.SurfaceContainer:
                            item.ApplyColor(onSurface);
                            break;
                        case LayerTypeYou.SurfaceContainerHigh:
                            item.ApplyColor(onSurface);
                            break;
                        case LayerTypeYou.SurfaceContainerHighest:
                            item.ApplyColor(onSurface);
                            break;
                        case LayerTypeYou.SurfaceContainerLow:
                            item.ApplyColor(onSurface);
                            break;
                        case LayerTypeYou.SurfaceContainerLowest:
                            item.ApplyColor(onSurface);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        foreach (var item in allInputs)
        {
            Color nrmlColor = Color.white;
            Color selectedColor = Color.white;
            Color error = Color.red;

            switch (item.layerTypeYouNormal)
            {
                case LayerTypeYou.Primary:
                    nrmlColor = primaryColor;
                    break;
                case LayerTypeYou.Secondary:
                    nrmlColor = secondaryColor;
                    break;
                case LayerTypeYou.Tertiary:
                    nrmlColor = tertiaryColor;
                    break;
                case LayerTypeYou.PrimaryContainer:
                    nrmlColor = primaryContainer;
                    break;
                case LayerTypeYou.SecondaryContainer:
                    nrmlColor = secondaryContainer;
                    break;
                case LayerTypeYou.TertiaryContainer:
                    nrmlColor = tertiaryContainer;
                    break;
                case LayerTypeYou.Background:
                    nrmlColor = background;
                    break;
                case LayerTypeYou.OnBackground:
                    nrmlColor = onBackground;
                    break;
                case LayerTypeYou.Surface:
                    nrmlColor = surface;
                    break;
                case LayerTypeYou.OnSurface:
                    nrmlColor = onSurface;
                    break;
                case LayerTypeYou.SurfaceVariant:
                    nrmlColor = surfaceVariant;
                    break;
                case LayerTypeYou.OnSurfaceVariant:
                    nrmlColor = onSurfaceVariant;
                    break;
                case LayerTypeYou.Outline:
                    nrmlColor = outline;
                    break;
                case LayerTypeYou.OnPrimaryContainer:
                    nrmlColor = onPrimaryContainer;
                    break;
                case LayerTypeYou.OnSecondaryContainer:
                    nrmlColor = onSecondaryContainer;
                    break;
                case LayerTypeYou.OnTertiaryContainer:
                    nrmlColor = onTertiaryContainer;
                    break;
                case LayerTypeYou.OnPrimary:
                    nrmlColor = onPrimary;
                    break;
                case LayerTypeYou.OnSecondary:
                    nrmlColor = onSecondary;
                    break;
                case LayerTypeYou.OnTertiary:
                    nrmlColor = onTertiary;
                    break;
                case LayerTypeYou.SurfaceDim:
                    nrmlColor = (surfaceDim);
                    break;
                case LayerTypeYou.SurfaceBright:
                    nrmlColor = (surfaceBright);
                    break;
                case LayerTypeYou.SurfaceContainer:
                    nrmlColor = (surfaceContainer);
                    break;
                case LayerTypeYou.SurfaceContainerHigh:
                    nrmlColor = (surfaceContainerHigh);
                    break;
                case LayerTypeYou.SurfaceContainerHighest:
                    nrmlColor = (surfaceContainerHighest);
                    break;
                case LayerTypeYou.SurfaceContainerLow:
                    nrmlColor = (surfaceContainerLow);
                    break;
                case LayerTypeYou.SurfaceContainerLowest:
                    nrmlColor = (surfaceContainerLowest);
                    break;
                default:
                    break;
            }

            switch (item.layerTypeYouSelected)
            {
                case LayerTypeYou.Primary:
                    selectedColor = primaryColor;
                    break;
                case LayerTypeYou.Secondary:
                    selectedColor = secondaryColor;
                    break;
                case LayerTypeYou.Tertiary:
                    selectedColor = tertiaryColor;
                    break;
                case LayerTypeYou.PrimaryContainer:
                    selectedColor = primaryContainer;
                    break;
                case LayerTypeYou.SecondaryContainer:
                    selectedColor = secondaryContainer;
                    break;
                case LayerTypeYou.TertiaryContainer:
                    selectedColor = tertiaryContainer;
                    break;
                case LayerTypeYou.Background:
                    selectedColor = background;
                    break;
                case LayerTypeYou.OnBackground:
                    selectedColor = onBackground;
                    break;
                case LayerTypeYou.Surface:
                    selectedColor = surface;
                    break;
                case LayerTypeYou.OnSurface:
                    selectedColor = onSurface;
                    break;
                case LayerTypeYou.SurfaceVariant:
                    selectedColor = surfaceVariant;
                    break;
                case LayerTypeYou.OnSurfaceVariant:
                    selectedColor = onSurfaceVariant;
                    break;
                case LayerTypeYou.Outline:
                    selectedColor = outline;
                    break;
                case LayerTypeYou.OnPrimaryContainer:
                    selectedColor = onPrimaryContainer;
                    break;
                case LayerTypeYou.OnSecondaryContainer:
                    selectedColor = onSecondaryContainer;
                    break;
                case LayerTypeYou.OnTertiaryContainer:
                    selectedColor = onTertiaryContainer;
                    break;
                case LayerTypeYou.OnPrimary:
                    selectedColor = onPrimary;
                    break;
                case LayerTypeYou.OnSecondary:
                    selectedColor = onSecondary;
                    break;
                case LayerTypeYou.OnTertiary:
                    selectedColor = onTertiary;
                    break;
                case LayerTypeYou.SurfaceDim:
                    selectedColor = (surfaceDim);
                    break;
                case LayerTypeYou.SurfaceBright:
                    selectedColor = (surfaceBright);
                    break;
                case LayerTypeYou.SurfaceContainer:
                    selectedColor = (surfaceContainer);
                    break;
                case LayerTypeYou.SurfaceContainerHigh:
                    selectedColor = (surfaceContainerHigh);
                    break;
                case LayerTypeYou.SurfaceContainerHighest:
                    selectedColor = (surfaceContainerHighest);
                    break;
                case LayerTypeYou.SurfaceContainerLow:
                    selectedColor = (surfaceContainerLow);
                    break;
                case LayerTypeYou.SurfaceContainerLowest:
                    selectedColor = (surfaceContainerLowest);
                    break;
                default:
                    break;
            }

            switch (item.layerTypeYouError)
            {
                case LayerTypeYou.Primary:
                    error = primaryColor;
                    break;
                case LayerTypeYou.Secondary:
                    error = secondaryColor;
                    break;
                case LayerTypeYou.Tertiary:
                    error = tertiaryColor;
                    break;
                case LayerTypeYou.PrimaryContainer:
                    error = primaryContainer;
                    break;
                case LayerTypeYou.SecondaryContainer:
                    error = secondaryContainer;
                    break;
                case LayerTypeYou.TertiaryContainer:
                    error = tertiaryContainer;
                    break;
                case LayerTypeYou.Background:
                    error = background;
                    break;
                case LayerTypeYou.OnBackground:
                    error = onBackground;
                    break;
                case LayerTypeYou.Surface:
                    error = surface;
                    break;
                case LayerTypeYou.OnSurface:
                    error = onSurface;
                    break;
                case LayerTypeYou.SurfaceVariant:
                    error = surfaceVariant;
                    break;
                case LayerTypeYou.OnSurfaceVariant:
                    error = onSurfaceVariant;
                    break;
                case LayerTypeYou.Outline:
                    error = outline;
                    break;
                case LayerTypeYou.OnPrimaryContainer:
                    error = onPrimaryContainer;
                    break;
                case LayerTypeYou.OnSecondaryContainer:
                    error = onSecondaryContainer;
                    break;
                case LayerTypeYou.OnTertiaryContainer:
                    error = onTertiaryContainer;
                    break;
                case LayerTypeYou.OnPrimary:
                    error = onPrimary;
                    break;
                case LayerTypeYou.OnSecondary:
                    error = onSecondary;
                    break;
                case LayerTypeYou.OnTertiary:
                    error = onTertiary;
                    break;
                case LayerTypeYou.Error:
                    error = this.error;
                    break;
                case LayerTypeYou.OnError:
                    error = this.onError;
                    break;
                case LayerTypeYou.ErrorContainer:
                    error = this.errorContainer;
                    break;
                case LayerTypeYou.OnErrorContainer:
                    error = this.onErrorContainer;
                    break;
                case LayerTypeYou.SurfaceDim:
                    error = (surfaceDim);
                    break;
                case LayerTypeYou.SurfaceBright:
                    error = (surfaceBright);
                    break;
                case LayerTypeYou.SurfaceContainer:
                    error = (surfaceContainer);
                    break;
                case LayerTypeYou.SurfaceContainerHigh:
                    error = (surfaceContainerHigh);
                    break;
                case LayerTypeYou.SurfaceContainerHighest:
                    error = (surfaceContainerHighest);
                    break;
                case LayerTypeYou.SurfaceContainerLow:
                    error = (surfaceContainerLow);
                    break;
                case LayerTypeYou.SurfaceContainerLowest:
                    error = (surfaceContainerLowest);
                    break;
                default:
                    break;
            }

            if (item.gameObject.activeInHierarchy)
            {
                item.ApplyColor(nrmlColor, selectedColor, error);
            }
        }

        #region OLD
        //if (pageManager == default)
        //{
        //    return;
        //}

        //if (pageManager.primaryOverlay != default)
        //{
        //    DestroyImmediate(pageManager.primaryOverlay);
        //    pageManager.primaryOverlay = default;
        //}

        //switch (pageManager.selectedLayerTypeYou)
        //{
        //    case LayerTypeYou.Primary:
        //        pageManager.ApplyColor(primaryColor);
        //        break;
        //    case LayerTypeYou.Secondary:
        //        pageManager.ApplyColor(secondaryColor);
        //        break;
        //    case LayerTypeYou.Tertiary:
        //        pageManager.ApplyColor(tertiaryColor);
        //        break;
        //    case LayerTypeYou.PrimaryContainer:
        //        pageManager.ApplyColor(primaryContainer);
        //        break;
        //    case LayerTypeYou.SecondaryContainer:
        //        pageManager.ApplyColor(secondaryContainer);
        //        break;
        //    case LayerTypeYou.TertiaryContainer:
        //        pageManager.ApplyColor(tertiaryContainer);
        //        break;
        //    case LayerTypeYou.Background:
        //        pageManager.ApplyColor(background);
        //        break;
        //    case LayerTypeYou.Surface:
        //        pageManager.ApplyColor(surface);
        //        pageManager.primaryOverlay = CreatePrimaryColorOverlay(pageManager.transform);
        //        break;
        //    case LayerTypeYou.SurfaceVariant:
        //        pageManager.ApplyColor(surfaceVariant);
        //        break;
        //    case LayerTypeYou.Outline:
        //        pageManager.ApplyColor(outline);
        //        break;
        //    default:
        //        break;
        //}

        //switch (pageManager.selectedTextLayer)
        //{
        //    case LayerTypeYou.Primary:
        //        pageManager.ApplyColorS(primaryColor);
        //        break;
        //    case LayerTypeYou.Secondary:
        //        pageManager.ApplyColorS(secondaryColor);
        //        break;
        //    case LayerTypeYou.Tertiary:
        //        pageManager.ApplyColorS(tertiaryColor);
        //        break;
        //    case LayerTypeYou.PrimaryContainer:
        //        pageManager.ApplyColorS(primaryContainer);
        //        break;
        //    case LayerTypeYou.SecondaryContainer:
        //        pageManager.ApplyColorS(secondaryContainer);
        //        break;
        //    case LayerTypeYou.TertiaryContainer:
        //        pageManager.ApplyColorS(tertiaryContainer);
        //        break;
        //    case LayerTypeYou.Background:
        //        pageManager.ApplyColorS(background);
        //        break;
        //    case LayerTypeYou.Surface:
        //        pageManager.ApplyColorS(surface);
        //        pageManager.primaryOverlay = CreatePrimaryColorOverlay(pageManager.transform);
        //        break;
        //    case LayerTypeYou.SurfaceVariant:
        //        pageManager.ApplyColorS(surfaceVariant);
        //        break;
        //    case LayerTypeYou.Outline:
        //        pageManager.ApplyColorS(outline);
        //        break;
        //    case LayerTypeYou.OnSurface:
        //        pageManager.ApplyColorS(onSurface);
        //        break;
        //    case LayerTypeYou.OnSurfaceVariant:
        //        pageManager.ApplyColorS(onSurfaceVariant);
        //        break;
        //    case LayerTypeYou.OnBackground:
        //        pageManager.ApplyColorS(onBackground);
        //        break;
        //    case LayerTypeYou.OnSecondaryContainer:
        //        pageManager.ApplyColorS(onSecondaryContainer);
        //        break;
        //    case LayerTypeYou.OnTertiaryContainer:
        //        pageManager.ApplyColorS(onTertiaryContainer);
        //        break;
        //    case LayerTypeYou.OnPrimaryContainer:
        //        pageManager.ApplyColorS(onPrimaryContainer);
        //        break;
        //    case LayerTypeYou.OnPrimary:
        //        pageManager.ApplyColorS(onPrimary);
        //        break;
        //    case LayerTypeYou.OnSecondary:
        //        pageManager.ApplyColorS(onSecondary);
        //        break;
        //    case LayerTypeYou.OnTertiary:
        //        pageManager.ApplyColorS(onTertiary);
        //        break;
        //    default:
        //        break;
        //}

        //switch (pageManager.normalTextLayer)
        //{
        //    case LayerTypeYou.Primary:
        //        pageManager.ApplyColorN(primaryColor);
        //        break;
        //    case LayerTypeYou.Secondary:
        //        pageManager.ApplyColorN(secondaryColor);
        //        break;
        //    case LayerTypeYou.Tertiary:
        //        pageManager.ApplyColorN(tertiaryColor);
        //        break;
        //    case LayerTypeYou.PrimaryContainer:
        //        pageManager.ApplyColorN(primaryContainer);
        //        break;
        //    case LayerTypeYou.SecondaryContainer:
        //        pageManager.ApplyColorN(secondaryContainer);
        //        break;
        //    case LayerTypeYou.TertiaryContainer:
        //        pageManager.ApplyColorN(tertiaryContainer);
        //        break;
        //    case LayerTypeYou.Background:
        //        pageManager.ApplyColorN(background);
        //        break;
        //    case LayerTypeYou.Surface:
        //        pageManager.ApplyColorN(surface);
        //        pageManager.primaryOverlay = CreatePrimaryColorOverlay(pageManager.transform);
        //        break;
        //    case LayerTypeYou.SurfaceVariant:
        //        pageManager.ApplyColorN(surfaceVariant);
        //        break;
        //    case LayerTypeYou.Outline:
        //        pageManager.ApplyColorN(outline);
        //        break;
        //    case LayerTypeYou.OnSurface:
        //        pageManager.ApplyColorN(onSurface);
        //        break;
        //    case LayerTypeYou.OnSurfaceVariant:
        //        pageManager.ApplyColorN(onSurfaceVariant);
        //        break;
        //    case LayerTypeYou.OnBackground:
        //        pageManager.ApplyColorN(onBackground);
        //        break;
        //    case LayerTypeYou.OnSecondaryContainer:
        //        pageManager.ApplyColorN(onSecondaryContainer);
        //        break;
        //    case LayerTypeYou.OnTertiaryContainer:
        //        pageManager.ApplyColorN(onTertiaryContainer);
        //        break;
        //    case LayerTypeYou.OnPrimaryContainer:
        //        pageManager.ApplyColorN(onPrimaryContainer);
        //        break;
        //    case LayerTypeYou.OnPrimary:
        //        pageManager.ApplyColorN(onPrimary);
        //        break;
        //    case LayerTypeYou.OnSecondary:
        //        pageManager.ApplyColorN(onSecondary);
        //        break;
        //    case LayerTypeYou.OnTertiary:
        //        pageManager.ApplyColorN(onTertiary);
        //        break;
        //    default:
        //        break;
        //}
        #endregion
    }

    public Color GetColor(LayerTypeYou layer)
    {
        switch (layer)
        {
            case LayerTypeYou.Primary:
                return (primaryColor);
                break;
            case LayerTypeYou.Secondary:
                return (secondaryColor);
                break;
            case LayerTypeYou.Tertiary:
                return (tertiaryColor);
                break;
            case LayerTypeYou.PrimaryContainer:
                return (primaryContainer);
                break;
            case LayerTypeYou.SecondaryContainer:
                return (secondaryContainer);
                break;
            case LayerTypeYou.TertiaryContainer:
                return (tertiaryContainer);
                break;
            case LayerTypeYou.Background:
                return (background);
                break;
            case LayerTypeYou.Surface:
                return (surface);
                break;
            case LayerTypeYou.SurfaceVariant:
                return (surfaceVariant);
                break;
            case LayerTypeYou.Outline:
                return (outline);
                break;
            case LayerTypeYou.OnSurface:
                return (onSurface);
                break;
            case LayerTypeYou.OnSurfaceVariant:
                return (onSurfaceVariant);
                break;
            case LayerTypeYou.OnBackground:
                return (onBackground);
                break;
            case LayerTypeYou.OnSecondaryContainer:
                return (onSecondaryContainer);
                break;
            case LayerTypeYou.OnTertiaryContainer:
                return (onTertiaryContainer);
                break;
            case LayerTypeYou.OnPrimaryContainer:
                return (onPrimaryContainer);
                break;
            case LayerTypeYou.OnPrimary:
                return (onPrimary);
                break;
            case LayerTypeYou.OnSecondary:
                return (onSecondary);
                break;
            case LayerTypeYou.OnTertiary:
                return (onTertiary);
                break;
            case LayerTypeYou.SurfaceDim:
                return(surfaceDim);
                break;
            case LayerTypeYou.SurfaceBright:
                return(surfaceBright);
                break;
            case LayerTypeYou.SurfaceContainer:
                return(surfaceContainer);
                break;
            case LayerTypeYou.SurfaceContainerHigh:
                return(surfaceContainerHigh);
                break;
            case LayerTypeYou.SurfaceContainerHighest:
                return(surfaceContainerHighest);
                break;
            case LayerTypeYou.SurfaceContainerLow:
                return(surfaceContainerLow);
                break;
            case LayerTypeYou.SurfaceContainerLowest:
                return(surfaceContainerLowest);
                break;
            default:
                break;
        }

        return Color.black;
    }

    public bool RequirePrimaryOverlay(LayerTypeYou layer)
    {
        switch (layer)
        {
            case LayerTypeYou.Primary:
                return false;
                break;
            case LayerTypeYou.Secondary:
                return false;
                break;
            case LayerTypeYou.Tertiary:
                return false;
                break;
            case LayerTypeYou.PrimaryContainer:
                return false;
                break;
            case LayerTypeYou.SecondaryContainer:
                return false;
                break;
            case LayerTypeYou.TertiaryContainer:
                return false;
                break;
            case LayerTypeYou.Background:
                return false;
                break;
            case LayerTypeYou.Surface:
                return true;
                break;
            case LayerTypeYou.SurfaceVariant:
                return true;
                break;
            case LayerTypeYou.Outline:
                return false;
                break;
            case LayerTypeYou.OnSurface:
                return false;
                break;
            case LayerTypeYou.OnSurfaceVariant:
                return false;
                break;
            case LayerTypeYou.OnBackground:
                return false;
                break;
            case LayerTypeYou.OnSecondaryContainer:
                return false;
                break;
            case LayerTypeYou.OnTertiaryContainer:
                return false;
                break;
            case LayerTypeYou.OnPrimaryContainer:
                return false;
                break;
            case LayerTypeYou.OnPrimary:
                return false;
                break;
            case LayerTypeYou.OnSecondary:
                return false;
                break;
            case LayerTypeYou.OnTertiary:
                return false;
                break;
            case LayerTypeYou.SurfaceDim:
                return true;
                break;
            case LayerTypeYou.SurfaceBright:
                return true;
                break;
            case LayerTypeYou.SurfaceContainer:
                return true;
                break;
            case LayerTypeYou.SurfaceContainerHigh:
                return true;
                break;
            case LayerTypeYou.SurfaceContainerHighest:
                return true;
                break;
            case LayerTypeYou.SurfaceContainerLow:
                return true;
                break;
            case LayerTypeYou.SurfaceContainerLowest:
                return true;
                break;
            default:
                break;
        }

        return false;
    }

    [ExecuteAlways, ContextMenu("Generate And Apply Color")]
    public void GenerateAndApply()
    {
        GenerateColorPalette();
        ApplyColor();
    }

    public LayerTypeYou GetOnLayerTypeYou(LayerTypeYou layer)
    {
        switch (layer)
        {
            case LayerTypeYou.Primary:
                return LayerTypeYou.OnPrimary;
                break;
            case LayerTypeYou.Secondary:
                return LayerTypeYou.OnSecondary;
                break;
            case LayerTypeYou.Tertiary:
                return LayerTypeYou.OnTertiary;
                break;
            case LayerTypeYou.PrimaryContainer:
                return LayerTypeYou.OnPrimaryContainer;
                break;
            case LayerTypeYou.SecondaryContainer:
                return LayerTypeYou.OnSecondaryContainer;
                break;
            case LayerTypeYou.TertiaryContainer:
                return LayerTypeYou.OnTertiaryContainer;
                break;
            case LayerTypeYou.Background:
                return LayerTypeYou.OnBackground;
                break;
            case LayerTypeYou.OnBackground:
                break;
            case LayerTypeYou.Surface:
                return LayerTypeYou.OnSurface;
                break;
            case LayerTypeYou.OnSurface:
                break;
            case LayerTypeYou.SurfaceVariant:
                return LayerTypeYou.OnSurfaceVariant;
                break;
            case LayerTypeYou.OnSurfaceVariant:
                break;
            case LayerTypeYou.Outline:
                break;
            case LayerTypeYou.OnPrimaryContainer:
                break;
            case LayerTypeYou.OnSecondaryContainer:
                break;
            case LayerTypeYou.OnTertiaryContainer:
                break;
            case LayerTypeYou.OnPrimary:
                break;
            case LayerTypeYou.OnSecondary:
                break;
            case LayerTypeYou.OnTertiary:
                break;
            case LayerTypeYou.SurfaceDim:
                return LayerTypeYou.OnSurface;
                break;
            case LayerTypeYou.SurfaceBright:
                return LayerTypeYou.OnSurface;
                break;
            case LayerTypeYou.SurfaceContainer:
                return LayerTypeYou.OnSurface;
                break;
            case LayerTypeYou.SurfaceContainerHigh:
                return LayerTypeYou.OnSurface;
                break;
            case LayerTypeYou.SurfaceContainerHighest:
                return LayerTypeYou.OnSurface;
                break;
            case LayerTypeYou.SurfaceContainerLow:
                return LayerTypeYou.OnSurface;
                break;
            case LayerTypeYou.SurfaceContainerLowest:
                return LayerTypeYou.OnSurface;
                break;
            default:
                break;
        }

        return layer;
    }

    public GameObject CreatePrimaryColorOverlay(Transform transform)
    {
        if (transform.Find("PrimaryOverlay") == default)
        {
            GameObject go = Instantiate(primaryColorOverlayPrefab, transform);

            go.name = "PrimaryOverlay";

            go.transform.SetAsFirstSibling();

            go.GetComponent<Image>().sprite = transform.GetComponent<Image>().sprite;
            go.GetComponent<Image>().pixelsPerUnitMultiplier = transform.GetComponent<Image>().pixelsPerUnitMultiplier;

            ThemeTag themeTag = transform.GetComponent<ThemeTag>();

            float opacity = 0;

            if (themeTag.elevation == 1)
            {
                opacity = 0.05f;
            }

            if (themeTag.elevation == 2)
            {
                opacity = 0.08f;
            }

            if (themeTag.elevation == 3)
            {
                opacity = 0.11f;
            }

            if (themeTag.elevation == 4)
            {
                opacity = 0.12f;
            }

            if (themeTag.elevation == 5)
            {
                opacity = 0.14f;
            }

            go.GetComponent<ThemeTag>().ApplyColor(primaryColor, opacity);

            return go;
        }
        else
        {
            GameObject go = transform.Find("PrimaryOverlay").gameObject;

            ThemeTag themeTag = transform.GetComponent<ThemeTag>();

            go.GetComponent<Image>().sprite = transform.GetComponent<Image>().sprite;
            go.GetComponent<Image>().pixelsPerUnitMultiplier = transform.GetComponent<Image>().pixelsPerUnitMultiplier;

            float opacity = 0;

            if (themeTag.elevation == 1)
            {
                opacity = 0.05f;
            }

            if (themeTag.elevation == 2)
            {
                opacity = 0.08f;
            }

            if (themeTag.elevation == 3)
            {
                opacity = 0.11f;
            }

            if (themeTag.elevation == 4)
            {
                opacity = 0.12f;
            }

            if (themeTag.elevation == 5)
            {
                opacity = 0.14f;
            }

            go.GetComponent<ThemeTag>().ApplyColor(primaryColor, opacity);

            return go;
        }

        return default;
    }

    public GameObject CreateStateLayerOverlay(Transform transform, LayerTypeYou parentLayer, ThemeTag.State state)
    {
        if (transform.Find("StateLayerOverlay") == default)
        {
            GameObject go = Instantiate(primaryColorOverlayPrefab, transform);

            go.GetComponent<Image>().sprite = transform.GetComponent<Image>().sprite;
            go.GetComponent<Image>().pixelsPerUnitMultiplier = transform.GetComponent<Image>().pixelsPerUnitMultiplier;

            go.name = "StateLayerOverlay";

            go.transform.SetAsFirstSibling();

            float opacity = 0;

            if (state == ThemeTag.State.Hover)
            {
                opacity = 0.08f;
            }

            if (state == ThemeTag.State.Focus)
            {
                opacity = 0.12f;
            }

            if (state == ThemeTag.State.Press)
            {
                opacity = 0.12f;
            }

            if (state == ThemeTag.State.Drag)
            {
                opacity = 0.16f;
            }

            if (state == ThemeTag.State.Disable)
            {
                opacity = 0.38f;
            }

            go.GetComponent<ThemeTag>().ApplyColor(GetColor(GetOnLayerTypeYou(parentLayer)), opacity);

            return go;
        }
        else
        {
            GameObject go = transform.Find("StateLayerOverlay").gameObject;

            go.GetComponent<Image>().sprite = transform.GetComponent<Image>().sprite;
            go.GetComponent<Image>().pixelsPerUnitMultiplier = transform.GetComponent<Image>().pixelsPerUnitMultiplier;

            float opacity = 0;

            if (state == ThemeTag.State.Hover)
            {
                opacity = 0.08f;
            }

            if (state == ThemeTag.State.Focus)
            {
                opacity = 0.12f;
            }

            if (state == ThemeTag.State.Press)
            {
                opacity = 0.12f;
            }

            if (state == ThemeTag.State.Drag)
            {
                opacity = 0.16f;
            }

            go.GetComponent<ThemeTag>().ApplyColor(GetColor(GetOnLayerTypeYou(parentLayer)), opacity);

            return go;
        }

        return default;
    }

    public enum LayerTypeYou
    {
        Primary,
        Secondary,
        Tertiary,
        PrimaryContainer,
        SecondaryContainer,
        TertiaryContainer,
        Background,
        OnBackground,
        Surface,
        OnSurface,
        SurfaceVariant,
        OnSurfaceVariant,
        Outline,
        OnPrimaryContainer,
        OnSecondaryContainer,
        OnTertiaryContainer,
        OnPrimary,
        OnSecondary,
        OnTertiary,
        Error,
        OnError,
        ErrorContainer,
        OnErrorContainer,
        SurfaceDim,
        SurfaceBright,
        SurfaceContainerLowest,
        SurfaceContainerLow,
        SurfaceContainer,
        SurfaceContainerHigh,
        SurfaceContainerHighest
    }
}
