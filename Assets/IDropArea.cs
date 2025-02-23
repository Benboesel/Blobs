using UnityEngine;

public interface IDropArea
{
    void Hover();
    void Unhover();
    void Accept(Pickupable item);
    float DropAreaHeight();
    public Transform GetTransform();
}
