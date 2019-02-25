using System;
using System.Diagnostics;
using System.Threading;

namespace ConsoleCoreLimitRequest
{
    /// <summary>
    /// 参考文章：https://www.cnblogs.com/zhanlang/p/10424757.html
    /// </summary>
    class Program
    {
        static LimitService l = new LimitService(1000, 1);
        static int totalOkCount;//全部pass的请求数量
        static int totalNoCount;//全部失败的请求数量 
        static int currentEndThreadIndex;//当前结束的线程索引

        static Stopwatch wgrobal = new Stopwatch();
        
        static void Main(string[] args)
        {
            wgrobal.Start();
            int threadCount = 50;//线程数量
            int threadRequestCount = 1000000;//每个线程的请求数量  100000000
            while (threadCount > 0)
            {
                Thread t = new Thread(s =>
                {
                    Limit(threadRequestCount);
                });
                t.Start();
                threadCount--;
            }  
            Console.Read(); 
        }
        static void Limit(int threadRequestCount)
        {
            int i = 0;
            int okCount = 0;
            int noCount = 0;
            Stopwatch w = new Stopwatch();
            w.Start();
            while (i < threadRequestCount)
            {
                var ret = l.IsContinue();
                if (ret)
                {
                    okCount++;
                }
                else
                {
                    noCount++;
                }
                i++;
            }
            w.Stop();
            totalOkCount = totalOkCount + okCount;
            totalNoCount = totalNoCount + noCount;

            currentEndThreadIndex++;
            Console.WriteLine($"共用{w.ElapsedMilliseconds}毫秒,允许：{okCount},  拦截：{noCount}");
            if (currentEndThreadIndex >= 50)
            {
                wgrobal.Stop();
                Console.WriteLine($"总耗时：{wgrobal.ElapsedMilliseconds}毫秒,允许总数量:{totalOkCount},拦截总数量:{totalNoCount}");
            }
        }
    }
}
