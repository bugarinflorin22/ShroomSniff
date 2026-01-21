using TMPro;
using ShroomSniff.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace ShroomSniff.UI
{
    public class GameUIController : MonoBehaviour
    {
        [SerializeField] private Image _chargeFillImage;
        [SerializeField] private TextMeshProUGUI _timerText;

        private void Awake()
        {
            ResetCharge();
        }

        private void OnEnable()
        {
            MushroomController.ChargeProgressChanged += HandleChargeProgressChanged;
        }

        private void OnDisable()
        {
            MushroomController.ChargeProgressChanged -= HandleChargeProgressChanged;
        }

        public void SetTimerText(string text)
        {
            _timerText.text = text;
        }
        
        private void HandleChargeProgressChanged(float normalized)
        {
            if (_chargeFillImage == null)
                return;

            _chargeFillImage.fillAmount = Mathf.Clamp01(normalized);
        }

        private void ResetCharge()
        {
            if (_chargeFillImage != null)
                _chargeFillImage.fillAmount = 0f;
        }
    }
}