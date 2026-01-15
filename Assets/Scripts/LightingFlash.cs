using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;

public class LightningFlash : MonoBehaviour
{
    public float maxIntensity = 5f;
    public Vector2 durationRange = new Vector2(0.05f, 0.2f);
    public Vector2 delayRange = new Vector2(1.0f, 5.0f);
    public Color flashColor = Color.white;
    public AudioSource myAudio;
    public AudioClip[] LightingSound;

    private Light2D targetLight;
    private float originalIntensity;
    private Color originalColor;
    private bool isFlashing = false;

    void Start()
    {
        targetLight = GetComponent<Light2D>();

        if (targetLight == null)
        {
            Debug.LogError("TargetLight is null");
            enabled = false;
            return;
        }

        originalIntensity = targetLight.intensity;
        originalColor = targetLight.color;
        StartCoroutine(LightningCycle());
    }

    IEnumerator LightningCycle()
    {
        while (true)
        {

            float delay = Random.Range(delayRange.x, delayRange.y);
            yield return new WaitForSeconds(delay);
            if (!isFlashing)
            {
                StartCoroutine(Flash());
            }
        }
    }

    IEnumerator Flash()
    {
        isFlashing = true;
        targetLight.color = flashColor;
        targetLight.intensity = maxIntensity;
        if (myAudio != null) { 
            myAudio.PlayOneShot(LightingSound[Random.Range(0, LightingSound.Length)]);
        }

        float flashDuration = Random.Range(durationRange.x, durationRange.y);
        yield return new WaitForSeconds(flashDuration);

        float startTime = Time.time;
        float fadeTime = 0.1f; 
        while (Time.time < startTime + fadeTime)
        {
            float t = (Time.time - startTime) / fadeTime;
            targetLight.intensity = Mathf.Lerp(maxIntensity, originalIntensity, t);
            yield return null;
        }

        targetLight.intensity = originalIntensity;
        targetLight.color = originalColor;

        isFlashing = false;
    }
    void OnDisable()
    {
        StopAllCoroutines();
    }
}