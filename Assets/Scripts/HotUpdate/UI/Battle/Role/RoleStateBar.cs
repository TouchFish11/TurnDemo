using System.Collections.Generic;
using Core.UI;
using HotUpdate.UI.Battle.Status;
using TMPro;
using UnityEngine.UI;

namespace HotUpdate.UI.Battle.Role
{
    /// <summary>
    /// 角色状态UI组件
    /// 负责显示单个角色的血量、能量、护盾、状态图标等信息
    /// </summary>
    public class RoleStateBar : UIBehaviourBase, ILogicView<RoleStateBar, RoleStateBarLogic>
    {
        [InjectUI] public Image imgIcon;              // 角色图标
        [InjectUI] public Image imgFade;              // 血量渐变填充条（用于血量减少时的延迟效果）
        [InjectUI] public Image imgHp;                // 当前血量填充条
        [InjectUI] public Image imgEnergy;            // 能量填充条
        [InjectUI] public Image imgShield;            // 护盾填充条
        [InjectUI] public ScrollRect svBuffBox;       // 状态图标的滚动容器
        [InjectUI] public TextMeshProUGUI txtBlood;   // 血量数值文本
        
        // 血量渐变速度
        public float fadeSpeed = 1f;

        // 能量条透明度
        public float nonFullAhpha = 0.35f;  // 能量未满时的透明度
        // 状态UI列表
        public List<StatusGridUI> StatusGrids { get; } = new();

        private RoleStateBarLogic _logic;

        /// <summary>
        /// 当前UI绑定的角色ID
        /// </summary>
        public int RoleId => _logic.RoleId;

        public void Init(RoleStateBarLogic logic)
        {
            _logic = logic;
        }
        
        /// <summary>
        /// 更新状态图标列表
        /// 通常在回合开始时调用，清理已失效的状态
        /// </summary>
        public void UpdateStatus()
        {
            _logic.UpdateStatus();
        }
        
        /// <summary>
        /// 按钮点击事件处理
        /// </summary>
        /// <param name="btnName">按钮名称</param>
        protected override void OnButtonClick(string btnName)
        {
            switch (btnName)
            {
                case "btnSkill":
                    _logic.TriggerUltimate();
                    break;
            }
        }
    }
}