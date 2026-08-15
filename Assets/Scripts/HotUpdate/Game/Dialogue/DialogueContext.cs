using System;
using System.Collections.Generic;
using System.Text;
using Core.Pool;
using HotUpdate.Game.Dialogue.Datas;
using HotUpdate.Game.Dialogue.Sources;
using UnityEngine;

namespace HotUpdate.Game.Dialogue
{
    /// <summary>
    /// 对话上下文，当前对话相关状态数据封装，不跨对话
    /// </summary>
    public class DialogueContext : IPoolData
    {
        /// <summary>
        /// 当前将显示的对话分支数据列表
        /// </summary>
        public List<BranchData> BranchDatas { get; } = new();
        
        /// <summary>
        /// 当前保存的对话分支来源
        /// </summary>
        public Dictionary<Type, IBranchDataSource> CurrentBranchSources { get; } = new();
        
        /// <summary>
        /// 是否有对话正在进行中
        /// </summary>
        public bool IsDialogueActive { get; set; }
        
        /// <summary>
        /// 当前单条对话是否播放完成（打字机/直接显示）
        /// </summary>
        public bool DialogueOver { get; set; }
        
        /// <summary>
        /// 启用打字机效果
        /// </summary>
        public bool EnableTypewriter { get; set; }
        
        /// <summary>
        /// 打字机效果字符间隔（秒）
        /// </summary>
        public readonly float TypewriterInterval = 0.05f;

        public StringBuilder TypewriterBuilder { get; } = new(256);
        
        // 打字机效果的协程引用
        public Coroutine TypewriterCor { get; set; }
        
        // 当前正在显示的对话信息
        public DialogueInfo CurrentDialogueInfo { get; set; }
        
        // 当前对话的NPC信息（说话者）
        public NpcInfo NpcInfo { get; set; }
        
        
        void IPoolData.ResetData()
        {
            IsDialogueActive = false;
            DialogueOver = false;
            EnableTypewriter = false;
            TypewriterBuilder.Clear();
            TypewriterCor = null;
            CurrentDialogueInfo = null;
            NpcInfo = null;
        }
    }
}
