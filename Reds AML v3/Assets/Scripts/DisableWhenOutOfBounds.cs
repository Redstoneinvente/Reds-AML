using UnityEngine;
using UnityEngine.UI;

public class DisableWhenOutOfBounds : MonoBehaviour
{
    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public static bool IsVisible(RectTransform rectTransform)
    {
        Rect rect = new Rect(rectTransform.position.x - rectTransform.rect.width / 2,
                             rectTransform.position.y - rectTransform.rect.height / 2,
                             rectTransform.rect.width, rectTransform.rect.height);
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        for (int i = 0; i < 4; i++)
        {
            Vector3 corner = corners[i];
            corner.z = Camera.main.transform.position.z;
            if (Camera.main.WorldToViewportPoint(corner).x > 1 || Camera.main.WorldToViewportPoint(corner).y > 1 ||
                Camera.main.WorldToViewportPoint(corner).x < 0 || Camera.main.WorldToViewportPoint(corner).y < 0)
            {
                return false;
            }
        }
        return true;
    }
}
