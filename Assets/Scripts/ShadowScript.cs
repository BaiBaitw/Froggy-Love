using UnityEngine;

public class ShadowScript : MonoBehaviour
{
    public GameObject fatherObject;
    public float shadowDepth = 0.1f;

    void LateUpdate()
    {
        gameObject.transform.localPosition = new Vector2(fatherObject.transform.localPosition.x + shadowDepth, fatherObject.transform.localPosition.y - shadowDepth);
        gameObject.transform.rotation = fatherObject.transform.rotation;
    }
}
