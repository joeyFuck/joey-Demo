using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace ConsoleCoreMQConsumer
{
    /// <summary>
    /// 消费者
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            var queueName = "joey";
            //创建连接工厂
            ConnectionFactory factory = new ConnectionFactory
            {
                UserName = "guest",//用户名
                Password = "guest",//密码
                HostName = "192.168.101.81"//rabbitmq ip
            };

            //创建连接
            var connection = factory.CreateConnection();
            //创建通道
            var channel = connection.CreateModel();

            //事件基本消费者
            EventingBasicConsumer consumer = new EventingBasicConsumer(channel);

            //接收到消息事件
            consumer.Received += (ch, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body);
                Console.WriteLine($"收到消息： {message}");
                //确认该消息已被消费
                //消费者收到一个消息之后，需要发送一个应答，然后RabbitMQ才会将这个消息从队列中删除，如果消费者在消费过程中出现异常，断开连接切没有发送应答，那么RabbitMQ会将这个消息重新投递。 
                Console.WriteLine($"收到该消息[{ea.DeliveryTag}] 延迟500ms发送回执");
                Thread.Sleep(500);
                channel.BasicAck(ea.DeliveryTag, false);
                Console.WriteLine($"已发送回执[{ea.DeliveryTag}]");
            };
            //启动消费者 设置为手动应答消息
            channel.BasicConsume(queueName, false, consumer);
            Console.WriteLine("消费者已启动");
            Console.ReadKey();
            channel.Dispose();
            connection.Close();
        }
    }
}
