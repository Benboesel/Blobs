using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    private UIElement currentUI;
    private UIElement currentTarget;


    public void ShowUI(UIElement element, Transform anchor)
    {
        if (currentUI != null)
        {
            Destroy(currentUI);
        }

        currentTarget = element;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(anchor.position);
        currentUI.transform.position = screenPos;
    }

    public void HideUI()
    {
        if (currentUI != null)
        {
            Destroy(currentUI);
            currentUI = null;
        }
    }
}
