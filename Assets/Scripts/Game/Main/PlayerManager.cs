using Framework;
using Game;
using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 玩家管理器
/// 管理玩家状态
/// </summary>
public class PlayerManager : SingletonBase<PlayerManager>, IPlayerManager
{
    // uid到用户实体的映射
    private Dictionary<uint, IEntityObject> uidToEntityMap = new Dictionary<uint, IEntityObject>();

    public IEntityObject MainPlayer => uidToEntityMap[1001];

    private PlayerManager()
    {

    }

    public async Task CreatePlayer(uint uid)
    {
        GameObject mainObj = new GameObject("Player");
        mainObj.transform.SetPositionAndRotation(new Vector3(0, 0, -5.6f), Quaternion.identity);

        CharacterController characterController = mainObj.AddComponent<CharacterController>();
        characterController.center = new Vector3(0, 1, 0);

        MainPlayer main = mainObj.AddComponent<MainPlayer>();
        main.BaseInit(1);

        // 根据本地编队数据，创建对应的角色模型作为该对象的子对象
        GameObject fireFlyObj = await ObjectBuilder.GetOrCreateInstance(E_AssetBundleType.Prefab, ResKeyCollection.Prefab_FireFly, main.transform);
        main.AddEntity(fireFlyObj.GetComponent<FireFly>());

        uidToEntityMap.Add(uid, main);
    }

    public void Clear()
    {
        // 销毁玩家
        foreach (var entity in uidToEntityMap.Values)
        {
            entity.DestroyEntity();
            GameObject.Destroy(entity.GameObject);
        }

        uidToEntityMap.Clear();
    }
}
