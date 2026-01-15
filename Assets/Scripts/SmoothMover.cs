using UnityEngine;
using UnityEngine.UI; 
using System.Collections;

public class SmoothMover : MonoBehaviour
{
    public static bool IsInputLocked { get; private set; } = false;
    public AudioSource myAudio;
    public AudioClip HitLeavesSound1;

    public Vector3 targetLocalPosition = new Vector3(0f, -247f, 0f); 
    public float moveSpeed = 5f;

    private Vector3 initialLocalPosition;
    private Vector3 initialPosition;
    private bool isMovedToTarget = false; 

    private RectTransform rectTransform;
    private Transform objectTransform;

    void Awake()
    {
        IsInputLocked = false;
        rectTransform = GetComponent<RectTransform>();

        if (rectTransform != null)
        {
            initialPosition = rectTransform.anchoredPosition;
        }
        else
        {
            objectTransform = transform;
            initialPosition = objectTransform.localPosition;
        }

        SetPosition(initialPosition);
    }

   
    public void ToggleMove()
    {
        myAudio.PlayOneShot(HitLeavesSound1, 2.0f);
        if (!IsInputLocked) 
        {
            IsInputLocked = true;
        }

        if (isMovedToTarget)
        {
            StartSmoothMove(initialPosition);
        }
        else
        {
            Vector3 destination = initialPosition + targetLocalPosition;
            StartSmoothMove(destination);
        }
        isMovedToTarget = !isMovedToTarget;
    }


    private void StartSmoothMove(Vector3 target)
    {
        StopAllCoroutines();
        StartCoroutine(MoveToPosition(target));
    }


    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        Vector3 currentPosition = GetCurrentPosition();
        while (Vector3.Distance(currentPosition, targetPosition) > 0.01f)
        {
            currentPosition = Vector3.Lerp(currentPosition, targetPosition, Time.deltaTime * moveSpeed);
            SetPosition(currentPosition);

            yield return null;
        }
        SetPosition(targetPosition);
        IsInputLocked = false;
    }

    private Vector3 GetCurrentPosition()
    {
        return rectTransform != null ? rectTransform.anchoredPosition : objectTransform.localPosition;
    }

    private void SetPosition(Vector3 newPosition)
    {
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = newPosition;
        }
        else if (objectTransform != null)
        {
            objectTransform.localPosition = newPosition;
        }
    }
}