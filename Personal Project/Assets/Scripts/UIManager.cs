using System.Collections;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private float titleSpinSpeed = 2;
    [SerializeField] private GameObject titleScreen;
    [SerializeField] private GameObject pauseScreen;    
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject highScoreScreen;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private TextMeshProUGUI overText;
    [SerializeField] private TextMeshProUGUI restartText;
    [SerializeField] private Rigidbody tubeRb;    


    private void Start()
    {                
        titleScreen.SetActive(true);

        scoreText.gameObject.SetActive(false);
        livesText.gameObject.SetActive(false);
        pauseScreen.SetActive(false);
        gameOverScreen.SetActive(false);
        highScoreScreen.SetActive(false);

        tubeRb = GameObject.Find("CTRL_spin").GetComponent<Rigidbody>();

    }

    private void Update()
    {      
        if (Input.GetKeyDown(KeyCode.Escape))
        {            
            if (titleScreen.activeSelf || pauseScreen.activeSelf)
            {
                LeaveGame();                                
            }
            else if(highScoreScreen.activeSelf && !GameManager.Instance.highScoreManager.isNameEnter)
            {
                string playerName = "AAA";
                GameManager.Instance.highScoreManager.SetPlayerName(playerName);
                GameManager.Instance.Restart(0);
            }
            else
            {
                TogglePause();
            }
        }

        if (Input.GetKeyDown(KeyCode.P) && !highScoreScreen.activeSelf)
        {
            TogglePause();
        }

        if (Input.anyKeyDown && titleScreen.activeSelf)
        {
            HideTitleScreen();
        }

        if (titleScreen.activeSelf)
        {
            tubeRb.transform.Rotate(Vector3.back * Time.deltaTime * titleSpinSpeed);
        }

        if (GameManager.Instance.highScoreManager.isNameEnter)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GameManager.Instance.Restart(0);
            }
        }

    }

    public void UpdateScore(int score)
    {
        scoreText.text = "score:\n" + score.ToString("D6");

    }

    public void UpdateLives(int lives)
    {
        livesText.text = "lives:\n" + lives.ToString("D3");

        if (lives <= 0)
        {
            gameOverScreen.SetActive(true);
        }

    }

    public void LoadHighScores(bool newHighScore)
    {
        gameOverScreen.SetActive(true);

        if (newHighScore)
        {
            overText.text = "HIGH SCORE";
        }
        else
        {
            overText.text = "GAME OVER";
        }

        StartCoroutine(ShowHighScores(newHighScore));

    }

    private IEnumerator ShowHighScores(bool newHighScore)
    {
        yield return new WaitForSeconds(3);
        gameOverScreen.SetActive(false);
        highScoreScreen.SetActive(true);

        HighScoreManager manager = GameManager.Instance.highScoreManager;
        if(newHighScore)
        {
            manager.EnterName();
        }
        else
        {
            manager.ConfirmName();
        }


    }

    private void HideTitleScreen()
    {
        titleScreen.SetActive(false);
        scoreText.gameObject.SetActive(true);
        livesText.gameObject.SetActive(true);
        GameManager.Instance.StartGame();

    }

    private void TogglePause()
    {
        pauseScreen.SetActive(!pauseScreen.activeSelf);
        GameManager.Instance.PauseGame();

    }      

    private void LeaveGame()
    {
        GameManager.Instance.ExitGame();

    }   


}
