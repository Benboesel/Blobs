using DG.Tweening;
using UnityEngine;

public class Slime : Pickupable
{
    public MeshRenderer Mesh;
    public Color HoverColor;
    public Color DefaultColor;

    public override void Hover()
    {
        base.Hover();
        Mesh.material.DOColor(HoverColor, 0.25f).SetEase(Ease.OutQuad);
    }

    public override void UnHover()
    {
        base.UnHover();
        Mesh.material.DOColor(DefaultColor, 0.25f).SetEase(Ease.OutQuad);
    }
}
