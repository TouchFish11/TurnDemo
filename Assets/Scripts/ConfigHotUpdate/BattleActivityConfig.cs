using System;
using System.Collections.Generic;
using Config.ActivityConfigSO;
using UnityEngine;

namespace ConfigHotUpdate
{
    /// <summary>
    /// 战斗活动配置
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "BattleActivityConfig", menuName = "Activity/BattleActivityConfig")]
    public class BattleActivityConfig : ActivityConfig
    {
        // 战斗活动关卡数据
        public BattleConfigEntryColletion BattleConfigEntryColletion;
    }
    
    [Serializable]
    public class BattleConfigEntryColletion
    {
        public List<BattleConfigEntry> battleConfigs;
    }

    [Serializable]
    public class BattleConfigEntry
    {
        // 战斗互动关卡唯一ID
        public int levelId;
        // 关卡名称
        public string levelName;
        // 战斗场景ID，对应场景配置表
        public int battleSceneId;
        // 战斗波次
        public int battleWave;
        // 战斗怪物对象ID，对应战斗实体配置表
        public List<int> monsterIds;
        // 战斗胜利条件Id，对应具体类型
        public int winConditionId;
    }
}
