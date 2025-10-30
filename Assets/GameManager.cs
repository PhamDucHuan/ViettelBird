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

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI hightScoreText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject startMessagePanel;

    [SerializeField] private ObstacleSpawner spawner;

    public float initialMoveSpeed = 3f;
    [SerializeField] private float speedIncreaseAmount = 0.5f;
    [SerializeField] private int scoreToIncreaseSpeed = 10;

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

        Time.timeScale = 1f;
        currentMoveSpeed = initialMoveSpeed; // Khởi tạo tốc độ ban đầu
    }

    void Start()
    {
        isGameOver = false;
        scoreData.score = 0;
        scoreText.text = "0";
        hightScoreText.text = "High Score: " + scoreData.highScore.ToString();
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

        scoreData.score++;
        scoreText.text = scoreData.score.ToString();
        Debug.Log("Score: " + scoreData.score);

        if (scoreData.score % scoreToIncreaseSpeed == 0 && scoreData.score != 0)
        {
            IncreaseGameSpeed();
        }
        StartCoroutine(CheckHightScore());
    }

    private IEnumerator CheckHightScore()
    {
        yield return new WaitForSeconds(0.1f); // Chờ một chút để đảm bảo điểm đã được cập nhật
        if (scoreData.score > scoreData.highScore)
        {
            scoreData.highScore = scoreData.score;
            hightScoreText.text = "High Score: " + scoreData.highScore.ToString();
            Debug.Log("New High Score: " + scoreData.highScore);
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
        Debug.Log("Game Over! Final Score: " + scoreData.score);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}