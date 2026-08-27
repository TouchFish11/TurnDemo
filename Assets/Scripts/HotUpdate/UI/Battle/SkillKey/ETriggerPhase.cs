namespace HotUpdate.UI.Battle.SkillKey
{
    /// <summary>
    /// 技能触发阶段
    /// 用于管控技能按键从"未选中→选中→触发"的状态流转
    /// </summary>
    public enum ETriggerPhase
    {
        /// <summary>
        /// 未选中状态
        /// 初始/取消选中时的默认状态
        /// </summary>
        NonSeleceted,
            
        /// <summary>
        /// 已选中状态
        /// 按键被选中但未触发技能的状态
        /// </summary>
        Selected,
            
        /// <summary>
        /// 触发状态
        /// 选中后再次点击进入的触发状态
        /// </summary>
        Trigger,
    }
}