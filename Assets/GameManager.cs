using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
// Không cần using System.Collections.Generic; nữa

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool isGameOver = false;
    private bool isPaused = false; // <-- THÊM MỚI: Biến kiểm soát trạng thái tạm dừng

    [Header("UI Elements")] // <-- THÊM MỚI: Header cho dễ nhìn
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI hightScoreText; // (Bạn có thể dùng cái này cho kỷ lục ở màn hình chính)
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject startMessagePanel;
    [SerializeField] private GameObject pausePanel; // <-- THÊM MỚI: Panel cho màn hình Pause

    [Header("Game Over UI")] // <-- THÊM MỚI: Header cho dễ nhìn
    [SerializeField] private TextMeshProUGUI finalScoreText_GameOver; // <-- THÊM MỚI: Text điểm cuối trên Panel
    [SerializeField] private TextMeshProUGUI recordScoreText_GameOver; // <-- THÊM MỚI: Text kỷ lục trên Panel

    [Header("Game Logic")] // <-- THÊM MỚI: Header cho dễ nhìn
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
        isPaused = false; // <-- THÊM MỚI
        scoreData.score = 0;
        scoreText.text = "0";
        hightScoreText.text = "Kỷ lục: " + scoreData.highScore.ToString();

        gameOverPanel.SetActive(false);
        startMessagePanel.SetActive(true);
        pausePanel.SetActive(false); // <-- THÊM MỚI: Đảm bảo Panel Pause tắt khi bắt đầu

        spawner.UpdateSpawnRate(currentMoveSpeed);
        Debug.Log("GameManager Started. Initial Speed: " + currentMoveSpeed);
    }

    public void StartGame()
    {
        if (isGameOver) return;

        startMessagePanel.SetActive(false);
        spawner.StartSpawning();
        Debug.Log("Game started from Player's first touch.");
    }

    public void AddScore()
    {
        if (isGameOver || isPaused) return; // <-- CẬP NHẬT: Không cộng điểm khi pause

        scoreData.score++;
        scoreText.text = scoreData.score.ToString();
        // Debug.Log("Score: " + scoreData.score); // Bỏ comment nếu cần

        if (scoreData.score % scoreToIncreaseSpeed == 0 && scoreData.score != 0)
        {
            IncreaseGameSpeed();
        }

        // Sửa lại: Không cần Coroutine, gọi trực tiếp sẽ tốt hơn
        CheckHighScore();
    }

    private void CheckHighScore() // <-- CẬP NHẬT: Bỏ Coroutine
    {
        if (scoreData.score > scoreData.highScore)
        {
            scoreData.highScore = scoreData.score;
            hightScoreText.text = "Kỷ lục: " + scoreData.highScore.ToString();
            Debug.Log("New High Score: " + scoreData.highScore);
            // (Giả sử ScoreData của bạn tự động lưu khi set highScore mới)
        }
    }

    void IncreaseGameSpeed()
    {
        currentMoveSpeed += speedIncreaseAmount;
        Debug.Log("Game Speed Increased! New currentMoveSpeed: " + currentMoveSpeed);
        spawner.UpdateSpawnRate(currentMoveSpeed);
    }

    // --- CẬP NHẬT HÀM GAMEOVER ---
    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        spawner.StopSpawning();
        Time.timeScale = 0f; // Dừng game
        Debug.Log("Game Over! Final Score: " + scoreData.score);

        // Cập nhật điểm số và kỷ lục cho 2 Text mới trên Panel Game Over
        if (finalScoreText_GameOver != null)
        {
            finalScoreText_GameOver.text = "ĐIỂM: " + scoreData.score.ToString();
        }
        if (recordScoreText_GameOver != null)
        {
            // Đảm bảo scoreData.highScore đã được cập nhật lần cuối
            CheckHighScore();
            recordScoreText_GameOver.text = "KỶ LỤC: " + scoreData.highScore.ToString();
        }

        gameOverPanel.SetActive(true); // Bật Panel Game Over
        pausePanel.SetActive(false); // Đảm bảo Panel Pause tắt
    }

    // --- CÁC HÀM THÊM MỚI ---

    public void RestartGame()
    {
        Debug.Log("Restarting Game...");
        Time.timeScale = 1f; // Phải set Time.timeScale = 1 TRƯỚC KHI tải lại Scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game button pressed.");
#if UNITY_EDITOR
        // Dừng game trong Unity Editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // Thoát ứng dụng khi đã build
            Application.Quit();
#endif
    }

    public void TogglePause()
    {
        // Không cho tạm dừng nếu game đã kết thúc
        if (isGameOver) return;

        isPaused = !isPaused; // Đảo ngược trạng thái tạm dừng

        if (isPaused)
        {
            Time.timeScale = 0f; // Dừng mọi thứ trong game
            pausePanel.SetActive(true); // Hiển thị PausePanel
            Debug.Log("Game Paused.");
        }
        else
        {
            Time.timeScale = 1f; // Tiếp tục game
            pausePanel.SetActive(false); // Ẩn PausePanel
            Debug.Log("Game Resumed.");
        }
    }
}