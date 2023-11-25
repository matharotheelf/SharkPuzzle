using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScreenFade : MonoBehaviour
{
    [SerializeField] CanvasGroup screenGroup;
    [SerializeField] float fadeDuration = 5f;

    private float fadeStartTime;
    private bool isFading = true;

    void OnEnable()
    {
        // Start screen fade in
        fadeStartTime = Time.time;
        isFading = true;
    }

    void Update()
    {
        // Screen opacity lerp to 1 during fade
        if(isFading)
        {
            float t = (Time.time - fadeStartTime) / fadeDuration;
            screenGroup.alpha = Mathf.Lerp(0, 1, t);
            isFading = t >= 1 ? false : true;
        }
    }
}
