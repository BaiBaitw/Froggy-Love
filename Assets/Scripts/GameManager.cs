using System.Collections;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class GameManager : MonoBehaviour
{
    public int LevelNumber;
    GameObject hint;
    public GameObject wave;
    public GameObject[] movingStuff;
    public GameObject littleRock;
    public GameObject littleRockDropPosition;
    public GameObject gameOver1;
    public GameObject gameOver2;
    public GameObject gameSuccess;
    public GameObject endSuccess;
    public GameObject rainDrop;
    public AudioSource myAudio;
    public AudioClip waterDrop1;
    public AudioClip waterDrop2;
    public AudioClip waterDrop3;
    public AudioClip frogMatchSound;
    public AudioClip fruitClickSound;
    public AudioSource littleRockRollingAudioSource;
    public bool isGameOver;
    public bool isGameWon = false;
    public float targetZoomSize = 2f; 
    public float smoothSpeed = 5f;   
    public GameObject cam;
    public int littleStoneCount;
    public Button endAgain;
    public Button endMenu;
    public Button endNext;
    public Button endMenu3;
    public Button retry;
    public Button backToMenu;
    public bool isClickingButton = false;
    public bool isRainyLevel = false;
    GameObject wall;
    GameObject startGameButton;
    GameObject redFrog;
    GameObject greenFrog;
    GameObject myRock;
    bool isHolding = false;
    bool isStartPlayingLittleRockSound = false;
    bool littleRockisShaking;

    float rainDropSpawnInterval = 0.03f;
    float rainDropSpwanDelay = 0f;
    float waveRadius = 2f;
    float holdTime = 0;
    GameObject stoneCount;
    TextMeshProUGUI stoneCountText;

    void Start()
    {
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        GameObject[] woodSticks = GameObject.FindGameObjectsWithTag("WoodStick");
        movingStuff = obstacles.Concat(woodSticks).ToArray();
        redFrog = GameObject.Find("RedFrogController");
        greenFrog = GameObject.Find("GreenFrogController");
        wall = GameObject.Find("Wall");
        isGameOver = false;
        stoneCount = GameObject.Find("StoneCount");
        if (stoneCount != null) {
            stoneCountText = stoneCount.GetComponent<TextMeshProUGUI>();
        }
        if (stoneCountText != null) {
            stoneCountText.text = littleStoneCount.ToString();
        }
        if (gameOver1 != null) {gameOver1.SetActive(false);}
        if (gameOver2 != null) { gameOver2.SetActive(false); }
        if (gameSuccess != null) { gameSuccess.SetActive(false); }

        if (endAgain != null) { endAgain.onClick.AddListener(ReloadScene); }
        if (endMenu != null) { endMenu.onClick.AddListener(BackToMenu); }
        if (endNext != null) { endNext.onClick.AddListener(GoNextLevel); }
        if (endMenu3 != null) { endMenu3.onClick.AddListener(BackToMenu); }
        if (retry != null) { retry.onClick.AddListener(ReloadScene); }
        if (backToMenu != null) { backToMenu.onClick.AddListener(BackToMenu); }



        if (isRainyLevel)
        {
            SpawnRainDropRepeatStart();
        }
        hint = GameObject.Find("Hint");
        if (LevelNumber == 1) {
            if (PlayerPrefs.GetInt("Level1Completed") != 1) {
                if (hint != null)
                {
                    StartCoroutine(FadeIn(hint.GetComponent<TextMeshProUGUI>(), 2f));
                    StartCoroutine(CloseHint());
                }
            }
        }
        if (LevelNumber == 2)
        {
            if (PlayerPrefs.GetInt("Level2Completed") != 1)
            {
                if (hint != null)
                {
                    StartCoroutine(FadeIn(hint.GetComponent<TextMeshProUGUI>(), 2f));
                    StartCoroutine(CloseHint());
                }
            }
        }
        if (LevelNumber == 3)
        {
            if (PlayerPrefs.GetInt("Level3Completed") != 1)
            {
                if (hint != null)
                {
                    StartCoroutine(FadeIn(hint.GetComponent<TextMeshProUGUI>(), 2f));
                    StartCoroutine(CloseHint());
                }
            }
        }
    }

    IEnumerator CloseHint() { 
        yield return new WaitForSeconds(5f);
        //if (hint != null) { hint.SetActive(false); }
        if (hint != null) { StartCoroutine(FadeOut(hint.GetComponent<TextMeshProUGUI>(), 3f)); }
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 myMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButton(0) && !isGameOver && !isClickingButton && !SmoothMover.IsInputLocked)
        {
            //Debug.Log("Clicked Left Mouse");
            holdTime += Time.deltaTime;
            if (!isHolding)
            {
                if (littleStoneCount > 0)
                {
                    myRock = Instantiate(littleRock, myMousePos, littleRock.transform.rotation);
                    isStartPlayingLittleRockSound = true;
                }
                else { Lose(2); }
            }
            if (myRock != null)
            {
                myRock.transform.position = myMousePos;
            }
            if (myRock != null && myRock.transform.localScale.x < 0.5f)
            {
                myRock.transform.localScale = new Vector2(0.1f + holdTime / 5, 0.1f + holdTime / 5);
            }
            if (myRock != null && myRock.transform.localScale.x >= 0.5f)
            {
                //Debug.Log("Little Rock shakes");
                littleRockisShaking = true;
            }
            if (myRock != null && littleRockisShaking)
            {
                float t = Mathf.PingPong(Time.time * 10, 1.0f);
                float targetAngle = Mathf.Lerp(-5, 5, t);
                Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
                myRock.transform.rotation = targetRotation;
            }
            isHolding = true;
            if (isStartPlayingLittleRockSound && !isClickingButton)
            {
                littleRockRollingAudioSource.Play();
                isStartPlayingLittleRockSound = false;
            }
        }
        if (SmoothMover.IsInputLocked || isClickingButton) {
            littleRockRollingAudioSource.Stop();
        }

        if (Input.GetMouseButtonUp(0) && !isGameOver && !isClickingButton && littleStoneCount > 0 && !SmoothMover.IsInputLocked)
        {
            if (stoneCountText != null && littleStoneCount > 0) {
                littleStoneCount -= 1;
                stoneCountText.text = littleStoneCount.ToString();
            }
            littleRockRollingAudioSource.Stop();
            bool didCollision;
            littleRockDropPosition.transform.position = myMousePos;
            isHolding = false;
            if (myRock != null) { myRock.transform.localScale = new Vector2(0.1f, 0.1f); }
            Destroy(myRock);
            littleRockDropPosition.transform.position = littleRockDropPosition.GetComponent<LittleRockDropPosition>().GetSafePosition(littleRockDropPosition.transform.position, out didCollision);

            StartCoroutine(WaitLittleRockDrop(didCollision));
        }
    }

    void LateUpdate() {

        if (isGameWon && redFrog != null)
        {
            Vector3 desiredPosition = new Vector3(redFrog.transform.position.x, redFrog.transform.position.y, cam.transform.position.z);
            cam.transform.position = Vector3.Lerp(cam.transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            cam.GetComponent<Camera>().orthographicSize = Mathf.Lerp(cam.GetComponent<Camera>().orthographicSize, targetZoomSize, smoothSpeed * Time.deltaTime);
        }
    }


    IEnumerator PlayLittleRockRollingSound() {

        yield return new WaitForSeconds(littleRockRollingAudioSource.clip.length);
    }

    public void Win() {
        if (!isGameOver) {
            myAudio.PlayOneShot(frogMatchSound, 0.5f);
            isGameWon = true;
            if (wall != null) { wall.transform.localScale = new Vector3(1f, 1f, wall.transform.localScale.z); }
            Debug.Log("GAME WIN!");
            PlayerPrefs.SetInt("Level" + LevelNumber.ToString() + "Completed", 1);
            gameSuccess.SetActive(true);
        }
        StartCoroutine(DelayShowEndNext());
    }

    IEnumerator DelayShowEndNext()
    {
        yield return new WaitForSeconds(2f);
        if (endNext != null) { endNext.gameObject.SetActive(true); }
    }

    public void Lose(int gameOverType)
    {
        if (!isGameWon) {
            isGameOver = true;
            Destroy(myRock);
            isStartPlayingLittleRockSound = false;
            if (gameOverType == 1)
            {
                StartCoroutine(DelayLose());
            }
            if (gameOverType == 2)
            {
                if (gameOver2 != null) { gameOver2.SetActive(true); }
                if (endAgain != null) { endAgain.gameObject.SetActive(true); }
                if (endMenu != null) { endMenu.gameObject.SetActive(true); }
            }
            Debug.Log("GAME OVER!");
        }
        

    }

    IEnumerator DelayLose() { 
        yield return new WaitForSeconds(2f);
        if (gameOver1 != null) { gameOver1.SetActive(true);}
        if (endAgain != null) { endAgain.gameObject.SetActive(true); }
        if (endMenu != null) { endMenu.gameObject.SetActive(true); }
    }

    void ReloadScene() {
        if (fruitClickSound != null) {
            myAudio.PlayOneShot(fruitClickSound,0.5f);
        }
        StartCoroutine(LoadNewScene(SceneManager.GetActiveScene().buildIndex));
    }

    void BackToMenu() {
        if (fruitClickSound != null){
            myAudio.PlayOneShot(fruitClickSound, 0.5f);
        }
        StartCoroutine(LoadNewScene(1));
    }

    public IEnumerator LoadNewScene(int sceneNumber)
    {
        yield return Resources.UnloadUnusedAssets();
        System.GC.Collect();

        yield return new WaitForSeconds(0.5f);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneNumber);
        asyncLoad.allowSceneActivation = false;

        float timer = 0f;
        float minLoadTime = 2f; 

        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        timer = 0f;
        while (timer < minLoadTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        asyncLoad.allowSceneActivation = true;
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    void GoNextLevel() {
        if (SceneManager.GetActiveScene().buildIndex != 4)
        {
            StartCoroutine(LoadNewScene(SceneManager.GetActiveScene().buildIndex + 1));
        }
        else {
            endSuccess.SetActive(true);
            endNext.gameObject.SetActive(false);
            StartCoroutine(DelayShowMenu3());
        }
    }

    IEnumerator DelayShowMenu3() { 
        yield return new WaitForSeconds(3f);
        if (endMenu3 != null) { endMenu3.gameObject.SetActive(true); }
    }

    IEnumerator WaitLittleRockDrop(bool didCollision) {
        if (didCollision) {
            yield return new WaitForSeconds(0.3f);
        }

        int waterDropSoundNumber;
        waterDropSoundNumber = Random.Range(1, 4);

        if (waterDropSoundNumber == 1)
        {
            myAudio.PlayOneShot(waterDrop1);
        }
        if (waterDropSoundNumber == 2)
        {
            myAudio.PlayOneShot(waterDrop2);
        }
        if (waterDropSoundNumber == 3)
        {
            myAudio.PlayOneShot(waterDrop3);
        }


        if (holdTime > 2.8f)
        {
            holdTime = 2.8f;
        }

        if (redFrog != null) {
            redFrog.GetComponent<FrogController>().forceAmount = holdTime;
        }
        if (greenFrog != null) {
            greenFrog.GetComponent<FrogController>().forceAmount = holdTime;
        }

        for (int i = 0; i < movingStuff.Length; i++)
        {
            movingStuff[i].GetComponent<ObstacleScript>().forceAmount = holdTime / 2;
        }

        GameObject newObject = Instantiate(wave, littleRockDropPosition.transform.position, wave.transform.rotation);
        newObject.transform.localScale = new Vector2(0.1f + 1 * holdTime / 2, 0.1f + 1 * holdTime / 2);

        waveRadius += holdTime;

        if (redFrog != null) {
            if (Vector2.Distance(redFrog.transform.position, littleRockDropPosition.transform.position) < waveRadius)
            {
                redFrog.GetComponent<FrogController>().MakeWaveMove(littleRockDropPosition.transform.position);
            }
        }

        if (greenFrog != null) {
            if (Vector2.Distance(greenFrog.transform.position, littleRockDropPosition.transform.position) < waveRadius)
            {
                greenFrog.GetComponent<FrogController>().MakeWaveMove(littleRockDropPosition.transform.position);
            }
        }

        for (int i = 0; i < movingStuff.Length; i++)
        {
            GameObject currentObject = movingStuff[i];
            if (Vector2.Distance(currentObject.transform.position, littleRockDropPosition.transform.position) < waveRadius)
            {
                currentObject.GetComponent<ObstacleScript>().MakeWaveMove(littleRockDropPosition.transform.position);
            }
        }

        waveRadius = 2f;
        littleRockisShaking = false;
        holdTime = 0f;
    }

    void SpawnRainDropRepeatStart() {
        InvokeRepeating("SpwanRainDrop", rainDropSpwanDelay, rainDropSpawnInterval);
    }

    void SpwanRainDrop() {
        float xPos = Random.Range(-13f, 13f);
        float yPos = Random.Range(-5f, 5f);
        Instantiate(rainDrop, new Vector3(xPos, yPos, rainDrop.transform.position.z), rainDrop.transform.rotation);
    }

    IEnumerator FadeOut(TMP_Text textComponent, float duration)
    {
        float startAlpha = 1f;
        float endAlpha = 0f;
        float elapsedTime = 0f;

        Color color = textComponent.color;
        color.a = startAlpha;
        textComponent.color = color; 

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            color.a = Mathf.Lerp(startAlpha, endAlpha, t);
            textComponent.color = color;

            yield return null; 
        }
        color.a = endAlpha;
        textComponent.color = color;
    }

    IEnumerator FadeIn(TMP_Text textComponent, float duration)
    {
        float startAlpha = 0f;
        float endAlpha = 1f;
        float elapsedTime = 0f;

        Color color = textComponent.color;
        color.a = startAlpha;
        textComponent.color = color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            color.a = Mathf.Lerp(startAlpha, endAlpha, t);
            textComponent.color = color;

            yield return null;
        }
        color.a = endAlpha;
        textComponent.color = color;
    }

}
