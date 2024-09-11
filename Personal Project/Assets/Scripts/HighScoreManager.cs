using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HighScoreManager : MonoBehaviour
{
    public List<int> highScores { get; private set; } = new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    public List<string> highScoresName { get; private set; } = new List<string>() { "AAA", "AAA", "AAA", "AAA", "AAA", "AAA", "AAA", "AAA", "AAA", "AAA" };
    public int rank { get; set; } = 666;
    public bool isNameEnter { get; private set; } = false;

    [SerializeField] private GameObject scoreTitle;
    [SerializeField] private GameObject scoreTextPrefab;
    [SerializeField] private TextMeshProUGUI restartText;
    [SerializeField] private TMP_InputField playerInput;

    private List<int> scores = new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }; //Top 10


    private void ClearScores()
    {
        for (int i = scoreTitle.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(scoreTitle.transform.GetChild(i).gameObject);
        }

    }

    public void InitializeHighScore()
    {
        if(GameManager.Instance != null)
        {
            ClearScores();

            for (int i = 0; i < scores.Count; i++)
            {
                //Debug.Log("score: " + i);
                Vector3 posOffset = scoreTitle.transform.position - new Vector3(0, i * 60 + 100, 0);
                GameObject scorePrefab = Instantiate(scoreTextPrefab, posOffset, scoreTextPrefab.transform.rotation, scoreTitle.transform);
                scorePrefab.GetComponent<TextMeshProUGUI>().text = highScoresName[i] + 
                                                                   ".........." + 
                                                                   highScores[i].ToString("D6");

                if(i == rank) 
                {
                    scorePrefab.GetComponent<TextBlink>().isActive = true;
                }
            }

        }
     
    }       

    public void EnterName()
    {
        InitializeHighScore();
        playerInput.Select();        

    }

    public void ConfirmName()
    {
        isNameEnter = true;
        string playerName;
        if (playerInput.text == "" || playerInput.text == "   " )
        {
            playerName = "AAA";
        }
        else
        {

            playerName = playerInput.text;
        }
        playerInput.gameObject.SetActive(false);

        SetPlayerName(playerName);
        GameManager.Instance.SaveGame();

        restartText.text = "Press Space to Restart";
        rank = 666; //Reset and Stop blinking
        InitializeHighScore();

    }

    public void SetPlayerName(string playerName)
    {
        highScoresName[rank] = playerName;
        GameManager.Instance.SaveGame();
    }

    public void SetHighScores(List<int> scores, List<string> scoresName)
    {
        highScores = scores;
        highScoresName = scoresName;

    }
   

}
