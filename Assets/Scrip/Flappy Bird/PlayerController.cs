using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float jumpForce = 5f;
    private Rigidbody2D rb;
    private bool gameStarted = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true; // Nhân vật lơ lửng cho đến khi game bắt đầu
    }

    void Update()
    {
        // Nếu game đã kết thúc hoặc đang chờ bắt đầu, không cho nhảy
        if (GameManager.instance.isGameOver) return;

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse/Touch detected!"); // Debug: Chạm màn hình

            if (!gameStarted)
            {
                gameStarted = true;
                rb.isKinematic = false; // Bật trọng lực khi game bắt đầu
                GameManager.instance.StartGame();
                Debug.Log("First touch, starting game!"); // Debug: Game bắt đầu
            }

            // Thực hiện cú nhảy
            rb.velocity = Vector2.zero; // Đặt vận tốc y về 0 để mỗi cú nhảy nhất quán
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    // Xử lý va chạm VẬT LÝ (chạm đất, chạm trụ)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (GameManager.instance.isGameOver) return; // Tránh gọi GameOver nhiều lần

        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log("Player collided with: " + collision.gameObject.name); // Debug: Va chạm
            rb.velocity = Vector2.zero; // Dừng nhân vật ngay lập tức
            GameManager.instance.GameOver();
            this.enabled = false; // Tắt script này đi khi game over
        }
    }

    // Xử lý va chạm TRIGGER (đi qua vùng tính điểm)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (GameManager.instance.isGameOver) return; // Tránh cộng điểm khi game over

        Debug.Log("Player triggered with: " + other.gameObject.name + " Tag: " + other.gameObject.tag); // Debug: Trigger

        if (other.gameObject.CompareTag("ScoreZone"))
        {
            Debug.Log("ScoreZone detected! Adding score."); // Debug: Đi qua vùng điểm
            GameManager.instance.AddScore();

            // Tắt ScoreZone này đi ngay lập tức để không bị tính điểm nhiều lần
            other.gameObject.SetActive(false);
        }
    }
}