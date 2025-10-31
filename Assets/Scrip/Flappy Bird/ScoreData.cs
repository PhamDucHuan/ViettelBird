using UnityEngine;

[CreateAssetMenu(fileName = "ScoreData", menuName = "Game Data/Score Data")]
public class ScoreData : ScriptableObject
{
    public int score; // Điểm số hiện tại của ván chơi
    public int highScore; // Điểm số cao nhất

    private const string highScoreKey = "HighScore"; // Key để lưu trong PlayerPrefs

    void OnEnable()
    {
        // Tải High Score khi ScriptableObject được bật (ví dụ: khi khởi động game)
        LoadHighScore();
    }

    public void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt(highScoreKey, 0);
        Debug.Log("ScoreData: High Score loaded: " + highScore);
    }

    public void SaveHighScore()
    {
        PlayerPrefs.SetInt(highScoreKey, highScore);
        PlayerPrefs.Save(); // Lưu ngay lập tức
        Debug.Log("ScoreData: High Score saved: " + highScore);
    }

    // Bạn có thể thêm một hàm để reset High Score nếu muốn
    public void ResetHighScore()
    {
        highScore = 0;
        SaveHighScore();
        Debug.Log("ScoreData: High Score reset.");
    }
}