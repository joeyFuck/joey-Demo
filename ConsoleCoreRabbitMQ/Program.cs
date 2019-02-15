using RabbitMQ.Client;
using System;
using System.Collections.Generic;
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
            TestMyRabbitMQHelper();
            //DefaultExchange();
            //DirectExchange();
            //FanoutExchange();
            //TopicExchange();
            Console.ReadKey();
        }

        static void TestMyRabbitMQHelper()
        {
            using (RabbitMQHelper rm = new RabbitMQHelper(_exchangeName: "joey.amq.fanout"))
            {
                var channel = rm.GetChannel("joey");
                var input = new List<InputModel> { new InputModel { id = "123", name = "Ed Sheeran" } };
                rm.SendMsg<List<InputModel>>(channel, input);
                Console.WriteLine("已成功发送：" + Newtonsoft.Json.JsonConvert.SerializeObject(input));
            }
            GC.Collect();
            //由于rm已经手动释放资源且在dispose中GC.SuppressFinalize(this);通知垃圾回收器不再调用终结器。
            //断点不会进入到析构函数中。


            //var action = new Action(()=> {
            //    RabbitMQHelper rm = new RabbitMQHelper(_exchangeName: "joey.amq.fanout");
            //    var channel = rm.GetChannel("joey");
            //    var input = new List<InputModel> { new InputModel { id = "123", name = "Ed Sheeran" } };
            //    rm.SendMsg<List<InputModel>>(channel, input);
            //    Console.WriteLine("已成功发送：" + Newtonsoft.Json.JsonConvert.SerializeObject(input));
            //}); 
            //action();

            //GC.Collect();//手动调用GC，会调用所有即将回收的对象的析构函数。（虽然对象rm已经出了作用域被当做垃圾扔进了第0代，但是还不会进行垃圾回收）

            //满足下面条件之一时，GC自动调用
            //1 系统具有较低的物理内存；

            //2 由托管堆上已分配的对象使用的内存超出了可接受的范围；

            //3 手动调用GC.Collect方法，但几乎所有的情况下，我们都不必调用，因为垃圾回收器会自动调用它，但在上面的例子中，为了体验一下不及时回收垃圾带来的危害，所以手动调用了GC.Collect，大家也可以仔细体会一下运行这个方法带来的不同。

            /*代的概念：一共分3代：0代、1代、2代。而这三代，相当于是三个队列容器，第0代包含的是一些短期生存的对象，上面的例子fs就是个短期对象，当方法执行完后，fs就被丢到了GC的第0代，但不进行垃圾回收，只有当第0代满了的时候，GC才会工作对其进行回收。*/
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
            channel.QueueDeclare(queueName2, false, false, false, null);
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
            //定义一个Fanout类型交换机 add 
            channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout, false, false, null);
            //声明一个队列
            channel.QueueDeclare(queueName, false, false, false, null);//也可在管理台中手动添加queue http://localhost:15672/#/queues
            channel.QueueDeclare(queueName2, false, false, false, null);
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
            //定义一个Topic类型交换机 add 
            channel.ExchangeDeclare(exchangeName, ExchangeType.Topic, false, false, null);
            //声明一个队列
            channel.QueueDeclare(queueName, false, false, false, null);//也可在管理台中手动添加queue http://localhost:15672/#/queues
            channel.QueueDeclare(queueName2, false, false, false, null);
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

    class InputModel
    {
        public string id { get; set; }
        public string name { get; set; }
    }
}
