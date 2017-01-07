using UnityEngine;
using System.Collections;

public class Goal : MonoBehaviour {

    public int Player = 1;


    public ScorePanel _ScorePanel;

    public void GetPoint()
    {
        _ScorePanel.AddScore(Player);
    }
}
