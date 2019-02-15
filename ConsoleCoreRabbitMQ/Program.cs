using RabbitMQ.Client;
using System;
using System.Text;

namespace ConsoleCoreRabbitMQ
{
    /// <summary>
    /// 定义生产者
    /// Direct Fanout Topic三种exchange demo
    /// (路由模式,发布订阅模式,通配符模式) 其实是同一种模式，另外两者只是发布订阅的变式，已满足某些通用的需求
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("hello rabbitMQ /producer");
            //DefaultExchange();
            //DirectExchange();
            //FanoutExchange();
            //TopicExchange();
            Console.ReadKey();
        }
        /// <summary>
        /// 默认的Exchanges为name:(AMQP default) type:direct
        /// </summary>
        static void DefaultExchange()
        {
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
            //声明一个队列
            channel.QueueDeclare("joey", false, false, false, null);//也可在管理台中手动添加queue http://localhost:15672/#/queues

            Console.WriteLine("\nRabbitMQ连接成功，请输入消息，输入exit退出！");

            string input;
            do
            {
                input = Console.ReadLine();

                var sendBytes = Encoding.UTF8.GetBytes(input);
                //发布消息
                channel.BasicPublish("", "joey", null, sendBytes);

            } while (input.Trim().ToLower() != "exit");
            channel.Close();
            connection.Close(); 
        }

        /// <summary>
        /// DirectExchange
        /// 所有发送到Direct Exchange的消息被转发到具有指定RouteKey的Queue。即其他queue不会收到消息
        /// 
        /// key： channel.BasicPublish(exchangeName, routeKey, null, sendBytes);消息只发送给routeKey对应的queue
        /// （可以多个queue用同一个routekey，这样多个queue也会同时收到消息）
        /// </summary>
        static void DirectExchange()
        {
            var exchangeName = "joey.amq.direct";
            var queueName = "joey";
            var queueName2 = "joey2";
            var routeKey = "route";
            var routeKey2 = "route2";
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
            //定义一个Direct类型交换机 add 
            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, false, false, null);
            //声明一个队列
            channel.QueueDeclare(queueName, false, false, false, null);//也可在管理台中手动添加queue http://localhost:15672/#/queues
            //将队列绑定到交换机 add
            channel.QueueBind(queueName, exchangeName, routeKey, null);
            channel.QueueBind(queueName2, exchangeName, routeKey2, null);
            Console.WriteLine("\nRabbitMQ连接成功，请输入消息，输入exit退出！");

            string input;
            do
            {
                input = Console.ReadLine();

                var sendBytes = Encoding.UTF8.GetBytes(input);
                //发布消息
                channel.BasicPublish(exchangeName, routeKey, null, sendBytes);

            } while (input.Trim().ToLower() != "exit");
            channel.Close();
            connection.Close();
        }

        /// <summary>
        /// FanoutExchange
        /// 所有发送到Fanout Exchange的消息都会被转发到与该Exchange 绑定(Binding)的所有Queue上。
        /// 
        /// Fanout Exchange 不需要处理RouteKey 。只需要简单的将队列绑定到exchange 上。
        /// 这样发送到exchange的消息都会被转发到与该交换机绑定的所有队列上。类似子网广播，每台子网内的主机都获得了一份复制的消息。
        /// 
        /// 所以，Fanout Exchange 转发消息是最快的。
        /// 
        /// key： channel.BasicPublish(exchangeName, routeKey, null, sendBytes);这里的routeKey就无关紧要了
        /// </summary>
        static void FanoutExchange()
        {
            var exchangeName = "joey.amq.fanout";
            var queueName = "joey";
            var queueName2 = "joey2";
            var routeKey = "route";
            var routeKey2 = "route2";
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
            //定义一个Direct类型交换机 add 
            channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout, false, false, null);
            //声明一个队列
            channel.QueueDeclare(queueName, false, false, false, null);//也可在管理台中手动添加queue http://localhost:15672/#/queues
            //将队列绑定到交换机 add 绑定两个queue
            channel.QueueBind(queueName, exchangeName, routeKey, null);
            channel.QueueBind(queueName2, exchangeName, routeKey2, null);
            Console.WriteLine("\nRabbitMQ连接成功，请输入消息，输入exit退出！");

            string input;
            do
            {
                input = Console.ReadLine();

                var sendBytes = Encoding.UTF8.GetBytes(input);
                //发布消息
                channel.BasicPublish(exchangeName, routeKey, null, sendBytes);

            } while (input.Trim().ToLower() != "exit");
            channel.Close();
            connection.Close();
        }

        /// <summary>
        /// TopicExchange
        /// 所有发送到Topic Exchange的消息被转发到能和Topic匹配的Queue上，
        /// 
        /// Exchange 将路由进行模糊匹配。可以使用通配符进行模糊匹配，符号“#”匹配一个或多个词，符号“*”匹配不多不少一个词。
        /// 因此“XiaoChen.#”能够匹配到“XiaoChen.pets.cat”，但是“XiaoChen.*” 只会匹配到“XiaoChen.money”。
        /// 
        /// 所以，Topic Exchange 使用非常灵活。
        /// 
        /// key： channel.BasicPublish(exchangeName, "route.one", null, sendBytes);;这里的routeKey与route.*匹配，跟route2.*不匹配，所以queue(joey2)不会收到消息
        /// </summary>
        static void TopicExchange()
        {
            var exchangeName = "joey.amq.topic";
            var queueName = "joey";
            var queueName2 = "joey2";
            var routeKey = "route.*";
            var routeKey2 = "route2.*";
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
            //定义一个Direct类型交换机 add 
            channel.ExchangeDeclare(exchangeName, ExchangeType.Topic, false, false, null);
            //声明一个队列
            channel.QueueDeclare(queueName, false, false, false, null);//也可在管理台中手动添加queue http://localhost:15672/#/queues
            //将队列绑定到交换机 add 绑定两个queue
            channel.QueueBind(queueName, exchangeName, routeKey, null);
            channel.QueueBind(queueName2, exchangeName, routeKey2, null);
            Console.WriteLine("\nRabbitMQ连接成功，请输入消息，输入exit退出！");

            string input;
            do
            {
                input = Console.ReadLine();

                var sendBytes = Encoding.UTF8.GetBytes(input);
                //发布消息
                channel.BasicPublish(exchangeName, "route.one", null, sendBytes);

            } while (input.Trim().ToLower() != "exit");
            channel.Close();
            connection.Close();
        }

    }
}
