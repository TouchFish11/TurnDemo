using System;
using UnityEngine;

namespace Framework.YieldInstruction
{
    /// <summary>
    /// 等待条件满或超时
    /// </summary>
    public class WaitUntilOrTimeout : CustomYieldInstruction
    {
        private Func<bool> _condition;
        private float _timeoutTime;
        private bool _hasTimeout;

        public override bool keepWaiting
        {
            get
            {
                //条件满足，停止等待
                if (_condition())
                {
                    return false;
                }

                //超时，停止等待
                if (Time.time > _timeoutTime)
                {
                    //改变标识
                    this._hasTimeout = true;
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// 是否超时
        /// </summary>
        public bool HasTimeout => this._hasTimeout;

        public WaitUntilOrTimeout(Func<bool> condition, float timeout)
        {
            this._condition = condition;
            this._timeoutTime = Time.time + timeout;
            this._hasTimeout = false;
        }
    }
}
