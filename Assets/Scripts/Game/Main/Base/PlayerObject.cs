using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class PlayerObject : BattleObject
    {
        public override void BaseInit(int id)
        {
            base.BaseInit(id);
        }

        public override void BattleInit(int id, IBattleContext context)
        {
            base.BattleInit(id, context);

            // 监听对话结束事件
            DialogueManager.Instance.OnDialogueEnd += OnDialogueEnd;
            // 监听对话开始事件
            DialogueManager.Instance.OnDialogueStart += OnDialogueStart;
        }

        protected virtual void OnDialogueEnd()
        {
            // 启用输入
            this.GetComponent<InputComponent>().EnableInput();
            // 启用移动
            this.GetComponent<MoveComponent>().Enable();
        }

        protected virtual void OnDialogueStart()
        {
            // 禁用输入
            this.GetComponent<InputComponent>().DisEnableInput();
            // 切换为待机动画
            this.GetComponent<AnimComponent>().SetIdle();
            // 禁用移动
            this.GetComponent<MoveComponent>().Disable();
        }

        public override IEnumerator ExecuteAction()
        {
            throw new System.NotImplementedException();
        }

        public override int GetSpeed()
        {
            throw new System.NotImplementedException();
        }

        private void OnDestroy()
        {
            // 取消监听对话开始事件
            DialogueManager.Instance.OnDialogueStart -= OnDialogueStart;
            // 取消监听对话结束事件
            DialogueManager.Instance.OnDialogueEnd -= OnDialogueEnd;
        }
    }
}
