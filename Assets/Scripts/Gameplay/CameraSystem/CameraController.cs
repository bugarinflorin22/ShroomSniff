using UnityEngine;

namespace ShroomSniff.Gameplay
{
    public class CameraController : MonoBehaviour
    {
        private const float ScrollScale = 0.1f;
        
        private CameraConfiguration _cameraConfig;
        private Camera _mainCamera;
        private bool _shouldStop;
        private bool _isEnabled = true;
        private float _currentZoom;
        private bool _isDragging;
        private Vector3 _lastMousePosition;

        public void Inject(CameraConfiguration cameraConfiguration)
        {
            _cameraConfig = cameraConfiguration;
            
            Initialize();
        }

        public void SetEnabled(bool enabled)
        {
            _isEnabled = enabled;
            
            // Cancel any ongoing drag when disabled
            if (!enabled)
                _isDragging = false;
        }

        private void Initialize()
        {
            _mainCamera = Camera.main;
            _currentZoom = _cameraConfig.initialZoom;

            if (_mainCamera == null)
            {
                Debug.LogError($"[{nameof(CameraController)}] Camera is null.");
                _shouldStop = true;
                return;
            }
            
            _mainCamera.orthographic = true;
            _mainCamera.orthographicSize = _currentZoom;
        }

        private void Update()
        {
            if (_shouldStop || !_isEnabled)
                return;

            HandleZoom();
            HandleDragPan();
            HandleKeyboardPan();
            EnforceRootConstraints();
        }

        private void HandleZoom()
        {
            var scroll = Input.mouseScrollDelta.y;
            if (Mathf.Abs(scroll) <= Mathf.Epsilon) 
                return;

            _currentZoom -= scroll * ScrollScale * _cameraConfig.zoomSpeed;
            _currentZoom = Mathf.Clamp(_currentZoom, _cameraConfig.minZoom, _cameraConfig.maxZoom);
            _mainCamera.orthographicSize = _currentZoom;
        }

        private void HandleDragPan()
        {
            if (Input.GetMouseButtonDown(1))
            {
                _lastMousePosition = Input.mousePosition;
                _isDragging = true;
            }

            if (Input.GetMouseButton(1) && _isDragging)
            {
                var mouseDelta = Input.mousePosition - _lastMousePosition;
                var right = new Vector3(transform.right.x, 0f, transform.right.z).normalized;
                var forward = new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;

                var unitsPerPixel = (_mainCamera.orthographicSize * 2f) / Screen.height;
                var delta = (-right * mouseDelta.x + -forward * mouseDelta.y) * (unitsPerPixel * _cameraConfig.dragSpeed);
                transform.position = ClampToBounds(transform.position + delta);
                _lastMousePosition = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(1))
                _isDragging = false;
        }

        private void HandleKeyboardPan()
        {
            var horizontal = Input.GetAxisRaw("Horizontal");
            var vertical = Input.GetAxisRaw("Vertical");
            if (Mathf.Abs(horizontal) <= Mathf.Epsilon && Mathf.Abs(vertical) <= Mathf.Epsilon) 
                return;

            var right = new Vector3(transform.right.x, 0f, transform.right.z).normalized;
            var forward = new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;
            var delta = (right * horizontal + forward * vertical) * (_cameraConfig.panSpeed * Time.deltaTime);
            transform.position = ClampToBounds(transform.position + delta);
        }

        private Vector3 ClampToBounds(Vector3 position)
        {
            position.y = 0f;
            if (_cameraConfig.useBounds == false) 
                return position;

            position.x = Mathf.Clamp(position.x, -_cameraConfig.halfExtents.x, _cameraConfig.halfExtents.x);
            position.z = Mathf.Clamp(position.z, -_cameraConfig.halfExtents.y, _cameraConfig.halfExtents.y);
            return position;
        }

        private void EnforceRootConstraints()
        {
            if (Mathf.Abs(transform.position.y) > Mathf.Epsilon)
                transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
        }
    }
}