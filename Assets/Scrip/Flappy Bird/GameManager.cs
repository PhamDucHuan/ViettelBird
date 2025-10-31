using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool isGameOver = false;
    private bool isPaused = false;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI hightScoreText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject startMessagePanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject pauseGameButton; // <-- THÊM MỚI: Tham chiếu đến nút Pause

    [Header("Game Over UI")]
    [SerializeField] private TextMeshProUGUI finalScoreText_GameOver;
    [SerializeField] private TextMeshProUGUI recordScoreText_GameOver;

    [Header("Game Logic")]
    [SerializeField] private ObstacleSpawner spawner;
    public float initialMoveSpeed = 3f;
    [SerializeField] private float speedIncreaseAmount = 0.5f;
    [SerializeField] private int scoreToIncreaseSpeed = 10;
    [SerializeField] private ScoreData scoreData;

    [HideInInspector] public float currentMoveSpeed;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        Time.timeScale = 1f;
        currentMoveSpeed = initialMoveSpeed;
    }

    void Start()
    {
        isGameOver = false;
        isPaused = false;
        scoreData.score = 0;
        scoreText.text = "0";
        hightScoreText.text = "Kỷ lục: " + scoreData.highScore.ToString();

        gameOverPanel.SetActive(false);
        startMessagePanel.SetActive(true);
        pausePanel.SetActive(false);

        pauseGameButton.SetActive(false); // <-- THÊM MỚI: Ẩn nút Pause khi game khởi động

        spawner.UpdateSpawnRate(currentMoveSpeed);
        Debug.Log("GameManager Started. Initial Speed: " + currentMoveSpeed);
    }

    public void StartGame()
    {
        if (isGameOver) return;

        startMessagePanel.SetActive(false);
        pauseGameButton.SetActive(true); // <-- THÊM MỚI: Hiện nút Pause khi game bắt đầu
        spawner.StartSpawning();
        Debug.Log("Game started from Player's first touch.");
    }

    public void AddScore()
    {
        if (isGameOver || isPaused) return;

        scoreData.score++;
        scoreText.text = scoreData.score.ToString();

        if (scoreData.score % scoreToIncreaseSpeed == 0 && scoreData.score != 0)
        {
            IncreaseGameSpeed();
        }

        CheckHighScore();
    }

    private void CheckHighScore()
    {
        if (scoreData.score > scoreData.highScore)
        {
            scoreData.highScore = scoreData.score;
            hightScoreText.text = "Kỷ lục: " + scoreData.highScore.ToString();
            Debug.Log("New High Score: " + scoreData.highScore);
            scoreData.SaveHighScore();
        }
    }

    void IncreaseGameSpeed()
    {
        currentMoveSpeed += speedIncreaseAmount;
        Debug.Log("Game Speed Increased! New currentMoveSpeed: " + currentMoveSpeed);
        spawner.UpdateSpawnRate(currentMoveSpeed);
    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        spawner.StopSpawning();
        Time.timeScale = 0f;
        Debug.Log("Game Over! Final Score: " + scoreData.score);

        if (finalScoreText_GameOver != null)
        {
            finalScoreText_GameOver.text = "ĐIỂM: " + scoreData.score.ToString();
        }
        if (recordScoreText_GameOver != null)
        {
            CheckHighScore();
            recordScoreText_GameOver.text = "KỶ LỤC: " + scoreData.highScore.ToString();
        }

        gameOverPanel.SetActive(true);
        pausePanel.SetActive(false);
        pauseGameButton.SetActive(false); // <-- THÊM MỚI: Ẩn nút Pause khi Game Over
    }

    public void RestartGame()
    {
        Debug.Log("Restarting Game...");
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game button pressed.");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    public void TogglePause()
    {
        if (isGameOver || startMessagePanel.activeSelf) return;

        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
            pausePanel.SetActive(true);
            Debug.Log("Game Paused.");
        }
        else
        {
            Time.timeScale = 1f;
            pausePanel.SetActive(false);
            Debug.Log("Game Resumed.");
        }
    }
}