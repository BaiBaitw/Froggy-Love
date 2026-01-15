using System.Collections;
using UnityEngine;

public class FrogController : MonoBehaviour
{
    Rigidbody2D myRB;
    public float forceAmount = 0.3f;
    public bool isAloneLeaf;
    GameObject gameManager;
    GameObject loveAnim;
    Animator frogAnim;
    float waveDelayTime = 0.3f;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameObject.Find("GameManager");
        myRB = GetComponent<Rigidbody2D>();
        frogAnim = GetComponent<Animator>();
        if (!isAloneLeaf) {
            loveAnim = GameObject.Find("LoveAnim");
        }
        if (loveAnim != null) {
            loveAnim.SetActive(false);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (!isAloneLeaf && loveAnim != null) {
            loveAnim.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y+0.5f,loveAnim.transform.position.z);
        }
    }

    public void MakeWaveMove(Vector2 pos)
    {
        StartCoroutine(Delay(pos));
    }

    IEnumerator Delay(Vector2 pos) { 
        yield return new WaitForSeconds(waveDelayTime);
        Vector2 myPosition2D = gameObject.transform.position;
        Vector2 direction = myPosition2D - pos;
        Vector2 normalizedDirection = direction.normalized;

        //Debug.Log("ForceAmount is" + forceAmount);

        myRB.AddForce(normalizedDirection * forceAmount, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Frog") && !gameManager.GetComponent<GameManager>().isGameOver) {
            gameManager.GetComponent<GameManager>().Win();
            if (!isAloneLeaf)
            {
                frogAnim.SetBool("win_bool", true);
                loveAnim.SetActive(true);
            }
            else { 
                frogAnim.SetBool("leaf_bool", true);
            }

        }
    }

    public void FrogEatenBySnake() {
        StartCoroutine(EatenDelay());
    }

    IEnumerator EatenDelay() {
        yield return new WaitForSeconds(0.6f);
        frogAnim.SetBool("leaf_bool", true);
    }
}
