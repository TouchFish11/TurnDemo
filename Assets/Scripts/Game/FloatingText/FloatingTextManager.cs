using Framework;
using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 浮动文本管理器
/// </summary>
public class FloatingTextManager : SingletonAutoMono<FloatingTextManager>
{
    // Npc对象列表
    private readonly List<NpcObject> npcObjects = new List<NpcObject>();
    // 浮动文本列表
    private readonly Dictionary<NpcObject, FloatingText> npcToTextMap = new Dictionary<NpcObject, FloatingText>();

    // 玩家对象位置
    private Transform player;
    // 最大显示距离
    private const float MaxDisplayDistance = 6f;

    private void Awake()
    {
        MonoManager.Instance.AddFixedUpdateListener(OnFixedUpdate);
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        // 测试：通过NPC标签找到场景上所有的NPC
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Npc");
        foreach (GameObject obj in objs)
        {
            npcObjects.Add(obj.GetComponent<NpcObject>());
        }

        // 测试：通过玩家标签查找
        player = GameObject.FindGameObjectWithTag("PlayerObject").transform;
    }

    private async void OnFixedUpdate()
    {
        if (player == null)
        {
            return;
        }

        // 检测距离
        foreach (NpcObject npcObject in npcObjects)
        {
            // 显示浮动文本
            if (Vector3.Distance(npcObject.transform.position, player.transform.position) <= MaxDisplayDistance)
            {
                if (!npcObject.IsShowFloatingText)
                {
                    npcObject.IsShowFloatingText = true;
                    FloatingText floatingText = await ObjectBuilder.GetObject<FloatingText>(E_AssetBundleType.Prefab, ResKeyCollection.UI_3D_FloatingText, null);
                    floatingText.Init(npcObject.transform, npcObject.NpcInfo.f_speakerName, npcObject.NpcInfo.f_identity);
                    npcToTextMap.TryAdd(npcObject, floatingText);
                }
            }
            // 隐藏浮动文本
            else
            {
                if (npcObject.IsShowFloatingText)
                {
                    npcObject.IsShowFloatingText = false;
                    PoolManager.Instance.PushObj(npcToTextMap[npcObject].gameObject);
                    npcToTextMap.Remove(npcObject);
                }
            }
        }
    }

    /// <summary>
    /// 清空缓存
    /// </summary>
    public void ClearCache()
    {
        npcObjects.Clear();
        foreach (FloatingText text in npcToTextMap.Values)
        {
            PoolManager.Instance.PushObj(text.gameObject);
        }
        npcToTextMap.Clear();
        player = null;
    }
}
