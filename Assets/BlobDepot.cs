using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BlobDepot : MonoBehaviour
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
        Text.text = Score.ToString();
        
        if(Score >= nextScore)
        {
            WinLevel();
        }
    }

    public void WinLevel()
    {
        Debug.Log("SHOW ME THAT SHOP");
        nextScoreIndex++;
        if(nextScoreIndex < BreakPoints.Count)
        {
            nextScore = BreakPoints[nextScoreIndex];
        }
        else
        {
            Debug.Log("WIN GAME");
        }
    }
}
