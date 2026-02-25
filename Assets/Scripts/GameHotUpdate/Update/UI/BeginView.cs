using Core.Mono;
using Core.Service;
using Core.UI;
using Core.UI.MVC;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameHotUpdate.Update.UI
{
    /// <summary>
    /// 开始界面
    /// </summary>
    public class BeginView : UIView
    {
        [Inject] public Image imgLoading;
        [Inject] public TextMeshProUGUI txtPhase;
        [Inject] public TextMeshProUGUI txtProgress;
        [Inject] public TextMeshProUGUI txtDownloadSizeAndSpeed;
        [Inject] public Slider sliderProgress;
        [Inject] public Button btnStop;
        [Inject] public Button btnEnter;
        
        [Inject(1)] public RectTransform UpdateArea { get; private set; }
        [Inject(1)] public RectTransform EnterArea { get; private set; }

        [SerializeField] private float _rotateSpeed = 300f;
        
        protected override void Awake()
        {
            base.Awake();
            UpdateArea.gameObject.SetActive(false);
            SetEnterAreaActive(false);
        }

        public void SetUpdateAreaActive(bool isActive)
        {
            if (isActive)
            {
                ServiceLocator.Get<IMonoAdapter>().AddUpdateListener(OnUpdate);
            }
            else
            {
                ServiceLocator.Get<IMonoAdapter>().RemoveUpdateListener(OnUpdate);
            }
            UpdateArea.gameObject.SetActive(isActive);
        }

        public void SetEnterAreaActive(bool isActive)
        {
            EnterArea.gameObject.SetActive(isActive);
        }
        
        public void SetSliderProgress(float value)
        {
            sliderProgress.value = value;
        }
        
        public void SetTextProgress(string text)
        {
            txtProgress.text = text;
        }
        
        public void SetTextPhase(string phase)
        {
            txtPhase.text = phase;
        }
        
        public void SetDownloadSizeAndSpeedText(string txt)
        {
            txtDownloadSizeAndSpeed.text = txt;
        }

        public void OnUpdate()
        {
            if (!UpdateArea.gameObject.activeSelf)
            {
                return;
            }

            imgLoading.transform.Rotate(new Vector3(0, 0, Time.deltaTime * _rotateSpeed));
        }
    }
}
