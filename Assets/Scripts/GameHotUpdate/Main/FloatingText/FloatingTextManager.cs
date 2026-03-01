using System.Collections.Generic;
using Core.Mono;
using Core.Pool;
using Core.Service;
using Core.Singleton;
using GameHotUpdate.Battle.Object;
using GameHotUpdate.Config;
using GameHotUpdate.Main.Object;
using UnityEngine;

namespace GameHotUpdate.Main.FloatingText
{
    /// <summary>
    /// �����ı�������
    /// </summary>
    public class FloatingTextManager : SingletonAutoMono<FloatingTextManager>, IFloatingTextManager
    {
        // Npc�����б�
        private readonly List<NpcObject> npcObjects = new();
        // �����ı��б�
        private readonly Dictionary<NpcObject, FloatingTextObj> npcToTextMap = new();

        // ��Ҷ���λ��
        private Transform player;
        // �����ʾ����
        private const float MaxDisplayDistance = 10f;

        private void Awake()
        {
            ServiceLocator.Get<IMonoAdapter>().AddFixedUpdateListener(OnFixedUpdate);
        }

        public void Init()
        {
            // ���ԣ�ͨ��NPC��ǩ�ҵ����������е�NPC
            var objs = GameObject.FindGameObjectsWithTag("Npc");
            foreach (GameObject obj in objs)
            {
                npcObjects.Add(obj.GetComponent<NpcObject>());
            }

            // ͨ����ҹ�������ȡ
            player = ServiceLocator.Get<IPlayerManager>().MainPlayer.GameObject.transform;
        }

        private async void OnFixedUpdate()
        {
            if (player == null)
            {
                return;
            }

            // ������
            foreach (NpcObject npcObject in npcObjects)
            {
                // ��ʾ�����ı�
                if (Vector3.Distance(npcObject.transform.position, player.transform.position) <= MaxDisplayDistance)
                {
                    if (!npcObject.IsShowFloatingText)
                    {
                        npcObject.IsShowFloatingText = true;
                        var floatingTextObj = await ServiceLocator.Get<IObjectBuilder>().GetHotfixObject<FloatingTextObj>(AbKeyCollection.Prefab, ResKeyCollection.UI_3D_FloatingText, null);
                        floatingTextObj.Init(npcObject.transform, player, npcObject.NpcInfo.f_speakerName, npcObject.NpcInfo.f_identity);
                        npcToTextMap.TryAdd(npcObject, floatingTextObj);
                    }
                }
                // ���ظ����ı�
                else
                {
                    if (npcObject.IsShowFloatingText)
                    {
                        npcObject.IsShowFloatingText = false;
                        ServiceLocator.Get<IPoolManager>().PushObj(npcToTextMap[npcObject].gameObject);
                        npcToTextMap.Remove(npcObject);
                    }
                }
            }
        }

        public void ClearCache()
        {
            npcObjects.Clear();
            foreach (var text in npcToTextMap.Values)
            {
                ServiceLocator.Get<IPoolManager>().PushObj(text.gameObject);
            }
            npcToTextMap.Clear();
            player = null;
        }
    }
}
