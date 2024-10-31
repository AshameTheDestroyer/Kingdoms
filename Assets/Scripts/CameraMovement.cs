using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField, Range(1, 10)] private float _movementSpeed = 2;
    [SerializeField, Range(5, 20)] private float _rotationSpeed = 10;

    [SerializeField] private KeyCode _rotateLeftKey = KeyCode.Q;
    [SerializeField] private KeyCode _rotateRightKey = KeyCode.E;

    private float ROTATION_DEGREES = 90f;

    private Quaternion _rotation = Quaternion.identity;

    private void Start()
    {
        _rotation = transform.rotation;
    }

    private void Update()
    {
        HandleRotation();
    }

    private void LateUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        transform.position = Vector3.Lerp(transform.position, GameManager.Instance.SelectedCharacter.transform.position, _movementSpeed * Time.deltaTime);
    }

    private void HandleRotation()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, _rotation, _rotationSpeed * Time.deltaTime);

        var dotProduct = Quaternion.Dot(transform.rotation, _rotation);
        bool isRotating = Mathf.Abs(1 - Mathf.Abs(dotProduct)) > 0.001f;
        
        if (isRotating) { return; }

        if (Input.GetKeyDown(_rotateLeftKey))
        {
            _rotation = Quaternion.Euler(0, _rotation.eulerAngles.y + ROTATION_DEGREES, 0);
        }

        if (Input.GetKeyDown(_rotateRightKey))
        {
            _rotation = Quaternion.Euler(0, _rotation.eulerAngles.y - ROTATION_DEGREES, 0);
        }
    }
}
