using UnityEngine;

public class ParallaxBackround : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float positionRight;
    [SerializeField] private float positionLeft;
    [SerializeField] private float smoothness;

    private Vector3 targetPosition;

    void Start()
    {
        targetPosition = transform.position;
    }

    void Update()
    {
        float newPositionX = Mathf.Lerp(transform.position.x, transform.position.x - moveSpeed * Time.deltaTime, smoothness);
        targetPosition = new Vector3(newPositionX, transform.position.y, transform.position.z);

        transform.position = targetPosition;

        if (cameraTransform.position.x >= transform.position.x + positionRight)
        {
            transform.position = new Vector3(cameraTransform.position.x + -positionLeft, transform.position.y, transform.position.z);
        }
    }
}
