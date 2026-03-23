using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Test
{
    /// <summary>
    /// 加成数据，对单一的属性类型的两种加成的封装
    /// </summary>
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class BonusData
    {
        [JsonProperty][SerializeField] private EStatType _statType;
        [JsonProperty][SerializeField] private float _buildValue;
        [JsonProperty][SerializeField] private float _percentValue;

        /// <summary>
        /// 属性类型
        /// </summary>
        public EStatType StatType
        {
            get => _statType;
            set => _statType = value;
        }

        /// <summary>
        /// 固定加成
        /// </summary>
        public float BuildValue
        {
            get => _buildValue;
            set => _buildValue = value;
        }

        /// <summary>
        /// 百分比加成
        /// </summary>
        public float PercentValue
        {
            get => _percentValue;
            set => _percentValue = value;
        }
    }
}
