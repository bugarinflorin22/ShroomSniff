using ShroomSniff.Data;
using ShroomSniff.Gameplay;
using UnityEngine;
using VContainer;
using Object = UnityEngine.Object;

namespace ShroomSniff.App
{
    public class AudioSystem : IAudioSystem
    {
        private readonly AppConfiguration _configuration;
        private AudioSource _audioSource;
        private bool _isInitialized;
        
        [Inject]
        public AudioSystem(AppConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Initialize()
        {
            if (_isInitialized)
                return;

            _isInitialized = true;

            var audioObject = new GameObject("AudioSystem");
            Object.DontDestroyOnLoad(audioObject);

            _audioSource = audioObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _audioSource.spatialBlend = 0f;

            MushroomController.MushroomPulled += HandleMushroomPulled;
        }

        public void PlaySuction()
        {
            if (_audioSource == null || _configuration == null)
                return;

            var clip = _configuration.SuctionSoundEffect;
            if (clip == null)
                return;

            _audioSource.PlayOneShot(clip);
        }

        public void Dispose()
        {
            MushroomController.MushroomPulled -= HandleMushroomPulled;

            if (_audioSource != null)
                Object.Destroy(_audioSource.gameObject);
        }

        private void HandleMushroomPulled(MushroomCategory category, MushroomType type)
        {
            PlaySuction();
        }
    }
}
