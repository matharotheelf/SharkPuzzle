using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSphere : MonoBehaviour
{
    enum LightState
    {
        Dark,
        Lighting,
        Lit
    }

    [SerializeField] Color initialColor = Color.HSVToRGB(0, 60, 20);
    [SerializeField] Color finalColor = Color.HSVToRGB(60, 60, 100);
    [SerializeField] float lightingDuration = 10f;
    [SerializeField] Renderer lightRenderer;
    [SerializeField] PlayerScore playerScore;


    private LightState lightState = LightState.Dark;
    private float initialLightTime;


    private void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.tag == "Player" && lightState == LightState.Dark)
        {

            lightState = LightState.Lighting;
            initialLightTime = Time.time;
            playerScore.ScorePoint();
        }
    }

    private void Update()
    {
        if (lightState == LightState.Lighting)
        {
            float t = (Time.time - initialLightTime) / lightingDuration;
            Color newColor = LerpHSV(initialColor, finalColor, t);
            lightRenderer.material.SetColor("_EmissionColor", newColor);

            lightState = t >= 1 ? LightState.Lit : LightState.Lighting;
        }
        
    }

    private static Color LerpHSV(Color initial, Color final, float t)
    {
        float initial_h;
        float initial_s;
        float initial_v;
        float final_h;
        float final_s;
        float final_v;

        Color.RGBToHSV(initial, out initial_h, out initial_s, out initial_v);
        Color.RGBToHSV(final, out final_h, out final_s, out final_v);

        return Color.HSVToRGB
        (
            initial_h + t * (final_h - initial_h),    // H
            initial_s + t * (final_s - initial_s),    // S
            initial_v + t * (final_v - initial_v)    // V
        );
    }
}
