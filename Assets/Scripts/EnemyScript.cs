using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyScript : MonoBehaviour
{
    public GameObject snake;
    public AudioSource myAudio;
    public AudioClip snakeSound;
    public AudioClip snakeAttackSound;
    public AudioClip[] FrogSound;
    Animator myAnim;
    GameObject redFrog;
    GameObject greenFrog;
    GameObject gameManager;
    float lookAtOffset;
    float rotationSpeed = 1f;
    float warningDistance = 7f;
    float secondWarningDistance = 6f;
    float attackDistance = 5.5f;
    Quaternion ogRotation;
    bool isAimRedFrog = false;
    bool isAimGreenFrog = false;
    bool isSecondWarning = false;

    void Start()
    {
        myAnim = snake.GetComponent<Animator>();
        ogRotation = transform.rotation;
        lookAtOffset = transform.rotation.z - 225f;
        redFrog = GameObject.Find("RedFrogController");
        greenFrog = GameObject.Find("GreenFrogController");
        gameManager = GameObject.Find("GameManager");
    }

    void Update()
    {
        if (!gameManager.GetComponent<GameManager>().isGameWon)
        {
            if (Vector2.Distance(transform.position, redFrog.transform.position) < warningDistance && !isAimRedFrog && !isAimGreenFrog)
            {
                StartCoroutine(StartWaiting());
                StartCoroutine(KeepWaitingDelay());
                isAimRedFrog = true;
            }

            if (isAimRedFrog)
            {
                StartAimTarget(redFrog);
            }

            if (Vector2.Distance(transform.position, redFrog.transform.position) < secondWarningDistance && isAimRedFrog && !isSecondWarning){
                isSecondWarning = true;
            }
            
            if(isAimRedFrog && Vector2.Distance(transform.position, redFrog.transform.position) > secondWarningDistance){
                isSecondWarning = false;
                StopSecondWarning();
                StartCoroutine(CancelSecondWarningDelay());
            }

            if (isSecondWarning) {
                StartCoroutine(KeepSecondWarning());
            }

            if (Vector2.Distance(transform.position, redFrog.transform.position) < attackDistance && isAimRedFrog)
            {
                StartCoroutine(Attack());
                redFrog.GetComponent<FrogController>().FrogEatenBySnake();
            }

            if (Vector2.Distance(transform.position, redFrog.transform.position) > warningDistance && isAimRedFrog)
            {
                ResetRotation();
                StopWaiting();
                StartCoroutine(WaitingBackDelay());
            }

            if (Vector2.Distance(transform.position, greenFrog.transform.position) < warningDistance && !isAimGreenFrog && !isAimRedFrog)
            {
                StartCoroutine(StartWaiting());
                StartCoroutine(KeepWaitingDelay());
                isAimGreenFrog = true;
            }

            if (isAimGreenFrog)
            {
                StartAimTarget(greenFrog);
            }

            if (Vector2.Distance(transform.position, greenFrog.transform.position) < secondWarningDistance && isAimGreenFrog && !isSecondWarning)
            {
                isSecondWarning = true;
            }

            if (isAimGreenFrog && Vector2.Distance(transform.position, greenFrog.transform.position) > secondWarningDistance)
            {
                isSecondWarning = false;
                StopSecondWarning();
                StartCoroutine(CancelSecondWarningDelay());
            }

            if (isSecondWarning)
            {
                StartCoroutine(KeepSecondWarning());
            }


            if (Vector2.Distance(transform.position, greenFrog.transform.position) < attackDistance && isAimGreenFrog)
            {
                StartCoroutine(Attack());
                greenFrog.GetComponent<FrogController>().FrogEatenBySnake();
            }

            if (Vector2.Distance(transform.position, greenFrog.transform.position) > warningDistance && isAimGreenFrog)
            {
                ResetRotation();
                StopWaiting();
                StartCoroutine(WaitingBackDelay());
            }
        }
        if (gameManager.GetComponent<GameManager>().isGameWon)
        {
            ResetRotation();
            StopWaiting();
            StartCoroutine(WaitingBackDelay());
        }
    }

    void StartAimTarget(GameObject target)
    {
        Vector3 direction = target.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle - lookAtOffset);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    void ResetRotation()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, ogRotation, rotationSpeed * Time.deltaTime);
    }

    IEnumerator StartWaiting() {
        if (!isAimRedFrog)
        {
            myAnim.SetTrigger("StartWaiting_trig");
            myAudio.PlayOneShot(snakeSound, 1.0f);
        }
        if (!isAimGreenFrog)
        {
            myAnim.SetTrigger("StartWaiting_trig");
            myAudio.PlayOneShot(snakeSound, 1.0f);
        }
        yield return null;
    }

    IEnumerator KeepSecondWarning(){
        yield return null;
        myAnim.SetBool("SecondWarning_bool", true);
    }
    void StopSecondWarning() {
        myAnim.SetBool("SecondWarning_bool", false);
    }

    IEnumerator CancelSecondWarningDelay() {
        yield return new WaitForSeconds(0.5f);
        isSecondWarning = false;
    }



    IEnumerator KeepWaitingDelay()
    {
        yield return new WaitForSeconds(0.5f);
        myAnim.SetBool("Waiting_bool", true);
    }

    [ContextMenu("Stop Waiting")]
    void StopWaiting()
    {
        myAnim.SetBool("Waiting_bool", false);
    }

    IEnumerator WaitingBackDelay()
    {
        yield return new WaitForSeconds(0.5f);
        myAnim.SetTrigger("WaitingBack_trig");
        isAimGreenFrog = false;
        isAimRedFrog = false;
    }

    IEnumerator Attack()
    {
        if (!gameManager.GetComponent<GameManager>().isGameOver)
        {
            myAnim.SetTrigger("Attack_trig");
            myAnim.SetBool("GotFrog_bool", true);
            gameManager.GetComponent<GameManager>().Lose(1);
            StartCoroutine(AttackSoundDelay());
        }
        yield return null;
    }

    IEnumerator AttackSoundDelay()
    {
        yield return new WaitForSeconds(0.3f);
        myAudio.PlayOneShot(snakeAttackSound, 6f);
        StartCoroutine(FrogDieSoundDelay());
    }

    IEnumerator FrogDieSoundDelay()
    {
        yield return new WaitForSeconds(0.3f);
        myAudio.PlayOneShot(FrogSound[Random.Range(0, FrogSound.Length)], 8.0f);
    }
}
