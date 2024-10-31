using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private CameraMovement _cameraContainer;
    [SerializeField, Range(0, 50)] private float _movementSpeed = 40;

    private Vector3 _input;

    private Rigidbody _rigidbody;


    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        UpdateInput();
    }


    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void UpdateInput()
    {
        _input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        var isometricProjection = new Matrix4x4() {
            m00 = +1, m01 = +0, m02 = +1, m03 = +0,
            m10 = +0, m11 = +1, m12 = +0, m13 = +0,
            m20 = -1, m21 = +0, m22 = +1, m23 = +0,
            m30 = +0, m31 = +0, m32 = +0, m33 = +1,
        } * Matrix4x4.Rotate(_cameraContainer.transform.rotation);

        _input = isometricProjection.MultiplyVector(_input);
    }

    private void HandleMovement()
    {
        _rigidbody.velocity = _input * _movementSpeed * Time.fixedDeltaTime * 10;
    }
}