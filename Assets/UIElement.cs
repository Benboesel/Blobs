using UnityEngine;
using DG.Tweening;

public class UIElement : MonoBehaviour
{
    public CanvasGroup CanvasGroup;
    public RectTransform Rect;
    public Canvas Canvas;

    public void Awake()
    {
        Canvas = GetComponentInParent<Canvas>();
        Rect = GetComponent<RectTransform>();
        CanvasGroup = GetComponent<CanvasGroup>();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        CanvasGroup.DOFade(1f, 0.25f);
    }

    public void Hide()
    {
        CanvasGroup.DOFade(0f, 0.25f).OnComplete(() => gameObject.SetActive(false));
    }

    public void HideImmediate()
    {
        CanvasGroup.alpha = 0;
        gameObject.SetActive(false);
    }

    public void SetPosition(Vector3 worldPosition, float verticalOffset = 0f)
    {
        // Convert world position to screen space
        worldPosition.y += verticalOffset;
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

        // Convert screen position to local position in the UI's RectTransform
        RectTransform canvasRect = Canvas.GetComponent<RectTransform>();
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPosition, null, out localPoint);

        localPoint.y += Rect.rect.height;
        // Apply the anchored position correctly
        Rect.anchoredPosition = localPoint;
    }
}
