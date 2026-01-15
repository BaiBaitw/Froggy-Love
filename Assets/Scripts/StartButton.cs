using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

//using UnityEngine.UIElements;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    public AudioSource myAudio;
    public AudioClip frogSound2;
    public float moveDuration = 1.5f;
    public GameObject gameManager;

    void Start()
    {
        PlayerPrefs.DeleteAll();
    }
    public void StartGame()
    {
        gameManager.GetComponent<GameManager>().isClickingButton = true;
        StartCoroutine(DelayStart());
        myAudio.PlayOneShot(frogSound2,5.0f);
    }

    IEnumerator DelayStart() {
        yield return new WaitForSeconds(1f);
        //UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        StartCoroutine(LoadNewScene("ChooseLevelScene"));
    }

    public IEnumerator LoadNewScene(string sceneName)
    {
        yield return Resources.UnloadUnusedAssets();
        System.GC.Collect();

        yield return new WaitForSeconds(1f);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        //while (asyncLoad.progress < 0.9f)
        //{
        //    yield return null;
        //}

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

    
}
