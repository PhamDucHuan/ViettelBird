using UnityEngine;

public class MoveLeft : MonoBehaviour
{
    // Bỏ "public float speed = 3f;" và thay bằng biến cục bộ
    // Hoặc giữ lại nếu bạn muốn có tốc độ base riêng cho từng loại đối tượng
    // Ví dụ: City muốn chậm hơn Ground/Obstacle
    public bool isCityBackground = false; // Thêm biến này để phân biệt City

    public bool destroyOffscreen = true;
    private float leftBound = -15f;

    void Update()
    {
        if (Time.timeScale == 0f || GameManager.instance == null) return;

        // Lấy tốc độ hiện tại từ GameManager
        float currentSpeed = GameManager.instance.currentMoveSpeed;

        // Nếu là nền thành phố, di chuyển chậm hơn
        if (isCityBackground)
        {
            currentSpeed /= 3f; // Ví dụ: 1/3 tốc độ chính
        }

        transform.Translate(Vector2.left * currentSpeed * Time.deltaTime);
        // Debug.Log(gameObject.name + " is moving at: " + currentSpeed); // Debug: Kiểm tra tốc độ thực tế

        if (destroyOffscreen && transform.position.x < leftBound)
        {
            Destroy(gameObject);
        }
    }
}