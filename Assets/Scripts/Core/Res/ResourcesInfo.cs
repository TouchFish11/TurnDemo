using UnityEngine;
using UnityEngine.Events;

namespace Core.Res
{
    /// <summary>
    /// ��Դ��Ϣ��
    /// </summary>
    /// <typeparam name="T">��Դ����</typeparam>
    public class ResourcesInfo<T> : BaseResourcesInfo where T : Object
    {
        //�洢����Դ����
        private T _asset;
        //�Ƿ�Ҫɾ���ı�ʶ
        private bool _isDelete;
        //��ԴЭ�̶���
        private Coroutine _resCoroutine;
        //��Դ�ص�����
        public event UnityAction<T> ResCallBack;

        /// <summary>
        /// ��Դ
        /// </summary>
        public T Asset { get { return _asset; } set { _asset = value; } }

        /// <summary>
        /// �Ƿ�ɾ��
        /// </summary>
        public bool IsDelete { get { return _isDelete; } set { _isDelete = value; } }

        /// <summary>
        /// Э�̶���
        /// </summary>
        public Coroutine ResCoroutine { get { return _resCoroutine; } set { _resCoroutine = value; } }

        public ResourcesInfo(UnityAction<T> assetCallBack)
        {
            ++_refCount;
            ResCallBack += assetCallBack;
        }

        /// <summary>
        /// ִ�лص�
        /// </summary>
        public void Invoke()
        {
            ResCallBack?.Invoke(_asset);
            ResCallBack = null;
        }
    }
}
