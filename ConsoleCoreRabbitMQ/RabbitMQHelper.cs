using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleCoreRabbitMQ
{
    public class RabbitMQHelper:IDisposable
    {
        private ConnectionFactory factory;//连接工厂
        private IConnection connection;//创建连接
        private IModel channel;//创建通道
        private string exchangeName;
        private string routeKey; 
         
        private RabbitMQHelper() { }

        public RabbitMQHelper(string _exchangeName,string _routeKey = "")
        { 
            this.factory = new ConnectionFactory
            {
                UserName = "guest",//用户名
                Password = "guest",//密码
                HostName = "192.168.101.81"//rabbitmq ip
            };

            this.connection = this.factory.CreateConnection();
            this.channel = this.connection.CreateModel();
            this.exchangeName = _exchangeName;
            this.routeKey = _routeKey; 
        }
 
        /// <summary>
        /// 设置Channel并返回
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="type">ExchangeType</param>
        /// <returns></returns>
        public IModel GetChannel(string queueName, string type = ExchangeType.Fanout)
        {
            //定义一个Direct类型交换机 add 
            channel.ExchangeDeclare(exchangeName, type, false, false, null); 
            //声明一个队列
            channel.QueueDeclare(queueName, false, false, false, null);
            //将队列绑定到交换机 add 绑定两个queue
            channel.QueueBind(queueName, exchangeName, routeKey, null);
  
            return channel; 
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_channel">通道实例 通过GetChannel获取</param>
        /// <param name="input">消息主体对象</param>
        public void SendMsg<T>(IModel _channel,T input) where T:class
        {
            var inputStr = Newtonsoft.Json.JsonConvert.SerializeObject(input);
            var sendBytes = Encoding.UTF8.GetBytes(inputStr);
            //发布消息
            _channel.BasicPublish(exchangeName, routeKey, null, sendBytes);
        }

        #region 资源释放
         
        /// <summary>
        /// 释放标记
        /// </summary>
        private bool disposed;

        /// <summary>
        /// 为了防止忘记显式的调用Dispose方法
        /// </summary>
        ~RabbitMQHelper()
        {
            //必须为false，到析构函数了说明已经由GC处理资源释放了，此时只需要手动释放非托管资源即可
            Dispose(false);
        }
         
        /// <summary>
        /// 执行与释放或重置非托管资源关联的应用程序定义的任务。
        /// </summary>
        public void Dispose()
        {
            //必须为true
            Dispose(true);
            //通知垃圾回收器不再调用终结器 以防止垃圾回收器对不需要终止的对象调用 Object.Finalize。
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 非必需的，只是为了更符合其他语言的规范，如C++、java
        /// </summary>
        public void Close()
        {
            Dispose();
        }

        /// <summary>
        /// 非密封类可重写的Dispose方法，方便子类继承时可重写
        /// </summary>
        /// <param name="disposing">bool参数的目的，是为了释放资源时区分对待托管资源和非托管资源
        /// 当传入true时代表要同时处理托管资源和非托管资源</param>
        ///
        /// 如果显式调用Dispose，那么类型就该按部就班的将自己的资源全部释放，
        /// 如果忘记了调用Dispose，那就假定自己的所有资源(哪怕是非普通类型)都交给GC了，所以不需要手动清理托管资源，只需要释放非托管资源。
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }
            //清理托管资源
            if (disposing)
            {
                if (this.factory != null)
                {
                    this.factory = null;
                }
                if (this.exchangeName != null)
                {
                    this.exchangeName = null;
                }
                if (this.routeKey != null)
                {
                    this.routeKey = null;
                } 
            }
            //清理非托管资源
            if (this.channel != null)
            {
                this.channel.Close();
                this.channel = null;
            }
            if (this.connection != null)
            { 
                this.connection.Close();
                this.connection = null;
            }

            //告诉自己已经被释放
            this.disposed = true;
        }

        #endregion 
    }
}
