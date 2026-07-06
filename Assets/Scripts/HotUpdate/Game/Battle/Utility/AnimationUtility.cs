using System;
using System.Collections;
using Core.AssetBundles.Management;
using HotUpdate.Base.Animation;
using HotUpdate.Base.Object;
using HotUpdate.Game.Animation.Component;
using HotUpdate.Game.Battle.Object.Monster;
using HotUpdate.Game.Battle.Object.Role;
using UnityEngine;

namespace HotUpdate.Game.Battle.Utility
{
    /// <summary>
    /// 动画工具类
    /// </summary>
    public class AnimationUtility
    {
        /// <summary>
        /// 等待通用动画播放完成
        /// </summary>
        /// <param name="battleAnimationComponent"></param>
        /// <param name="layerName"></param>
        /// <param name="type"></param>
        /// <param name="playOver"></param>
        /// <returns></returns>
        public static IEnumerator WaitForCommonAnimOver(BattleAnimationComponent battleAnimationComponent, string layerName, EAnimationType type, Action playOver = null)
        {
            battleAnimationComponent.SetCommonState(type);
            yield return new WaitUntil(() => battleAnimationComponent.GetCurrentAnimatorStateInfo(layerName).IsName(type.ToString()));
            yield return new WaitUntil(() => battleAnimationComponent.GetCurrentAnimatorStateInfo(layerName).normalizedTime >= 1);
            playOver?.Invoke();
        }

        /// <summary>
        /// 通过实体类型获取对应的动画配置文件Json
        /// </summary>
        /// <param name="entityObject"></param>
        /// <returns></returns>
        public static string GetAnimConfigCollectionJsonByType(IEntityObject entityObject)
        {
            if(entityObject == null)
                throw new ArgumentNullException(nameof(entityObject));
            
            var configName = entityObject switch
            {
                IPlayerObject playerObject => playerObject.RoleInfo.f_animProfile,
                IMonsterObject monsterObject => monsterObject.MonsterInfo.f_animProfile,
                _ => string.Empty
            };

            using var handle = GameAsset.LoadAsset<TextAsset>(configName);
            return handle.Asset.text;
        }
    }
}
