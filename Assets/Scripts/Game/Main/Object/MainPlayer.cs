using Framework;
using Game;
using Game.Battle;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 主玩家
/// 用户操作的实体对象
/// </summary>
public class MainPlayer : EntityObject
{
    // 编队位置到实体对象映射
    private Dictionary<int, IBattleEntityObject> indexToEntityMap = new Dictionary<int, IBattleEntityObject>(); 

    public override void BaseInit(int id)
    {
        // 相机跟随
        CreateCamera();
        this.AddComponent<InputComponent>();
        this.AddComponent<NormalAnimationComponent>();
        this.AddComponent<MoveComponent>();
        this.AddComponent<InteractComponent>();
        this.AddComponent<DialogueComponent>();
    }

    public void AddEntity(IBattleEntityObject entityObject)
    {
        indexToEntityMap.Add(indexToEntityMap.Count, entityObject);
        // ...

        SetDefault();
    }

    /// <summary>
    /// 设置默认角色
    /// </summary>
    private void SetDefault()
    {
        IBattleEntityObject defaultEntity = indexToEntityMap[0];
        this.GetComponent<NormalAnimationComponent>().SetAnimator(defaultEntity.GetComponentInChildren<Animator>());
    }

    private async void CreateCamera()
    {
        OrbitCameraController orbitCameraController = await ObjectBuilder.GetObject<OrbitCameraController>(E_AssetBundleType.Camera, ResKeyCollection.MainCamera, null);
    }
}
