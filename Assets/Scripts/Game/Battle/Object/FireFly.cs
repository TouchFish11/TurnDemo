using Framework;
using Game.Main;
using System;
using System.Collections;
using UnityEngine;

namespace Game.Battle
{
    public class FireFly : BattleObject
    {
        public override void Init(int id)
        {
            base.Init(id);

            // 测试
            // 添加移动、输入组件

            CreateCamera();
            this.AddComponent<InputComponent>();

            this.AddComponent<AnimComponent>();
            this.AddComponent<MoveComponent>();
            this.AddComponent<InteractComponent>();

            // 相机跟随
            OrbitCameraController.Instance.SetTarget(this.transform);
        }

        private async void CreateCamera()
        {
            await PoolManager.Instance.GetAssetBundleObjAsync(E_AssetBundleType.Camera, "Main Camera");
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
