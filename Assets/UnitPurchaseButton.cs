using UnityEngine;
using UnityEngine.UI;

public class UnitPurchaseButton : MonoBehaviour
{
    private Button button;
    public int Cost;
    public Transform SpawnPosition;
    public UnitType Type;

    public void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ButtonClicked);
    }

    public void ButtonClicked()
    {
        if (BlobDepot.instance.Score > Cost)
        {
            UnitManager.instance.SpawnUnit(Type, SpawnPosition.position);
            BlobDepot.instance.ReduceScore(Cost);
        }
    }
}
