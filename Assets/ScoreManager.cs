using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : Singleton<ScoreManager>
{
    public int Score;
    public TextMeshProUGUI Text;
    public List<int> BreakPoints;
    private int nextScore;
    private int nextScoreIndex;

    public void Start()
    {
        nextScore = BreakPoints[0];
    }
    public bool CanAfford(int cost)
    {
        return cost <= Score;
    }

    public void Buy(int cost)
    {
        if (CanAfford(cost))
        {
            ReduceScore(cost);
        }

        else
        {
            Debug.Log("CANT BUY, not enough money");
        }
    }

    public void UpdateText()
    {
        Text.text = Score.ToString();
    }

    public void ReduceScore(int amount)
    {
        Score -= amount;
        UpdateText();
    }

    public void IncreaseScore(int amount)
    {
        Score += amount;
        UpdateText();
    }

    public void WinLevel()
    {
        Debug.Log("SHOW ME THAT SHOP");
        nextScoreIndex++;
        if (nextScoreIndex < BreakPoints.Count)
        {
            nextScore = BreakPoints[nextScoreIndex];
        }
        else
        {
            Debug.Log("WIN GAME");
        }
    }
}
