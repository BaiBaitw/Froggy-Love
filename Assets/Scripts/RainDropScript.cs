using UnityEngine;
using System.Collections;

public class RainDropScript : MonoBehaviour
{

    void Start()
    {
        StartCoroutine(DestroySelf());
    }

    IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(0.55f);
        Destroy(gameObject);
    }
}
