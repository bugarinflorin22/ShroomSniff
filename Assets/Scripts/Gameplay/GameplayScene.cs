using ShroomSniff.Constants;
using UnityEngine;

namespace ShroomSniff.Gameplay
{
    public class GameplayScene : MonoBehaviour
    {
        [SerializeField] private CameraController _cameraController;
        
        public Transform GetGameplayRoot() => transform;
        public Transform GetLevelRoot() => _levelRoot;
        public Transform GetCameraRoot() => _cameraRoot;
        public CameraController GetCameraController() => _cameraControllerInstance;

        private Transform _levelRoot;
        private Transform _cameraRoot;
        private CameraController _cameraControllerInstance;

        private void Awake()
        {
            _cameraRoot = new GameObject
            {
                name = Texts.CAMERA_ROOT_NAME,
                transform = { position = Vector3.zero }
            }.transform;
            
            _levelRoot = new GameObject
            {
                name = Texts.LEVEL_ROOT_NAME,
                transform =
                {
                    parent = transform,
                    position = Vector3.zero,
                },
            }.transform;
        }
        
        public void Inject(CameraConfiguration cameraConfiguration)
        {
            _cameraControllerInstance = Instantiate(_cameraController, _cameraRoot);
            _cameraControllerInstance.Inject(cameraConfiguration);
        }
    }
}