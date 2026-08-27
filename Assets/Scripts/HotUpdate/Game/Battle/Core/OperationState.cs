using System.Collections.Generic;
using HotUpdate.Game.Battle.Object;

namespace HotUpdate.Game.Battle.Core
{
    public class OperationState
    {
        // 是否激活战斗输入
        private bool _isActiveInput;
        
        /// <summary>
        /// 上次选中的主目标
        /// </summary>
        public IBattleEntityObject LastTarget { get; set; }

        /// <summary>
        /// 上次选中的目标列表
        /// </summary>
        public List<IBattleEntityObject> LastTargets { get; set; }
        
        /// <summary>
        /// 当前选中技能的配置信息
        /// </summary>
        public SkillInfo CurrentSkillInfo { get; set; }
        
        /// <summary>
        /// 是否激活目标选择
        /// </summary>
        public bool IsActiveTargetSelect { get; set; }

        /// <summary>
        /// 是否激活战斗输入
        /// </summary>
        public bool IsActiveInput
        {
            get => _isActiveInput;
            set
            {
                if (value && !_isActiveInput)
                {
                    _isActiveInput = true;
                }
                else if (!value && _isActiveInput)
                {
                    _isActiveInput = false;
                }
            }
        }
    }
}
