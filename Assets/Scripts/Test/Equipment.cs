using System;
using System.Collections.Generic;
using System.Text;

namespace Test
{
    /// <summary>
    /// 装备
    /// </summary>
    public class Equipment
    {
        public int id;
        public string name;
        public List<BonusData> bonusDatas;

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var bonusData in bonusDatas)
            {
                switch (bonusData.StatType)
                {
                    case EStatType.Hp:
                        sb.Append($"{(bonusData.BuildValue != 0 ? $"生命 +{bonusData.BuildValue}" : "")}，");
                        sb.Append($"{(bonusData.PercentValue != 0 ? $"生命 +{bonusData.PercentValue * 100}%" : "")}；");
                        break;
                    case EStatType.Atk:
                        sb.Append($"{(bonusData.BuildValue != 0 ? $"攻击 +{bonusData.BuildValue}" : "")}，");
                        sb.Append($"{(bonusData.PercentValue != 0 ? $"攻击 +{bonusData.PercentValue * 100}%" : "")}；");
                        break;
                    case EStatType.Def:
                        sb.Append($"{(bonusData.BuildValue != 0 ? $"防御 +{bonusData.BuildValue}" : "")}，");
                        sb.Append($"{(bonusData.PercentValue != 0 ? $"防御 +{bonusData.PercentValue * 100}%" : "")}；");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return $"已穿戴：{id}，名称{name}，加成信息：{sb}";
        }
    }
}
