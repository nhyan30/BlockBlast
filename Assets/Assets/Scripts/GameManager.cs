using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;

    [Header("Game Over Panel")]
    public GameObject gameOverPanel; // A panel with a Restart button
    public TextMeshProUGUI gameOverScoreText; // Score achieved text on Game Over panel
    public TextMeshProUGUI gameOverHighScoreText; // High score text on Game Over panel

    [Header("Block Settings")]
    public Sprite[] blockSprites; // Drag your block images here in the Inspector

    private int score = 0;
    private int highScore = 0;

    void Awake()
    {
        Instance = this;
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateScoreUI();

        if (gameOverPanel) gameOverPanel.SetActive(false);
    }

    void Update()
    {
        // Reset High Score and Current Score by pressing Left CTRL + D
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.D))
        {
            PlayerPrefs.DeleteKey("HighScore");
            Debug.Log("Score Data Reset! Reloading Scene...");

            // Reload the scene immediately to restart the game
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
        }
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = score.ToString();

        if (highScoreText != null)
            highScoreText.text = highScore.ToString();
    }

    public void TriggerGameOver()
    {
        if (gameOverPanel) gameOverPanel.SetActive(true);

        // Update the Game Over panel texts
        if (gameOverScoreText != null)
            gameOverScoreText.text = score.ToString();

        if (gameOverHighScoreText != null)
            gameOverHighScoreText.text = highScore.ToString();

        Debug.Log("Game Over! No moves left.");
    }

    // Call this from a UI Button on your GameOver Panel to restart
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}