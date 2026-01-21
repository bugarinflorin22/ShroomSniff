using System;
using ShroomSniff.Data;
using UnityEngine;

namespace ShroomSniff.Gameplay
{
    public class MushroomController : MonoBehaviour
    {
        [SerializeField] private MushroomCategory _mushroomCategory;

        private MushroomType _mushroomType;
        private IMushroomRuntimeSettings _settings;
        private Vector3 _initialScale;
        private Vector3 _initialWorldPosition;
        private Quaternion _initialLocalRotation;
        private float _holdTimer;
        private bool _isCursorOver;
        private bool _wasCharging;
        private bool _isTargeted;
        private bool _isCollecting;
        private bool _isEnabled = true;
        private Camera _mainCamera;

        private void Awake()
        {
            _mainCamera = Camera.main;
            CacheInitialTransform();
        }

        public void Initialize(MushroomType mushroomType, IMushroomRuntimeSettings settings)
        {
            SetMushroomType(mushroomType);
            _settings = settings;
            ApplySize(settings);
            CacheInitialTransform();
        }

        private void SetMushroomType(MushroomType mushroomType)
        {
            _mushroomType = mushroomType;
        }

        public MushroomCategory GetMushroomCategory()
        {
            return _mushroomCategory;
        }

        public MushroomType GetMushroomType()
        {
            return _mushroomType;
        }

        public void SetEnabled(bool enabled)
        {
            _isEnabled = enabled;
            
            if (!enabled)
            {
                // Reset any ongoing collection when disabled
                if (_wasCharging || _holdTimer > 0f)
                {
                    ResetCharge();
                }
                _isTargeted = false;
            }
        }

        private void ApplySize(IMushroomRuntimeSettings settings)
        {
            if (settings == null)
                return;

            if (!settings.TryGetSizeMultiplier(_mushroomType, out var multiplier))
                return;

            transform.localScale = _initialScale * Mathf.Max(0f, multiplier);
        }

        private void Update()
        {
            if (_settings == null || !_isEnabled)
                return;

            if (_isCollecting)
                return;

            if (Input.GetMouseButtonDown(0) && _isCursorOver)
                _isTargeted = true;

            var isHolding = Input.GetMouseButton(0);
            var isCharging = _isTargeted && isHolding;
            if (isCharging)
            {
                AdvanceCharge(Time.deltaTime);
            }
            else if (_wasCharging || _holdTimer > 0f || _isTargeted)
            {
                ResetCharge();
                _isTargeted = false;
            }

            _wasCharging = isCharging;
        }

        private void AdvanceCharge(float deltaTime)
        {
            if (_isCollecting)
                return;

            var duration = Mathf.Max(0.01f, _settings.HoldToCollectDuration <= 0f ? 1f : _settings.HoldToCollectDuration);
            _holdTimer = Mathf.Min(_holdTimer + deltaTime, duration);
            var normalized = Mathf.Clamp01(_holdTimer / duration);

            ApplyPull(normalized);
            BroadcastCharge(normalized);

            if (_holdTimer >= duration)
                StartCollect();
        }

        private void ResetCharge()
        {
            _holdTimer = 0f;
            ApplyPull(0f);
            BroadcastCharge(0f);
        }

        private void ApplyPull(float normalized)
        {
            var curve = _settings.PullCurve ?? AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
            var t = Mathf.Clamp01(curve.Evaluate(normalized));

            transform.position = GetPulledWorldPosition(t);
            var rotationSpeed = Mathf.Max(0f, _settings.PullRotationSpeed);
            transform.localRotation = _initialLocalRotation * Quaternion.Euler(_settings.PullRotationOffset * (t * rotationSpeed));

            var scaleMultiplier = GetShrinkMultiplier();
            var scaled = Vector3.Scale(_initialScale, Vector3.Lerp(Vector3.one, scaleMultiplier, t));
            transform.localScale = scaled;
        }

        private Vector3 GetShrinkMultiplier()
        {
            var scaleMultiplier = _settings.PullScaleMultiplier;
            if (scaleMultiplier == Vector3.zero)
                scaleMultiplier = new Vector3(0.6f, 0.6f, 0.6f);

            if (scaleMultiplier.x >= 1f && scaleMultiplier.y >= 1f && scaleMultiplier.z >= 1f)
                scaleMultiplier = new Vector3(0.6f, 0.6f, 0.6f);

            return new Vector3(
                Mathf.Clamp(scaleMultiplier.x, 0.05f, 1f),
                Mathf.Clamp(scaleMultiplier.y, 0.05f, 1f),
                Mathf.Clamp(scaleMultiplier.z, 0.05f, 1f));
        }

        private void StartCollect()
        {
            if (_isCollecting)
                return;

            _isCollecting = true;
            BroadcastPulled();
            BroadcastCharge(0f);
            Destroy(gameObject);
        }

        private void CacheInitialTransform()
        {
            _initialWorldPosition = transform.position;
            _initialLocalRotation = transform.localRotation;
            _initialScale = transform.localScale;
        }

        private Vector3 GetPulledWorldPosition(float t)
        {
            if (_mainCamera == null)
                return _initialWorldPosition;

            var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            if (!TryGetGroundPoint(ray, out var cursorWorld))
                return _initialWorldPosition;

            var target = cursorWorld + _settings.PullPositionOffset;
            return Vector3.Lerp(_initialWorldPosition, target, t);
        }

        private static bool TryGetGroundPoint(Ray ray, out Vector3 point)
        {
            var ground = new Plane(Vector3.up, Vector3.zero);
            if (ground.Raycast(ray, out var distance))
            {
                point = ray.GetPoint(distance);
                return true;
            }

            point = Vector3.zero;
            return false;
        }

        private void OnMouseOver()
        {
            _isCursorOver = true;
        }

        private void OnMouseExit()
        {
            _isCursorOver = false;
        }

        private void OnDisable()
        {
            if (_wasCharging || _holdTimer > 0f)
                BroadcastCharge(0f);
        }

        public static event Action<float> ChargeProgressChanged;
        public static event Action<MushroomCategory, MushroomType> MushroomPulled;

        private static void BroadcastCharge(float normalized)
        {
            ChargeProgressChanged?.Invoke(normalized);
        }

        private void BroadcastPulled()
        {
            MushroomPulled?.Invoke(_mushroomCategory, _mushroomType);
        }
    }
}