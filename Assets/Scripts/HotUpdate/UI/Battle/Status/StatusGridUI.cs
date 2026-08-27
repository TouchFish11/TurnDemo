using Core.UI;
using TMPro;
using UnityEngine.UI;

namespace HotUpdate.UI.Battle.Status
{
    /// <summary>
    /// 角色状态栏的状态格子对象
    /// </summary>
    public class StatusGridUI : UIBehaviourBase, ILogicView<StatusGridUI, StatusGridLogic>
    {
        [InjectUI] public Image imgIcon;
        [InjectUI] public Image imgBuffOrDeBuff;
        [InjectUI] public TextMeshProUGUI txtPine;
        
        private StatusGridLogic _logic;
        
        public bool IsValid => _logic.IsValid;

        public int StatusId => _logic.GetStatusId();
        
        public void Init(StatusGridLogic logic)
        {
            _logic = logic;
        }
    }
}
