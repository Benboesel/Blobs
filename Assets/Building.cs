using System.Collections.Generic;
using UnityEngine;


public interface ISelectable
{
    void OnSelected();
    void OnDeselected();
    void OnHovered();
    void OnUnhovered();
    Transform GetTransform();
}


public class Building : MonoBehaviour, ISelectable
{
    public List<Renderer> MeshRenderers;
    private Color originalColor;
    public Color selectedColor;
    public Color hoverColor;

    private bool isSelected = false;

    private void Awake()
    {
        originalColor = MeshRenderers[0].material.color;
    }

    public virtual void OnSelected()
    {
        isSelected = true;
        SetColor(selectedColor);
    }

    public virtual void OnDeselected()
    {
        isSelected = false;
        SetColor(originalColor);
    }

    public void OnHovered()
    {
        if (!isSelected) // Only change if not selected
        {
            SetColor(hoverColor);

        }
    }

    public void OnUnhovered()
    {
        if (!isSelected) // Only revert if not selected
        {
            SetColor(originalColor);
        }
    }

    public void SetColor(Color color)
    {
        foreach (MeshRenderer mesh in MeshRenderers)
        {
            mesh.material.color = color;
        }
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
