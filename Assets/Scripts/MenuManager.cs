using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    GameObject gameManager;
    GameObject menu1_Ready;
    GameObject menu2_Ready;
    GameObject menu3_Ready;
    GameObject menu2_Unlock;
    GameObject menu3_Unlock;
    GameObject level2;
    GameObject level3;
    AudioSource myAudio;
    public AudioClip[] FrogSound;
    public bool isMenuManager = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameObject.Find("GameManager");
        menu1_Ready = GameObject.Find("Menu1_Ready");
        menu2_Ready = GameObject.Find("Menu2_Ready");
        menu3_Ready = GameObject.Find("Menu3_Ready");
        menu2_Unlock = GameObject.Find("Menu2_Unlock");
        menu3_Unlock = GameObject.Find("Menu3_Unlock");
        level2 = GameObject.Find("Level2");
        level3 = GameObject.Find("Level3");
        myAudio = GetComponent<AudioSource>();
        if (isMenuManager) {
            if (PlayerPrefs.GetInt("Level1Completed") == 1)
            {
                menu1_Ready.SetActive(false);
                menu2_Unlock.SetActive(false);
                level2.GetComponent<TextMeshProUGUI>().color = Color.white;
            }
            if (PlayerPrefs.GetInt("Level2Completed") == 1)
            {
                menu2_Ready.SetActive(false);
                menu3_Unlock.SetActive(false);
                level3.GetComponent<TextMeshProUGUI>().color = Color.white;
            }
            if (PlayerPrefs.GetInt("Level3Completed") == 1)
            {
                menu3_Ready.SetActive(false);
            }
        }
    }
    [ContextMenu("win level1")]
    void WinLevel1() {
    PlayerPrefs.SetInt("Level1Completed", 1);
    }

    [ContextMenu("win level2")]
    void WinLevel2()
    {
        PlayerPrefs.SetInt("Level2Completed", 1);
    }

    [ContextMenu("win level3")]
    void WinLevel3()
    {
        PlayerPrefs.SetInt("Level3Completed", 1);
    }

    [ContextMenu("Delete Data")]
    void DeleteData()
    {
        PlayerPrefs.DeleteAll();
    }

    public void PlayFrogSound() {
        if (FrogSound.Length != 0) {
            myAudio.PlayOneShot(FrogSound[Random.Range(0, FrogSound.Length)], 5.0f);
        }
    }

    // Update is called once per frame

    public void LoadLevel1Scene() {
        gameManager.GetComponent<GameManager>().isClickingButton = true;
        PlayFrogSound();
        //StartCoroutine(DelayLoadScene(2));
        StartCoroutine(LoadNewScene("Level1Scene"));
    }

    public void LoadLevel2Scene(){
        gameManager.GetComponent<GameManager>().isClickingButton = true;
        PlayFrogSound();
        //StartCoroutine(DelayLoadScene(3));
        StartCoroutine(LoadNewScene("Level2Scene"));
    }
    public void LoadLevel3Scene(){
        gameManager.GetComponent<GameManager>().isClickingButton = true;
        PlayFrogSound();
        //StartCoroutine(DelayLoadScene(4));
        StartCoroutine(LoadNewScene("Level3Scene"));
    }

    //IEnumerator DelayLoadScene(int sceneNumber) {
    //    yield return new WaitForSeconds(1f);
    //    SceneManager.LoadScene(sceneNumber);
    //}

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

