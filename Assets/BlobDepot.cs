using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BlobDepot : Singleton<BlobDepot>
{
    public TextMeshProUGUI Text;
    public List<int> BreakPoints;
    public int Score;
    private int nextScore;
    private int nextScoreIndex;

    public void Start()
    {
        nextScore = BreakPoints[0];
    }

    public void AddBlob(Transform blob)
    {
        Score++;
        Destroy(blob.gameObject);
        UpdateText();
        if (Score >= nextScore)
        {
            WinLevel();
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
