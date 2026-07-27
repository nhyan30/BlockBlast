using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;

    [Header("Game Over Panel")]
    public TextMeshProUGUI gameOverScoreText;
    public TextMeshProUGUI gameOverHighScoreText;

    [Header("Block Settings")]
    public Sprite[] blockSprites;

    private int score = 0;
    private int displayedScore = 0;
    private int highScore = 0;

    void Awake()
    {
        Instance = this;
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateScoreUI();
    }

    void Update()
    {
        // Gradual score counting
        if (displayedScore != score)
        {
            int diff = score - displayedScore;
            int step = Mathf.Max(1, Mathf.Abs(diff) / 10); // Move at least 1, or 10% of diff per frame
            if (diff > 0) displayedScore += step;
            else displayedScore -= step;

            if (Mathf.Abs(score - displayedScore) < step) displayedScore = score;

            if (scoreText != null) scoreText.text = displayedScore.ToString();
        }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.D))
        {
            PlayerPrefs.DeleteKey("HighScore");
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
        if (highScoreText != null) highScoreText.text = highScore.ToString();
    }

    public void TriggerGameOver()
    {
        StartCoroutine(GameOverRoutine());
    }

    private IEnumerator GameOverRoutine()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.Play(SoundType.GameOver);

        // Show "No Spaces Left" first
        if (UIManager.Instance != null) UIManager.Instance.ShowNoSpacesLeft();

        yield return new WaitForSeconds(1.5f); // Wait for 1.5 seconds

        if (UIManager.Instance != null) UIManager.Instance.HideNoSpacesLeft();

        // Update Game Over panel texts
        if (gameOverScoreText != null) gameOverScoreText.text = score.ToString();
        if (gameOverHighScoreText != null) gameOverHighScoreText.text = highScore.ToString();

        // Tell UIManager to Fade in the Game Over panel
        if (UIManager.Instance != null) UIManager.Instance.ShowGameOver();
    }

    // Soft restart - no scene reload
    public void ResetGameplay()
    {
        score = 0;
        displayedScore = 0;
        if (scoreText != null) scoreText.text = "0";

        if (GridManager.Instance != null) GridManager.Instance.ClearGrid();
        if (BlockSpawner.Instance != null)
        {
            BlockSpawner.Instance.ClearBlocks();
            BlockSpawner.Instance.SpawnNewBlocks();
        }
    }

    // Kept for compatibility if called from elsewhere
    public void RestartGame()
    {
        ResetGameplay();
    }
}