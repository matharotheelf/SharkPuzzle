using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScreenFade : MonoBehaviour
{
    public CanvasGroup screenGroup;
    public float fadeDuration = 5f;

    private float fadeStartTime;
    private bool isFading = true;

    void OnEnable()
    {
        fadeStartTime = Time.time;
        isFading = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(isFading)
        {
            float t = (Time.time - fadeStartTime) / fadeDuration;
            screenGroup.alpha = Mathf.Lerp(0, 1, t);
            isFading = t >= 1 ? false : true;
        }
    }
}
