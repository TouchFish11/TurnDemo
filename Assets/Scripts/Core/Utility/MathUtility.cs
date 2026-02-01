using UnityEngine;
using UnityEngine.Events;

namespace Core.Utility
{
    /// <summary>
    /// ��ѧ���㹤����
    /// </summary>
    public static class MathUtility
    {
        /// <summary>
        /// �Ƕ�ת����
        /// </summary>
        /// <param name="deg">�Ƕ�</param>
        /// <returns>����</returns>
        public static float Deg2Rad(float deg)
        {
            return Mathf.Deg2Rad * deg;
        }

        /// <summary>
        /// ����ת�Ƕ�
        /// </summary>
        /// <param name="deg">����</param>
        /// <returns>�Ƕ�</returns>
        public static float Rad2Deg(float rad)
        {
            return Mathf.Rad2Deg * rad;
        }

        /// <summary>
        /// ��ȡXZƽ������ľ���
        /// </summary>
        /// <param name="scrPos">Դ��</param>
        /// <param name="targetPos">Ŀ���</param>
        /// <returns>XZƽ����������</returns>
        public static float GetDistanceXZ(Vector3 scrPos, Vector3 targetPos)
        {
            scrPos.y = 0;
            targetPos.y = 0;
            return Vector3.Distance(scrPos, targetPos);
        }

        /// <summary>
        /// �ж�XZƽ����������Ƿ�С�ڵ��ڸ�������
        /// </summary>
        /// <param name="scrPos">Դ��</param>
        /// <param name="targetPos">Ŀ���</param>
        /// <param name="dis">��������</param>
        /// <returns>true��С�ڸ������룻false�����ڸ�������</returns>
        public static bool CheckXZ2PosDisIsLessDis(Vector3 scrPos, Vector3 targetPos, float dis)
        {
            return GetDistanceXZ(scrPos, targetPos) <= dis;
        }

        /// <summary>
        /// ��ȡXYƽ������ľ���
        /// </summary>
        /// <param name="scrPos">Դ��</param>
        /// <param name="targetPos">Ŀ���</param>
        /// <returns>XYƽ����������</returns>
        public static float GetDistanceXY(Vector3 scrPos, Vector3 targetPos)
        {
            scrPos.z = 0;
            targetPos.z = 0;
            return Vector3.Distance(scrPos, targetPos);
        }

        /// <summary>
        /// �ж�XYƽ����������Ƿ�С�ڵ��ڸ�������
        /// </summary>
        /// <param name="scrPos">Դ��</param>
        /// <param name="targetPos">Ŀ���</param>
        /// <param name="dis">��������</param>
        /// <returns>true��С�ڸ������룻false�����ڸ�������</returns>
        public static bool CheckDistanceXY(Vector3 scrPos, Vector3 targetPos, float dis)
        {
            return GetDistanceXY(scrPos, targetPos) <= dis;
        }

        /// <summary>
        /// �ж���������ϵ���Ƿ�����Ļ��
        /// </summary>
        /// <param name="worldPos">��������ϵ��</param>
        /// <returns>true������Ļ�⣻false������Ļ��</returns>
        public static bool CheckWorldPosIsOutScreen(Vector3 worldPos)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            //�ж��Ƿ�����Ļ��
            if (screenPos.x >= 0 && screenPos.x <= Screen.width &&
                screenPos.y >= 0 && screenPos.y <= Screen.height)
                return false;
            return true;
        }

        /// <summary>
        /// �ж�ĳ�����Ƿ���XZƽ������η�Χ��
        /// </summary>
        /// <param name="censterPos">���ĵ�</param>
        /// <param name="forward">�泯��</param>
        /// <param name="targetPos">Ŀ���</param>
        /// <param name="radius">�뾶</param>
        /// <param name="angle">�Ƕ�</param>
        /// <returns>true�������η�Χ�ڣ�false���������η�Χ��</returns>
        public static bool CheckPosIsInSectorRangeXZ(Vector3 censterPos, Vector3 forward, Vector3 targetPos, float radius, float angle)
        {
            censterPos.y = 0;
            forward.y = 0;
            targetPos.y = 0;

            return Vector3.Distance(censterPos, targetPos) <= radius && Vector3.Angle(forward, targetPos - censterPos) <= angle / 2f;
        }


        /// <summary>
        /// ���߼��-��ȡRaycastHit
        /// </summary>
        /// <param name="ray">����</param>
        /// <param name="callBack">�ص�����</param>
        /// <param name="maxDistance">������</param>
        /// <param name="layerMask">ָ���㼶</param>
        /// <param name="queryTriggerInteraction">�Ƿ���Դ�����</param>
        public static void RayCast(Ray ray, UnityAction<RaycastHit> callBack, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
        {
            RaycastHit hitInfo;
            if(Physics.Raycast(ray, out hitInfo, maxDistance, layerMask, queryTriggerInteraction))
                callBack?.Invoke(hitInfo);
        }

        /// <summary>
        /// ���߼��-��ȡGameObject
        /// </summary>
        /// <param name="ray">����</param>
        /// <param name="callBack">�ص�����</param>
        /// <param name="maxDistance">������</param>
        /// <param name="layerMask">ָ���㼶</param>
        /// <param name="queryTriggerInteraction">�Ƿ���Դ�����</param>
        public static void RayCast(Ray ray, UnityAction<GameObject> callBack, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, maxDistance, layerMask, queryTriggerInteraction))
                callBack?.Invoke(hitInfo.collider.gameObject);
        }

        /// <summary>
        /// ���߼��-��ȡ�ű�
        /// </summary>
        /// <param name="ray">����</param>
        /// <param name="callBack">�ص�����</param>
        /// <param name="maxDistance">������</param>
        /// <param name="layerMask">ָ���㼶</param>
        /// <param name="queryTriggerInteraction">�Ƿ���Դ�����</param>
        public static void RayCast<T>(Ray ray, UnityAction<T> callBack, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, maxDistance, layerMask, queryTriggerInteraction))
                callBack?.Invoke(hitInfo.collider.GetComponent<T>());
        }

        /// <summary>
        /// ���߼��-��ȡ���RaycastHit
        /// </summary>
        /// <param name="ray">����</param>
        /// <param name="callBack">�ص�����</param>
        /// <param name="maxDistance">������</param>
        /// <param name="layerMask">ָ���㼶</param>
        /// <param name="queryTriggerInteraction">�Ƿ���Դ�����</param>
        public static void RayCastAll(Ray ray, UnityAction<RaycastHit> callBack, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
        {
            RaycastHit[] raycastHits = Physics.RaycastAll(ray, maxDistance, layerMask, queryTriggerInteraction);
            for (int i = 0; i < raycastHits.Length; i++)
            {
                callBack?.Invoke(raycastHits[i]);
            }
        }

        /// <summary>
        /// ���߼��-��ȡ���GameObject
        /// </summary>
        /// <param name="ray">����</param>
        /// <param name="callBack">�ص�����</param>
        /// <param name="maxDistance">������</param>
        /// <param name="layerMask">ָ���㼶</param>
        /// <param name="queryTriggerInteraction">�Ƿ���Դ�����</param>
        public static void RayCastAll(Ray ray, UnityAction<GameObject> callBack, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
        {
            RaycastHit[] raycastHits = Physics.RaycastAll(ray, maxDistance, layerMask, queryTriggerInteraction);
            for (int i = 0; i < raycastHits.Length; i++)
            {
                callBack?.Invoke(raycastHits[i].collider.gameObject);
            }
        }

        /// <summary>
        /// ���߼��-��ȡ����ű�
        /// </summary>
        /// <param name="ray">����</param>
        /// <param name="callBack">�ص�����</param>
        /// <param name="maxDistance">������</param>
        /// <param name="layerMask">ָ���㼶</param>
        /// <param name="queryTriggerInteraction">�Ƿ���Դ�����</param>
        public static void RayCastAll<T>(Ray ray, UnityAction<T> callBack, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
        {
            RaycastHit[] raycastHits = Physics.RaycastAll(ray, maxDistance, layerMask, queryTriggerInteraction);
            for (int i = 0; i < raycastHits.Length; i++)
            {
                callBack?.Invoke(raycastHits[i].collider.GetComponent<T>());
            }
        }
    }
}
