using UnityEngine;

public class FishMove : MonoBehaviour
{
    float speed = 2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition += new Vector3(0f, 1f,0f) * Time.deltaTime * speed;
    }
}
