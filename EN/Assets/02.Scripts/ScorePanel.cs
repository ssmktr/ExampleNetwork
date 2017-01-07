using UnityEngine;
using System.Collections;

public class ScorePanel : MonoBehaviour {

    public UILabel Score1Lbl, Score2Lbl;
    public int ScoreLimit = 10;
    int P1_Score = 0;
    int P2_Score = 0;

    private void Start()
    {
        SetScore();
    }

    void SetScore()
    {
        Score1Lbl.text = string.Format("Player1 : {0}", P1_Score);
        Score2Lbl.text = string.Format("Player2 : {0}", P2_Score);
    }

    public void AddScore(int player)
    {
        if (player == 1)
        {
            P1_Score++;
        }
        else if (player == 2)
        {
            P2_Score++;
        }

        if (P1_Score >= ScoreLimit || P2_Score >= ScoreLimit)
        {
            if (P1_Score > P2_Score)
            {
                Debug.Log("Player 1 WIN");
            }
            else if (P1_Score < P2_Score)
            {
                Debug.Log("Player 2 WIN");
            }
            else
            {
                Debug.Log("Players are tied");
            }

            P1_Score = 0;
            P2_Score = 0;
        }

        SetScore();
    }
}
