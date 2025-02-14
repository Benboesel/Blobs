using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class GameManager : Singleton<GameManager>
{
    public float PlayAreaSize;
    public BlobDepot Depot;

    public bool CanAfford(int cost)
    {
        return cost <= Depot.Score;
    }

    public void Buy(int cost)
    {
        if (CanAfford(cost))
        {
            Depot.ReduceScore(cost);
        }

        else
        {
            Debug.Log("CANT BUY, not enough money");
        }
    }
}
