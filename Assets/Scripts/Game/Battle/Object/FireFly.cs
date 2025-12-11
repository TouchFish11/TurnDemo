using Framework;
using Game.Main;
using System;
using System.Collections;
using UnityEngine;

namespace Game.Battle
{
    public class FireFly : BattleObject
    {
        public override void BaseInit(int id)
        {
            base.BaseInit(id);

            // 测试
            // 添加移动、输入组件

            CreateCamera();
            this.AddComponent<InputComponent>();

            this.AddComponent<AnimComponent>();
            this.AddComponent<MoveComponent>();
            this.AddComponent<InteractComponent>();
            this.AddComponent<DialogueComponent>();

            // 相机跟随
            OrbitCameraController.Instance.SetTarget(this.transform);
        }

        private async void CreateCamera()
        {
            await PoolManager.Instance.GetAssetBundleObjAsync(E_AssetBundleType.Camera, ResConfigCollection.MainCamera);
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
