using System.Collections;
using UnityEngine;

public class WaveScript : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(DestroySelf());
    }

    IEnumerator DestroySelf() {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
