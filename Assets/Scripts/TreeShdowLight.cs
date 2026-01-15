using UnityEngine;

public class TreeShdowLight : MonoBehaviour
{
    public Vector3 startPosition;
    public float amplitude = 1.0f;
    public float speed = 2.0f;
    [Range(0.01f, 0.5f)]
    public float smoothFactor = 0.05f;

    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float shakeValue = Mathf.Sin(Time.time * speed);

        Vector3 targetPosition = startPosition;
        targetPosition.y += shakeValue * amplitude;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothFactor
        );
    }
}
