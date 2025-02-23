using UnityEngine;
using UnityEngine.UI;

public class TreeOfLifeButton : MonoBehaviour
{
    public Button Button;
    public TreeOfLife TreeOfLife;
    public UnitType Type;
    public int Cost;

    public void Start()
    {
        Button.onClick.AddListener(OnButtonClicked);
    }

    public void OnButtonClicked()
    {
        if (ScoreManager.instance.CanAfford(Cost))
        {
            ScoreManager.instance.Buy(Cost);
            TreeOfLife.StartGrowingSeed(Type);
        }
    }
}
