using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameScreen : MonoBehaviour
{
    public GameObject killZone;
    public GameObject pounceZone;

    public void Setup(string screenName)
    {
        // Stop shark interactions once game is over
        killZone.SetActive(false);
        pounceZone.SetActive(false);

        // Activate final screen
        gameObject.transform.Find(screenName).gameObject.SetActive(true);

        // Make cursor visible and movable for final screen
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Restart()
    {
        // Reload scene to restart the game
        SceneManager.LoadScene("SharkPuzzle");
    }
}
