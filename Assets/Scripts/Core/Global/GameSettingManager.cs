using Core.DataPersistence.Json;
using Core.Singleton;

namespace Core.Global
{
    public class GameSettingManager : SingletonBase<GameSettingManager>, IGameSettingManager
    {
        // ��Ϸ��������
        private GameSettingData gameSettingData;

        /// <summary>
        /// �Ի��ı����ֻ����ñ仯�¼�
        /// </summary>
        public event GameSettingEvent<bool> OnEnableTypewriterChanged;

        private GameSettingManager()
        {

        }

        /// <summary>
        /// ��ʼ��
        /// </summary>
        public void Init()
        {
            gameSettingData = JsonManager.Instance.FromJson<GameSettingData>("");
            gameSettingData.enableTypewriter = true;
        }

        /// <summary>
        /// �����Ƿ����öԻ��ı����ֻ�Ч��
        /// </summary>
        /// <param name="enable"></param>
        public void SetEnableTypewriter(bool enable)
        {
            gameSettingData.enableTypewriter = enable;
            OnEnableTypewriterChanged?.Invoke(enable);
        }
    }
}
