using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class LittleRockDropPosition : MonoBehaviour
{
    public bool isTriggering = false;
    public AudioSource myAudio;
    public AudioClip[] HitLeavesSound;
    public AudioClip[] HitWoodStickSound;
    public AudioClip[] HitRockSound;
    public AudioClip[] FrogSound;
    public AudioClip snakeSound;
    public GameObject speedLineController;
    GameObject speedLine;
    Animator speedLineAnim;
    int fourQuadrant;
    float randomX = 0;
    float randomY = 0;

    void Start()
    {
        speedLine = GameObject.Find("SpeedLine");
        speedLine.GetComponent<SpriteRenderer>().color = new Color(speedLine.GetComponent<SpriteRenderer>().color.r, speedLine.GetComponent<SpriteRenderer>().color.g, speedLine.GetComponent<SpriteRenderer>().color.b,0f);
        speedLineAnim = speedLine.GetComponent<Animator>();
    }

    public Vector2 GetSafePosition(Vector2 startPos,out bool hadCollision)
    {
        Vector2 checkPos = startPos;
        int maxAttempts = 20;
        int currentAttempt = 0;
        bool positionFound = false;
        float checkRadius = 0.2f;

        hadCollision = false;

        while (!positionFound && currentAttempt < maxAttempts)
        {
            Collider2D hit = Physics2D.OverlapCircle(checkPos, checkRadius);

            if (hit != null && (hit.CompareTag("Obstacle") || hit.CompareTag("WoodStick") || hit.CompareTag("Rock") || hit.CompareTag("Frog") || hit.CompareTag("Snake")))
            {
                if (currentAttempt == 0) { 
                    hadCollision = true;
                    if (hit.CompareTag("Obstacle")) {
                        int randomIndex = Random.Range(0, HitLeavesSound.Length);
                        myAudio.PlayOneShot(HitLeavesSound[randomIndex], 2.0f);
                    }
                    if (hit.CompareTag("WoodStick")) {
                        int randomIndex = Random.Range(0, HitWoodStickSound.Length);
                        myAudio.PlayOneShot(HitWoodStickSound[randomIndex], 1.0f);
                    }
                    if (hit.CompareTag("Rock")) {
                        int randomIndex = Random.Range(0, HitRockSound.Length);
                        myAudio.PlayOneShot(HitRockSound[randomIndex], 1.0f);
                    }
                    if (hit.CompareTag("Frog")) {
                        int randomIndex = Random.Range(0, FrogSound.Length);
                        myAudio.PlayOneShot(FrogSound[randomIndex], 3.0f);
                    }
                    if (hit.CompareTag("Snake")) {
                        myAudio.PlayOneShot(snakeSound, 1.0f);
                    }
                }
                if (!isTriggering)
                {
                    DecideQuadrant();
                }
                isTriggering = true;
                if (fourQuadrant == 1)
                {
                    randomX = Random.Range(-0.5f, 0f);
                    randomY = Random.Range(0f, 0.5f);
                }
                if (fourQuadrant == 2)
                {
                    randomX = Random.Range(0f, 0.5f);
                    randomY = Random.Range(0f, 0.5f);
                }
                if (fourQuadrant == 3)
                {
                    randomX = Random.Range(-0.5f, 0f);
                    randomY = Random.Range(-0.5f, 0f);
                }
                if (fourQuadrant == 4)
                {
                    randomX = Random.Range(0f, 0.5f);
                    randomY = Random.Range(-0.5f, 0f);
                }

                checkPos += new Vector2(randomX, randomY);
                currentAttempt++;
            }
            else
            {
                speedLineController.transform.position = startPos;
                Vector2 direction = (checkPos - startPos).normalized;
                speedLineController.transform.right = direction;
                if (Vector2.Distance(checkPos, startPos) > 1.5f) {
                    TriggerSpeedLineEffect(speedLine);
                }
                positionFound = true;
                isTriggering = false;
            }
        }
        return checkPos;
    }

    void DecideQuadrant() {
        fourQuadrant = Random.Range(1, 5);
    }

    IEnumerator FadeIn(SpriteRenderer renderer, float duration)
    {
        speedLineAnim.SetTrigger("SpeedLine_trig");
        float startAlpha = 0f;
        float endAlpha = 1f;
        float elapsedTime = 0f;
        Color color = renderer.color;


        color.a = startAlpha;
        renderer.color = color;

        while (elapsedTime < duration)
        {

            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            color.a = Mathf.Lerp(startAlpha, endAlpha, t);
            renderer.color = color;
            yield return null;
        }
        color.a = endAlpha;
        renderer.color = color;
    }

    IEnumerator FadeOut(SpriteRenderer renderer, float duration)
    {
        float startAlpha = 1f;
        float endAlpha = 0f;
        float elapsedTime = 0f;
        Color color = renderer.color;


        color.a = startAlpha;
        renderer.color = color;

        while (elapsedTime < duration)
        {

            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            color.a = Mathf.Lerp(startAlpha, endAlpha, t);
            renderer.color = color;
            yield return null;
        }
        color.a = endAlpha;
        renderer.color = color;
    }

    IEnumerator AppearThenVanish(
        GameObject target,
        float fadeInDuration,
        float visibleDuration,
        float fadeOutDuration)
    {
        SpriteRenderer renderer = target.GetComponent<SpriteRenderer>();
        Color c = renderer.color;
        c.a = 0f;
        renderer.color = c;

        yield return StartCoroutine(FadeIn(renderer, fadeInDuration));

        if (visibleDuration > 0f)
        {
            yield return new WaitForSeconds(visibleDuration);
        }

        yield return StartCoroutine(FadeOut(renderer, fadeOutDuration));
    }

    void TriggerSpeedLineEffect(GameObject targetObject)
    {
        float fadeInTime = 0.1f;  
        float holdTime = 0.2f;     
        float fadeOutTime = 0.1f; 

        StartCoroutine(AppearThenVanish(targetObject, fadeInTime, holdTime, fadeOutTime));
    }
}
