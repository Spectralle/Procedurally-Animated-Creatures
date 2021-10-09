using UnityEngine;
using Sirenix.OdinInspector;


namespace IKProceduralAnimation_GeckoImplementation
{
    [RequireComponent(typeof(Rigidbody))]
    public class IKCharacterController : MonoBehaviour
    {
        [Title("General")]
        [SerializeField, Required] private Transform _camera;
        [SerializeField] private Vector3 _cameraOffset;
        [Title("Movement Controls")]
        [SerializeField] private bool _enableMovement = true;
        [SerializeField, EnableIf("_enableMovement", true), Range(1, 100)] private float _moveSpeed = 10f;
        [Title("Mouse Controls")]
        [SerializeField] private bool _enableMouseControl = true;
        [SerializeField, EnableIf("_enableMouseControl", true), Min(0)] private Vector2Int _mouseSensitivity = new Vector2Int(15, 15);
        [SerializeField, EnableIf("_enableMouseControl", true)] private bool _enableCameraMovement = true;

        //private float _rotationY;


        private void Update()
        {
            ApplyRotation();
            ApplyMovement();
        }

        private void ApplyRotation()
        {
            if (!_enableMouseControl)
                return;

            float _rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * _mouseSensitivity.x;
            float _rotationY = transform.localEulerAngles.x;
            _rotationY += Input.GetAxis("Mouse Y") * _mouseSensitivity.y;
            _rotationY = Mathf.Clamp(_rotationY, -30f, 70f);

            transform.localEulerAngles = new Vector3(0, _rotationX, 0);

            _camera.localEulerAngles = new Vector3(-_rotationY, _rotationX, 0);
            if (_enableCameraMovement)
                _camera.position = transform.localPosition + _cameraOffset;
        }

        private void ApplyMovement()
        {
            if (!_enableMovement)
                return;

            transform.position += transform.forward * Input.GetAxis("Vertical") * (_moveSpeed / 100);
            transform.position += transform.right * Input.GetAxis("Horizontal") * (_moveSpeed / 100);
        }
    }
}