using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionSystem : MonoBehaviour
{
    public Camera mainCamera;
    private ISelectable currentSelection;
    private ISelectable currentHover;
    public LayerMask SelectableMask;

    void Update()
    {
        HandleHover();
        HandleSelection();
    }

    private void HandleHover()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return; // Prevent hover if over UI

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, SelectableMask))
        {
            ISelectable hoverTarget = hit.collider.GetComponent<ISelectable>();

            if (hoverTarget != null && hoverTarget != currentHover)
            {
                if (currentHover != null)
                {
                    currentHover.OnUnhovered();
                }
                hoverTarget.OnHovered();
                currentHover = hoverTarget;
            }
        }
        else if (currentHover != null)
        {
            currentHover.OnUnhovered();
            currentHover = null;
        }
    }

    private void HandleSelection()
    {
        if (Input.GetMouseButtonDown(0)) // Left Click to Select
        {
            if (EventSystem.current.IsPointerOverGameObject()) return; // Prevent hover if over UI

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, SelectableMask))
            {
                ISelectable selectable = hit.collider.GetComponent<ISelectable>();
                if (selectable != null)
                {
                    Select(selectable);
                }
                else
                {
                    Deselect();
                }
            }
            else
            {
                Deselect();
            }
        }
    }

    private void Select(ISelectable newSelection)
    {
        if (currentSelection != null && newSelection != currentSelection)
        {
            currentSelection.OnDeselected();
        }

        currentSelection = newSelection;
        currentSelection.OnSelected();
    }

    private void Deselect()
    {
        if (currentSelection != null)
        {
            currentSelection.OnDeselected();
            currentSelection = null;
        }
    }
}
