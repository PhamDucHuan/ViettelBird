using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
// Không cần using System.Collections.Generic; nữa

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int score = 0;
    public bool isGameOver = false;

    public TextMeshProUGUI scoreText;
    public GameObject gameOverPanel;
    public GameObject startMessagePanel;

    public ObstacleSpawner spawner;

    public float initialMoveSpeed = 3f;
    public float speedIncreaseAmount = 0.5f;
    public int scoreToIncreaseSpeed = 10;

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

        Time.timeScale = 1f;
        currentMoveSpeed = initialMoveSpeed; // Khởi tạo tốc độ ban đầu
    }

    void Start()
    {
        isGameOver = false;
        score = 0;
        scoreText.text = "0";
        gameOverPanel.SetActive(false);
        startMessagePanel.SetActive(true);

        // --- KHÔNG CẦN TÌM FindObjectsOfType<MoveLeft>() Ở ĐÂY NỮA ---
        // --- KHÔNG CẦN UpdateAllMoveLeftSpeeds() Ở ĐÂY NỮA ---

        // Cập nhật tốc độ spawn ban đầu cho spawner
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
        if (isGameOver) return;

        score++;
        scoreText.text = score.ToString();
        Debug.Log("Score: " + score);

        if (score % scoreToIncreaseSpeed == 0 && score != 0)
        {
            IncreaseGameSpeed();
        }
    }

    void IncreaseGameSpeed()
    {
        currentMoveSpeed += speedIncreaseAmount;
        Debug.Log("Game Speed Increased! New currentMoveSpeed: " + currentMoveSpeed);

        // --- KHÔNG CẦN UpdateAllMoveLeftSpeeds() Ở ĐÂY NỮA ---

        // Cập nhật tốc độ spawn của chướng ngại vật
        spawner.UpdateSpawnRate(currentMoveSpeed);
    }

    // --- XÓA HOÀN TOÀN HÀM UpdateAllMoveLeftSpeeds() VÀ RegisterMoveLeftObject() ---
    // Vì MoveLeft sẽ tự động lấy tốc độ từ đây

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        spawner.StopSpawning();
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
        Debug.Log("Game Over! Final Score: " + score);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}