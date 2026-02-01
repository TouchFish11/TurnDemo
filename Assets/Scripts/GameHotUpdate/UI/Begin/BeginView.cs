using Core.UI;
using Core.UI.MVC;
using UnityEngine;
using UnityEngine.UI;

namespace GameHotUpdate.UI.Begin
{
    /// <summary>
    /// ��ʼ����
    /// </summary>
    public class BeginView : UIView
    {
        [Inject(1)] private RectTransform progress;

        protected override void Awake()
        {
            base.Awake();

            progress.gameObject.SetActive(false);
        }

        [System.Obsolete]
        public void UpdateView(string key, object value)
        {
            switch (key)
            {
                case "sliderProgress":
                    binder.GetControl<Slider>(key).value = (float)value;
                    break;
                case "txtPro":
                    binder.GetControl<Text>(key).text = value.ToString();
                    break;
                case "txtPhase":
                    binder.GetControl<Text>(key).text = value.ToString();
                    break;
                case "txtSize":
                    binder.GetControl<Text>(key).text = value.ToString();
                    break;
                case "txtSpeed":
                    binder.GetControl<Text>(key).text = value.ToString();
                    break;
                case "isActiveProgress":
                    ShowProgress((bool)value);
                    break;
            }
        }

        /// <summary>
        /// ��ʾ������
        /// </summary>
        private void ShowProgress(bool isShow)
        {
            progress.gameObject.SetActive(isShow);
        }
    }
}
