using Game.Main;
using System;
using System.Collections;

namespace Game.Battle
{
    public class FireFly : BattleObject
    {
        public override void Init(int id)
        {
            base.Init(id);

            // 测试
            // 添加移动、输入组件
            this.AddComponent<InputComponent>();
            this.AddComponent<MoveComponent>();
            this.AddComponent<InteractComponent>();
        }

        public override IEnumerator ExecuteAction()
        {
            throw new NotImplementedException();
        }

        public override int GetSpeed()
        {
            throw new NotImplementedException();
        }
    }
}
