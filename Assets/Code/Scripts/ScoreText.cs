using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreText : MonoBehaviour
{
    [SerializeField] Text scoreText;
    [SerializeField] PlayerScore playerScore;

    void Start()
    {
        scoreText.text = $"{playerScore.score}/{playerScore.winningScore}";
    }

    void Update()
    {
        scoreText.text = $"{playerScore.score}/{playerScore.winningScore}";
    }
}
