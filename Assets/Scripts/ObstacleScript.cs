using UnityEngine;
using System.Collections;

public class ObstacleScript : MonoBehaviour
{
    Rigidbody2D myRB;
    public float forceAmount = 0.3f;
    float waveDelayTime = 0.3f;

    void Start()
    {
        myRB = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MakeWaveMove(Vector2 pos)
    {
        StartCoroutine(Delay(pos));
    }

    IEnumerator Delay(Vector2 pos)
    {
        yield return new WaitForSeconds(waveDelayTime);
        Vector2 myPosition2D = gameObject.transform.position;
        Vector2 direction = myPosition2D - pos;
        Vector2 normalizedDirection = direction.normalized;

        if (myRB != null) {
            myRB.AddForce(normalizedDirection * forceAmount, ForceMode2D.Impulse);
        }
    }
}
