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
    [SerializeField] AudioSource sphereAudioSource;
    [SerializeField] float litSoundVolume = 0.15f;
    [SerializeField] float lightingDuration = 10f;
    [SerializeField] float vibrationDistance = 0.1f;
    [SerializeField] float vibrationDuration = 0.3f;
    [SerializeField] Renderer lightRenderer;
    [SerializeField] PlayerScore playerScore;


    private LightState lightState = LightState.Dark;
    private float initialLightTime;

    private void OnTriggerEnter(Collider collider)
    {
        // Light the sphere if the user enters it
        if(collider.gameObject.tag == "Player" && lightState == LightState.Dark)
        {

            lightState = LightState.Lighting;

            // Start lighting visual lerp
            initialLightTime = Time.time;

            // turn up the audio sound to show sphere activated
            sphereAudioSource.volume = litSoundVolume;

            // user gets a point for lighting the sphere
            playerScore.ScorePoint();
        }
    }

    private void lightingUpLerp()
    {
        // Lerp the colour of the sphere to light it up
        float t = (Time.time - initialLightTime) / lightingDuration;

        // Use HSV lerp ysed to imitate the effect of a glowing light
        Color newColor = LerpHSV(initialColor, finalColor, t);
        lightRenderer.material.SetColor("_EmissionColor", newColor);

        lightState = t >= 1 ? LightState.Lit : LightState.Lighting;
    }


    private void vibrateLight()
    {
        // Vibrate sphere using a sin oscillation
        float vDistance = Mathf.Sin(Time.time / vibrationDuration) * vibrationDistance;
        transform.localPosition = vDistance * transform.right;
    }

    private void Update()
    {
        switch (lightState)
        {
            case LightState.Lighting:
                lightingUpLerp();
                vibrateLight();
                break;
            case LightState.Lit:
                vibrateLight();
                break;
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

        // Convert colours to HSV
        Color.RGBToHSV(initial, out initial_h, out initial_s, out initial_v);
        Color.RGBToHSV(final, out final_h, out final_s, out final_v);

        // lerp values and convert back to RGB
        return Color.HSVToRGB
        (
            initial_h + t * (final_h - initial_h),    // H
            initial_s + t * (final_s - initial_s),    // S
            initial_v + t * (final_v - initial_v)    // V
        );
    }
}
