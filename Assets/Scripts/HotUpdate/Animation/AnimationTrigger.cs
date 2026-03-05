using System;
using Core.Components;
using HotUpdate.Component;
using UnityEngine;

namespace HotUpdate.Animation
{
    /// <summary>
    /// ����������
    /// ��Ϊ���ݶ����¼��������м��
    /// </summary>
    [RequireComponent(typeof(AnimatorComponent))]
    [DisallowMultipleComponent]
    public class AnimationTrigger : MonoBehaviour, IComponent
    {
        public event Action<int> OnAttack;

        /// <summary>
        /// ��������
        /// �����¼��ص�
        /// </summary>
        public void OnAttackTrigger(int skillId)
        {
            // ��������ӡ֡��+��ǰ��������+�Ƿ���Attack״̬
            AnimatorStateInfo stateInfo = GetComponent<Animator>().GetCurrentAnimatorStateInfo(1);
            //LogManager.Log($"{gameObject.name}������������IDΪ{skillId} | ֡�ţ�{Time.frameCount} | NormalizedTime��{stateInfo.normalizedTime:F2} | �Ƿ�Attack״̬��{stateInfo.IsName("Attack")}");
            OnAttack?.Invoke(skillId);
        }

        public IEntityObject EntityObject { get; private set; }
        
        void IComponent.Init(IEntityObject entityObject)
        {
            
        }

        public void Destroy()
        {

        }
    }
}
