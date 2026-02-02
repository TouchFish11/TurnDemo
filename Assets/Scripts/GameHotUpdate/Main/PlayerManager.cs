using System.Collections.Generic;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.Components;
using Core.Config;
using Core.Service;
using Core.Singleton;
using Game.Main;
using Game.Objects;
using GameHotUpdate.Objects.Roles;
using UnityEngine;

namespace GameHotUpdate.Main
{
    /// <summary>
    /// ��ҹ�����
    /// �������״̬
    /// </summary>
    public class PlayerManager : SingletonBase<PlayerManager>, IPlayerManager
    {
        // uid���û�ʵ���ӳ��
        private readonly Dictionary<uint, IEntityObject> uidToEntityMap = new();

        public IEntityObject MainPlayer => uidToEntityMap[1001];

        private PlayerManager()
        {

        }

        public async Task CreatePlayer(uint uid)
        {
            var mainObj = new GameObject("Player");
            mainObj.transform.SetPositionAndRotation(new Vector3(0, 0, -5.6f), Quaternion.identity);

            var characterController = mainObj.AddComponent<CharacterController>();
            characterController.center = new Vector3(0, 1, 0);

            var main = mainObj.AddComponent<MainPlayer>();
            
            // ���ݱ��ر�����ݣ�������Ӧ�Ľ�ɫģ����Ϊ�ö�����Ӷ���
            var fireFlyObj = await ServiceLocator.Get<IObjectBuilder>().GetGameobject(EAssetBundleType.Prefab, ResKeyCollection.Prefab_FireFly, main.transform);
            main.AddEntity(fireFlyObj.AddComponent<FireFly>());
            
            main.BaseInit(1);
            uidToEntityMap.Add(uid, main);
        }

        public void Clear()
        {
            // �������
            foreach (var entity in uidToEntityMap.Values)
            {
                entity.Destroy();
                Object.Destroy(entity.GameObject);
            }

            uidToEntityMap.Clear();
        }
    }
}
