using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool isGameActive { get; private set; }
    public UIManager uiManager { get; private set; }
    public ScoreManager scoreManager { get; private set; }
    public HighScoreManager highScoreManager { get; private set; }
    public PlayerController playerController { get; private set; }
    public SpawnManager spawnManager { get; private set; }

    [SerializeField] GameObject playerMesh;

    private string savePath;


    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }        

    }

    private void OnDestroy()
    {
        Instance = null;
    }

    private void Start()
    {
        uiManager = FindAnyObjectByType<UIManager>();
        scoreManager = FindAnyObjectByType<ScoreManager>();
        highScoreManager = FindAnyObjectByType<HighScoreManager>();
        playerController = FindAnyObjectByType<PlayerController>();
        spawnManager= FindAnyObjectByType<SpawnManager>();

        playerMesh.SetActive(false);
        playerController.gameObject.SetActive(false);

        savePath = Application.persistentDataPath + "/savedata.json";

    }
    
    public void StartGame()
    {
        isGameActive = true;
        playerMesh.SetActive(true);
        playerController.gameObject.SetActive(true);

        spawnManager.StartCoroutine("SpawnObstacle");

    }

    public void PauseGame()
    {
        if (isGameActive)
        {
            Time.timeScale = 0;
            isGameActive = false;
        }
        else
        {
            Time.timeScale = 1;
            isGameActive = true;
        }

    }       

    public void Restart(float time)
    {        
        isGameActive = false;        
        StartCoroutine(RestartGame(time));

    }

    IEnumerator RestartGame(float time)
	{
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif

    }

    //Save System
    [System.Serializable]
    private class SavedData
    {
        public List<int> highScores = new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public List<string> highScoresName = new List<string>();

    }

    public void SaveGame()
    {
        SavedData savedData = new SavedData();

        savedData.highScores = highScoreManager.highScores;
        savedData.highScoresName = highScoreManager.highScoresName;

        string json = JsonUtility.ToJson(savedData);
        File.WriteAllText(savePath, json);

    }

    public void LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);

            SavedData savedData = JsonUtility.FromJson<SavedData>(json);

            highScoreManager.SetHighScores(savedData.highScores, savedData.highScoresName);
        }

    }


}
