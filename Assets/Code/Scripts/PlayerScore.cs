using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScore : MonoBehaviour
{
    public int score = 0;
    public int winningScore = 14;
    [SerializeField] GameScreen victoryScreen;

    public void ScorePoint()
    {
        score += 1;
        if (score == winningScore)
        {
            victoryScreen.Setup("VictoryScreen");
        }
    }
}
