using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Mono.MonoFunction;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// Mono������
    /// </summary>
    public class MonoManager : SingletonAutoMono<MonoManager>, IMonoManager
    {
        private List<IAwakable> awakables = new List<IAwakable>();
        private List<IEnable> enables = new List<IEnable>();
        private List<IStartable> startables = new List<IStartable>();
        private List<IDisable> disables = new List<IDisable>();
        private List<IDestroyable> destroyables = new List<IDestroyable>();

        // ����֡�����б�
        private List<Action> fixedUpdates = new List<Action>();
        // ֡�����б�
        private List<Action> updates = new List<Action>();
        // ���ڸ����б�
        private List<Action> lateUpdates = new List<Action>();

        private void Awake()
        {
            for (int i = 0; i < awakables.Count; ++i)
            {
                awakables[i].Awake();
            }
        }

        private void OnEnable()
        {
            
        }

        private void Start()
        {
            
        }

        public new Coroutine StartCoroutine(IEnumerator coroutine)
        {
            return base.StartCoroutine(coroutine);
        }

        public void AddFixedUpdateListener(Action fixedUpdateFun)
        {
            fixedUpdates.Add(fixedUpdateFun);
        }

        public void AddUpdateListener(Action updateFun)
        {
            updates.Add(updateFun);
        }

        public void AddLateUpdateListener(Action lateUpdateFun)
        {
            lateUpdates.Add(lateUpdateFun);
        }

        public void RemoveFixedUpdateListener(Action fixedUpdateFun)
        {
            fixedUpdates.Remove(fixedUpdateFun);
        }

        public void RemoveUpdateListener(Action updateFun)
        {
            updates.Remove(updateFun);
        }


        public void RemoveLateUpdateListener(Action lateUpdateFun)
        {
            lateUpdates.Remove(lateUpdateFun);
        }

        private void FixedUpdate()
        {
            for (int i = 0; i < fixedUpdates.Count; ++i)
            {
                fixedUpdates[i]?.Invoke();
            }
        }

        private void Update()
        {
            for (int i = 0; i < updates.Count; ++i)
            {
                updates[i]?.Invoke();
            }
        }

        private void LateUpdate()
        {
            for (int i = 0; i < lateUpdates.Count; ++i)
            {
                lateUpdates[i]?.Invoke();
            }
        }

        private void OnDisable()
        {
            
        }

        protected override void OnDestroy()
        {
            awakables.Clear();

            fixedUpdates.Clear();
            updates.Clear();
            lateUpdates.Clear();

            awakables = null;
            fixedUpdates = null;
            updates = null;
            lateUpdates = null;
            base.OnDestroy();
        }
    }
}
