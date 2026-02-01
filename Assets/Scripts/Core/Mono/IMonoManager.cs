using System;
using System.Collections;
using UnityEngine;

namespace Core.Mono
{
    /// <summary>
    /// Mono�������ӿ�
    /// </summary>
    public interface IMonoManager
    {
        /// <summary>
        /// ��������֡���¼�������
        /// </summary>
        /// <param name="fixedUpdateFun">����֡���¼�������</param>
        void AddFixedUpdateListener(Action fixedUpdateFun);

        /// <summary>
        /// ���Ӻ���֡���¼�������
        /// </summary>
        /// <param name="lateUpdateFun">����֡���¼�������</param>
        void AddLateUpdateListener(Action lateUpdateFun);

        /// <summary>
        /// ����֡���¼�������
        /// </summary>
        /// <param name="updateFun">֡���¼�������</param>
        void AddUpdateListener(Action updateFun);

        /// <summary>
        /// �Ƴ�����֡���¼�������
        /// </summary>
        /// <param name="fixedUpdateFun">����֡���¼�������</param>
        void RemoveFixedUpdateListener(Action fixedUpdateFun);

        /// <summary>
        /// �Ƴ�����֡���¼�������
        /// </summary>
        /// <param name="lateUpdateFun">����֡���¼�������</param>
        void RemoveLateUpdateListener(Action lateUpdateFun);

        /// <summary>
        /// �Ƴ�֡���¼�������
        /// </summary>
        /// <param name="updateFun">֡���¼�������</param>

        void RemoveUpdateListener(Action updateFun);

        /// <summary>
        /// ����Э��
        /// </summary>
        /// <param name="coroutine"></param>
        Coroutine StartCoroutine(IEnumerator coroutine);
        
        /// <summary>
        /// 停止协程
        /// </summary>
        /// <param name="coroutine"></param>
        void StopCoroutine(Coroutine coroutine);
    }
}
