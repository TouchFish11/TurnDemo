using Core.UI.MVC;

namespace GameHotUpdate.Update.UI
{
    /// <summary>
    /// ��ʼ��������
    /// </summary>
    public class BeginModel : UIModel
    {
        private float  sliderProgress;
        private string txtPro;
        private string txtPhase;
        private string txtSize;
        private string txtSpeed;
        private bool isActiveProgress;

        public float SilderProgress
        {
            get => sliderProgress;
            set
            {
                sliderProgress = value;
                // TriggerDataChanged(nameof(sliderProgress), value);
            }
        }

        public string TxtProgress
        {
            get => txtPro;
            set
            {
                txtPro = value;
                // TriggerDataChanged(nameof(txtPro), value);
            }
        }

        public string TxtPhase
        {
            get => txtPhase;
            set
            {
                txtPhase = value;
                // TriggerDataChanged(nameof(txtPhase), value);
            }
        }

        public string TxtSize
        {
            get => txtSize;
            set
            {
                txtSize = value;
                // TriggerDataChanged(nameof(txtSize), value);
            }
        }


        public string TxtSpeed
        {
            get => txtSpeed;
            set
            {
                txtSpeed = value;
                // TriggerDataChanged(nameof(txtSpeed), value);
            }
        }

        public bool IsActiveProgress
        {
            get => isActiveProgress;
            set
            {
                isActiveProgress = value;
                // TriggerDataChanged(nameof(isActiveProgress), value);
            }
        }
    }
}
