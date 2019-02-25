using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleCoreLimitRequest
{
    /// <summary>
    /// 限流组件,采用数组做为一个环
    /// </summary>
    public class LimitService
    {
        //当前指针的位置
        int currentIndex = 0;
        //限制的时间的秒数，即：x秒允许多少请求
        int limitTimeSencond = 1;
        //请求环的容器数组
        //DateTime?[] requestRing = null;
        double?[] requestRing = null;
        //容器改变或者移动指针时候的锁
        object objLock = new object();

        /// <summary>
        /// countPerSecond/_limitTimeSencond
        /// 允许的并发数量/单位时间
        /// </summary>
        /// <param name="countPerSecond">允许的并发数量</param>
        /// <param name="_limitTimeSencond">单位时间</param>
        public LimitService(int countPerSecond, int _limitTimeSencond)
        {
            requestRing = new double?[countPerSecond];//new DateTime?[countPerSecond];
            limitTimeSencond = _limitTimeSencond*1000;
        }

        //程序是否可以继续
        public bool IsContinue()
        {
            lock (objLock)
            {
                var currentNode = requestRing[currentIndex];
                //如果当前节点的值加上设置的秒 超过当前时间，说明超过限制
                //if (currentNode != null && currentNode.Value.AddSeconds(limitTimeSencond) > DateTime.Now.Millisecond)
                var ts = DateTime.Now - new DateTime(1970, 1, 1);
                var nowTotalMilliseconds = ts.TotalMilliseconds;
                if (currentNode != null && currentNode.Value + (limitTimeSencond) > nowTotalMilliseconds)
                {
                    return false;
                }
                //当前节点设置为当前时间
                requestRing[currentIndex] = nowTotalMilliseconds;
                //指针移动一个位置
                MoveNextIndex(ref currentIndex);
            }
            return true;
        }
        //改变每秒可以通过的请求数
        public bool ChangeCountPerSecond(int countPerSecond)
        {
            lock (objLock)
            {
                //requestRing = new DateTime?[countPerSecond];
                requestRing = new double?[countPerSecond];
                currentIndex = 0;
            }
            return true;
        }

        //指针往前移动一个位置
        private void MoveNextIndex(ref int currentIndex)
        {
            if (currentIndex != requestRing.Length - 1)
            {
                currentIndex = currentIndex + 1;
            }
            else
            {
                currentIndex = 0;
            }
        }
    }
}
