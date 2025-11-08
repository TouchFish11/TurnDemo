using UnityEngine;
using UnityEngine.Events;

namespace Framework
{
    /// <summary>
    /// 数学计算工具类
    /// </summary>
    public static class MathUtility
    {
        /// <summary>
        /// 角度转弧度
        /// </summary>
        /// <param name="deg">角度</param>
        /// <returns>弧度</returns>
        public static float Deg2Rad(float deg)
        {
            return Mathf.Deg2Rad * deg;
        }

        /// <summary>
        /// 弧度转角度
        /// </summary>
        /// <param name="deg">弧度</param>
        /// <returns>角度</returns>
        public static float Rad2Deg(float rad)
        {
            return Mathf.Rad2Deg * rad;
        }

        /// <summary>
        /// 获取XZ平面两点的距离
        /// </summary>
        /// <param name="scrPos">源点</param>
        /// <param name="targetPos">目标点</param>
        /// <returns>XZ平面的两点距离</returns>
        public static float GetDistanceXZ(Vector3 scrPos, Vector3 targetPos)
        {
            scrPos.y = 0;
            targetPos.y = 0;
            return Vector3.Distance(scrPos, targetPos);
        }

        /// <summary>
        /// 判断XZ平面两点距离是否小于等于给定距离
        /// </summary>
        /// <param name="scrPos">源点</param>
        /// <param name="targetPos">目标点</param>
        /// <param name="dis">给定距离</param>
        /// <returns>true：小于给定距离；false：大于给定距离</returns>
        public static bool CheckXZ2PosDisIsLessDis(Vector3 scrPos, Vector3 targetPos, float dis)
        {
            return GetDistanceXZ(scrPos, targetPos) <= dis;
        }

        /// <summary>
        /// 获取XY平面两点的距离
        /// </summary>
        /// <param name="scrPos">源点</param>
        /// <param name="targetPos">目标点</param>
        /// <returns>XY平面的两点距离</returns>
        public static float GetDistanceXY(Vector3 scrPos, Vector3 targetPos)
        {
            scrPos.z = 0;
            targetPos.z = 0;
            return Vector3.Distance(scrPos, targetPos);
        }

        /// <summary>
        /// 判断XY平面两点距离是否小于等于给定距离
        /// </summary>
        /// <param name="scrPos">源点</param>
        /// <param name="targetPos">目标点</param>
        /// <param name="dis">给定距离</param>
        /// <returns>true：小于给定距离；false：大于给定距离</returns>
        public static bool CheckDistanceXY(Vector3 scrPos, Vector3 targetPos, float dis)
        {
            return GetDistanceXY(scrPos, targetPos) <= dis;
        }

        /// <summary>
        /// 判断世界坐标系点是否在屏幕外
        /// </summary>
        /// <param name="worldPos">世界坐标系点</param>
        /// <returns>true：在屏幕外；false：在屏幕内</returns>
        public static bool CheckWorldPosIsOutScreen(Vector3 worldPos)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            //判断是否在屏幕内
            if (screenPos.x >= 0 && screenPos.x <= Screen.width &&
                screenPos.y >= 0 && screenPos.y <= Screen.height)
                return false;
            return true;
        }

        /// <summary>
        /// 判断某个点是否在XZ平面的扇形范围内
        /// </summary>
        /// <param name="censterPos">中心点</param>
        /// <param name="forward">面朝向</param>
        /// <param name="targetPos">目标点</param>
        /// <param name="radius">半径</param>
        /// <param name="angle">角度</param>
        /// <returns>true：在扇形范围内；false：不在扇形范围内</returns>
        public static bool CheckPosIsInSectorRangeXZ(Vector3 censterPos, Vector3 forward, Vector3 targetPos, float radius, float angle)
        {
            censterPos.y = 0;
            forward.y = 0;
            targetPos.y = 0;

            return Vector3.Distance(censterPos, targetPos) <= radius && Vector3.Angle(forward, targetPos - censterPos) <= angle / 2f;
        }


        /// <summary>
        /// 射线检测-获取RaycastHit
        /// </summary>
        /// <param name="ray">射线</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="maxDistance">最大距离</param>
        /// <param name="layerMask">指定层级</param>
        /// <param name="queryTriggerInteraction">是否忽略触发器</param>
        public static void RayCast(Ray ray, UnityAction<RaycastHit> callBack, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
        {
            RaycastHit hitInfo;
            if(Physics.Raycast(ray, out hitInfo, maxDistance, layerMask, queryTriggerInteraction))
                callBack?.Invoke(hitInfo);
        }

        /// <summary>
        /// 射线检测-获取GameObject
        /// </summary>
        /// <param name="ray">射线</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="maxDistance">最大距离</param>
        /// <param name="layerMask">指定层级</param>
        /// <param name="queryTriggerInteraction">是否忽略触发器</param>
        public static void RayCast(Ray ray, UnityAction<GameObject> callBack, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, maxDistance, layerMask, queryTriggerInteraction))
                callBack?.Invoke(hitInfo.collider.gameObject);
        }

        /// <summary>
        /// 射线检测-获取脚本
        /// </summary>
        /// <param name="ray">射线</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="maxDistance">最大距离</param>
        /// <param name="layerMask">指定层级</param>
        /// <param name="queryTriggerInteraction">是否忽略触发器</param>
        public static void RayCast<T>(Ray ray, UnityAction<T> callBack, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, maxDistance, layerMask, queryTriggerInteraction))
                callBack?.Invoke(hitInfo.collider.GetComponent<T>());
        }

        /// <summary>
        /// 射线检测-获取多个RaycastHit
        /// </summary>
        /// <param name="ray">射线</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="maxDistance">最大距离</param>
        /// <param name="layerMask">指定层级</param>
        /// <param name="queryTriggerInteraction">是否忽略触发器</param>
        public static void RayCastAll(Ray ray, UnityAction<RaycastHit> callBack, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
        {
            RaycastHit[] raycastHits = Physics.RaycastAll(ray, maxDistance, layerMask, queryTriggerInteraction);
            for (int i = 0; i < raycastHits.Length; i++)
            {
                callBack?.Invoke(raycastHits[i]);
            }
        }

        /// <summary>
        /// 射线检测-获取多个GameObject
        /// </summary>
        /// <param name="ray">射线</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="maxDistance">最大距离</param>
        /// <param name="layerMask">指定层级</param>
        /// <param name="queryTriggerInteraction">是否忽略触发器</param>
        public static void RayCastAll(Ray ray, UnityAction<GameObject> callBack, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
        {
            RaycastHit[] raycastHits = Physics.RaycastAll(ray, maxDistance, layerMask, queryTriggerInteraction);
            for (int i = 0; i < raycastHits.Length; i++)
            {
                callBack?.Invoke(raycastHits[i].collider.gameObject);
            }
        }

        /// <summary>
        /// 射线检测-获取多个脚本
        /// </summary>
        /// <param name="ray">射线</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="maxDistance">最大距离</param>
        /// <param name="layerMask">指定层级</param>
        /// <param name="queryTriggerInteraction">是否忽略触发器</param>
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
