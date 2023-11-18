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
        killZone.SetActive(false);
        pounceZone.SetActive(false);

        gameObject.transform.Find(screenName).gameObject.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Restart()
    {
        SceneManager.LoadScene("SharkPuzzle");
    }
}
