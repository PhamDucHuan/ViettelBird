using UnityEngine;
using UnityEngine.UI; // Vẫn cần nếu bạn dùng Button truyền thống, nhưng TMProUGUI thì không
using UnityEngine.SceneManagement;
using TMPro; // Rất tốt khi sử dụng TextMeshPro
using System.Collections;
// Không cần using System.Collections.Generic; nữa

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool isGameOver = false;
    private bool isPaused = false; // <-- THÊM: Biến kiểm soát trạng thái tạm dừng

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI hightScoreText; // Đổi tên thành highScoreText cho nhất quán
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject startMessagePanel;
    [SerializeField] private GameObject pausePanel; // <-- THÊM: Panel tạm dừng

    [Header("Game Speed")]
    [SerializeField] private ObstacleSpawner spawner; // Đổi thứ tự cho logic
    public float initialMoveSpeed = 3f;
    [SerializeField] private float speedIncreaseAmount = 0.5f;
    [SerializeField] private int scoreToIncreaseSpeed = 10;

    [Header("Score Data")] // Tạo Header cho dễ quản lý trong Inspector
    [SerializeField] private ScoreData scoreData; // Biến này để lưu điểm cao

    [HideInInspector] public float currentMoveSpeed; // Biến này là tốc độ CHÍNH của game

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

        Time.timeScale = 1f; // Đảm bảo game chạy bình thường khi khởi động
        currentMoveSpeed = initialMoveSpeed; // Khởi tạo tốc độ ban đầu
    }

    void Start()
    {
        isGameOver = false;
        isPaused = false; // Đảm bảo game không bị tạm dừng khi khởi tạo

        scoreData.score = 0; // Reset điểm hiện tại
        scoreText.text = "0"; // Cập nhật UI điểm hiện tại

        // Đọc High Score từ ScoreData (đã được PlayerPrefs tải khi ScoreData được tạo)
        hightScoreText.text = "HIGH SCORE: " + scoreData.highScore.ToString();

        gameOverPanel.SetActive(false); // Ẩn màn hình Game Over
        startMessagePanel.SetActive(true); // Hiển thị màn hình "Chạm để bắt đầu"
        pausePanel.SetActive(false); // <-- THÊM: Ẩn Pause Panel khi game bắt đầu

        spawner.UpdateSpawnRate(currentMoveSpeed); // Cập nhật tốc độ spawn ban đầu cho spawner
        Debug.Log("GameManager Started. Initial Speed: " + currentMoveSpeed);
    }

    public void StartGame()
    {
        if (isGameOver) return; // Không bắt đầu lại nếu đã Game Over

        startMessagePanel.SetActive(false); // Ẩn màn hình "Chạm để bắt đầu"
        spawner.StartSpawning(); // Bắt đầu spawn chướng ngại vật
        Debug.Log("Game started from Player's first touch.");
    }

    public void AddScore()
    {
        if (isGameOver || isPaused) return; // Không cộng điểm nếu game over hoặc tạm dừng

        scoreData.score++;
        scoreText.text = scoreData.score.ToString();
        Debug.Log("Score: " + scoreData.score);

        if (scoreData.score % scoreToIncreaseSpeed == 0 && scoreData.score != 0)
        {
            IncreaseGameSpeed();
        }

        // Gọi CheckHighScore ngay lập tức, không cần Coroutine nữa vì scoreData đã cập nhật ngay
        CheckHighScore();
    }

    private void CheckHighScore() // <-- SỬA: Không cần IEnumerator và WaitForSeconds
    {
        if (scoreData.score > scoreData.highScore)
        {
            scoreData.highScore = scoreData.score;
            hightScoreText.text = "HIGH SCORE: " + scoreData.highScore.ToString();
            Debug.Log("New High Score: " + scoreData.highScore);
            // Lưu High Score vào PlayerPrefs NGAY LẬP TỨC qua ScoreData
            scoreData.SaveHighScore();
        }
    }

    void IncreaseGameSpeed()
    {
        currentMoveSpeed += speedIncreaseAmount;
        Debug.Log("Game Speed Increased! New currentMoveSpeed: " + currentMoveSpeed);
        spawner.UpdateSpawnRate(currentMoveSpeed);
    }

    // <-- THÊM: Chức năng tạm dừng/tiếp tục
    public void TogglePause()
    {
        if (isGameOver) return; // Không cho tạm dừng nếu game đã kết thúc

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

    public void GameOver()
    {
        if (isGameOver) return; // Đảm bảo chỉ gọi 1 lần

        isGameOver = true;
        spawner.StopSpawning(); // Dừng spawn chướng ngại vật
        Time.timeScale = 0f; // Dừng game hoàn toàn
        Debug.Log("Game Over! Final Score: " + scoreData.score);

        // Hiển thị điểm cao nhất lên UI (đã cập nhật nếu có)
        hightScoreText.text = "HIGH SCORE: " + scoreData.highScore.ToString();

        gameOverPanel.SetActive(true); // Hiển thị màn hình Game Over
        pausePanel.SetActive(false); // <-- Đảm bảo PausePanel tắt khi game over
    }

    public void RestartGame()
    {
        Debug.Log("Restarting Game...");
        Time.timeScale = 1f; // Đảm bảo game chạy lại bình thường trước khi tải Scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Tải lại Scene hiện tại
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game button pressed.");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Dừng trong Unity Editor
#else
            Application.Quit(); // Thoát ứng dụng khi đã build
#endif
    }
}