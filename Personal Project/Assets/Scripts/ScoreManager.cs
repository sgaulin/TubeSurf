using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public int lives { get; private set; }
    private int score;
        


    private void Start()
    {        
        score = 0;
        lives = 3;
        UpdateUI();

    }

    public void AddPoints(int points)
    {
        score += points;
        UpdateUI();

    }

    public void SubstractLife()
    {
        lives --;
        UpdateUI();

        if (lives <= 0)
        {
            GameOver();
        }

    }

    private void UpdateUI()
    {
        GameManager.Instance.uiManager.UpdateScore(score);
        GameManager.Instance.uiManager.UpdateLives(lives);

    }

    private void GameOver()
    {        
        if (GameManager.Instance != null)
        {
            //Check for new highscore
            GameManager.Instance.LoadGame();

            if(score >= GameManager.Instance.highScoreManager.highScores[9])
            {
                NewHighScore(score);

                GameManager.Instance.uiManager.LoadHighScores(true);
            }
            else
            {
                GameManager.Instance.uiManager.LoadHighScores(false);
            }
        }        

    }

    public void NewHighScore(int newScore)
    {
        score = newScore;
        string newPlayerName = "AAA";
        bool ranked = false;

        HighScoreManager manager = GameManager.Instance.highScoreManager;

        for (int i = 0; i < manager.highScores.Count; i++)
        {
            if (newScore >= manager.highScores[i])
            {
                int lowScore;
                string lowName;
                lowScore = manager.highScores[i];
                lowName = manager.highScoresName[i];

                manager.highScores[i] = newScore;
                manager.highScoresName[i] = newPlayerName;

                newScore = lowScore;
                newPlayerName = lowName;

                if (!ranked)
                {
                    manager.rank = i;
                    ranked = true;
                }
            }
        }

        GameManager.Instance.SaveGame();

    }


}
