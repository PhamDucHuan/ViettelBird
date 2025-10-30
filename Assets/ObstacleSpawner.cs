using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject obstaclePrefab;
    [SerializeField] private float heightRange = 2.5f;
    [SerializeField] private float baseSpawnRate = 1.8f;    // <-- ĐIỀU CHỈNH KHOẢNG CÁCH GẦN/XA Ở ĐÂY
    [SerializeField] private float minSpawnRate = 0.5f;

    private float currentSpawnRateValue;
    private float spawnTimer; // Bộ đếm thời gian của chúng ta
    private bool isSpawning = false; // Cờ kiểm soát việc spawn

    void Start()
    {
        currentSpawnRateValue = baseSpawnRate;
        // Chúng ta sẽ bắt đầu đếm ngược ngay khi StartSpawning được gọi
    }

    void Update()
    {
        // Nếu game chưa bắt đầu hoặc đã kết thúc, không làm gì cả
        if (!isSpawning || GameManager.instance.isGameOver)
        {
            return;
        }

        // Đếm ngược thời gian mỗi frame
        spawnTimer -= Time.deltaTime;

        // Nếu bộ đếm thời gian đã hết (<= 0)
        if (spawnTimer <= 0f)
        {
            SpawnObstacle();

            // Đặt lại bộ đếm thời gian cho lần spawn tiếp theo
            // Nó sẽ tự động lấy giá trị "currentSpawnRateValue" mới nhất
            spawnTimer = currentSpawnRateValue;
        }
    }

    public void StartSpawning()
    {
        isSpawning = true;
        // Đặt bộ đếm cho lần spawn ĐẦU TIÊN
        // (chờ 'baseSpawnRate' giây rồi mới spawn cái đầu tiên)
        spawnTimer = currentSpawnRateValue;
        Debug.Log("Obstacle Spawner: Start Spawning using Update(). Rate: " + currentSpawnRateValue);
    }

    public void StopSpawning()
    {
        isSpawning = false;
        Debug.Log("Obstacle Spawner: Stop Spawning.");
    }

    public void UpdateSpawnRate(float newGameSpeed)
    {
        float speedRatio = newGameSpeed / GameManager.instance.initialMoveSpeed;
        if (speedRatio < 1f) speedRatio = 1f;

        float newCalculatedSpawnRate = baseSpawnRate / speedRatio;
        newCalculatedSpawnRate = Mathf.Max(newCalculatedSpawnRate, minSpawnRate);

        // CHỈ CẦN CẬP NHẬT GIÁ TRỊ NÀY
        // Vòng lặp Update() sẽ tự động lấy giá trị mới này khi nó đặt lại 'spawnTimer'
        currentSpawnRateValue = newCalculatedSpawnRate;
        Debug.Log("New Spawn Rate calculated: " + currentSpawnRateValue);
    }

    void SpawnObstacle()
    {
        float randomY = Random.Range(-heightRange, heightRange);
        Vector3 spawnPos = new Vector3(10f, randomY, 0);

        Instantiate(obstaclePrefab, spawnPos, Quaternion.identity);
        Debug.Log("Spawned new obstacle. Next spawn in: " + currentSpawnRateValue + " seconds.");
    }
}