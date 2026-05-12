using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public class Solution
    {
        #region 两数之和
        
        public int[] TwoSum(int[] nums, int target)
        {
            Dictionary<int, int> dict = new Dictionary<int, int>();

            for (var i = 0; i < nums.Length; i++)
            {
                var sub = target - nums[i];
                if (dict.TryGetValue(sub, out var value))
                {
                    return new[] { i, value };
                }
                
                dict.TryAdd(nums[i], i);
            }
            
            return Array.Empty<int>();
        }
        
        
        #endregion
    }
    
    #region 相交链表

    public class ListNode 
    {
        public int val;
        public ListNode next;

        public ListNode(int x)
        {
            val = x;
        }
    }
    
    public ListNode GetIntersectionNode(ListNode headA, ListNode headB)
    {
        if (headA == null || headB == null) 
            return null;
        
        ListNode pA = headA;
        ListNode pB = headB;

        while (pA != pB)
        {
            pA = pA == null ? headB : pA.next;
            pB = pB == null ? headA : pB.next;
        }
        
        return pA;
    }

    #endregion

    #region 最小栈

    public class MinStack
    {
        // 主栈
        private readonly Stack<int> _mainStack;
        // 最小栈（辅助栈）
        private readonly Stack<int> _minStack;
        
        public MinStack()
        {
            _mainStack =  new Stack<int>();
            _minStack = new Stack<int>();
        }

        public void Push(int val)
        {
            _mainStack.Push(val);
            if (_minStack.Count == 0 || val <= _minStack.Peek())
            {
                _minStack.Push(val);
            }
        }

        public int Pop()
        {
            if (_mainStack.Count == 0)
                throw new InvalidOperationException();
            
            var obj=  _mainStack.Pop();
            if (obj == _minStack.Peek())
            {
                _minStack.Pop();
            }
            
            return obj;
        }

        public int Peek()
        {
            return _mainStack.Peek();
        }

        public int GetMin()
        {
            return _minStack.Peek();
        }
    }
    
    #endregion
    
    #region 用栈实现队列

    public class TwoQueueStack
    {
        private readonly Stack<int> inStack;
        private readonly Stack<int> outStack;
        
        public TwoQueueStack()
        {
            inStack =  new Stack<int>();
            outStack = new Stack<int>();
        }
        
        public void push(int val)
        {
            inStack.Push(val);
        }

        public int Pop()
        {
            EnSureOutStack();
            return outStack.Pop();
        }

        public int Peek()
        {
            EnSureOutStack();
            return outStack.Peek();
        }
        
        private void EnSureOutStack()
        {
            if (outStack.Count != 0) 
                return;
            while (inStack.Count > 0)
            {
                outStack.Push(inStack.Pop());
            }
        }
        
        public bool Empty()
        {
            return inStack.Count == 0 && outStack.Count == 0;
        }
    }
    
    #endregion
}
