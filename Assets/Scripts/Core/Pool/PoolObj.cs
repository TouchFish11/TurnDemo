using System.Collections.Generic;
using Core.Global;
using UnityEngine;

namespace Core.Pool
{
    /// <summary>
    /// �̳�Mono�Ļ������
    /// </summary>
    public class PoolObj
    {
        //�洢δʹ�ö����ջ
        private readonly Stack<GameObject> _unUsedObjStack = new Stack<GameObject>();
        //�������ĸ�����
        private GameObject _parentObj;

        public PoolObj(GameObject rootObj, string poolObjName)
        {
            if (_parentObj == null && GlobalSettings.Instance.isOpenLayout)
            {
                _parentObj = new GameObject(poolObjName);
                _parentObj.transform.SetParent(rootObj.transform, false);
            }
        }

        /// <summary>
        /// ��ȡδʹ�õĶ���
        /// </summary>
        /// <returns>����Ķ���</returns>
        public GameObject Get()
        {
            GameObject obj = null;
            if (_unUsedObjStack.Count > 0)
            {
                //��ȡδʹ�õĶ���
                obj = _unUsedObjStack.Pop();
                //�������
                obj.SetActive(true);
                //�Ͽ����ӹ�ϵ
                obj.transform.SetParent(null, false);
            }
            return obj;
        }

        /// <summary>
        /// �������
        /// </summary>
        /// <param name="obj">��ʹ�õĶ���</param>
        public void Push(GameObject obj)
        {
            if (GlobalSettings.Instance.isOpenLayout)
            {
                //���ø�����
                obj.transform.SetParent(_parentObj.transform, false);
            }
            //ʧ�����
            obj.SetActive(false);
            //�洢������
            _unUsedObjStack.Push(obj);
        }

        /// <summary>
        /// ����
        /// </summary>
        public void Clear()
        {
            _unUsedObjStack.Clear();
            GameObject.Destroy(_parentObj);
            _parentObj = null;
        }

        /// <summary>
        /// δʹ�ö�������
        /// </summary>
        public int UnUsedCount => _unUsedObjStack.Count;
    }
}
