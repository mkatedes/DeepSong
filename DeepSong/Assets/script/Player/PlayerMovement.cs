using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody _rb;
    [SerializeField] private float _speed = 5;
    [SerializeField] private float _turnSpeed = 360;
    private Vector3 _input;

    [SerializeField] private InputActionReference _moveAction;
    [SerializeField] private Animator _animator;


    private void OnEnable() => _moveAction.action.Enable();
    private void OnDisable() => _moveAction.action.Disable();

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        _animator = GetComponent<Animator>();

        _rb.useGravity = true;
    }
    private void Update()
    {
        GatherInput();
        if(_input == Vector3.zero)
        {
            _animator.SetFloat("Speed", 0f);
        }
        else
        {
            _animator.SetFloat("Speed", 1f);
        }
    }

    private void FixedUpdate()
    {
        Move();
        Look();
    }

    private void GatherInput()
    {
        var input2D = _moveAction.action.ReadValue<Vector2>();
        _input = new Vector3(-input2D.y, 0f, input2D.x).normalized;

    }

    private void Look()
    {
        if (_input == Vector3.zero) return;
        var rot = Quaternion.LookRotation(_input.ToIso(), Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, _turnSpeed * Time.deltaTime);
    }

    private void Move()
    {
        // Utilise la direction d'entrée directement au lieu de transform.forward
        _rb.MovePosition(transform.position + _input.ToIso() * _speed * Time.deltaTime);
    }
    public float Speed
    {
        get => _speed;
        set => _speed = value;
    }
}

public static class Helpers
{
    private static Matrix4x4 _isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
    public static Vector3 ToIso(this Vector3 input) => _isoMatrix.MultiplyPoint3x4(input);
}
