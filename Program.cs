using System;
using StackExchange.Redis;
namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var muxe = ConnectionMultiplexer.Connect("localhost");
            var db = muxe.GetDatabase();
            var JSONResult = db.CreateTransaction();
            db.Execute("JSON.SET", "dog:1", "$", "{\"name\":\"Honey\",\"breed\":\"Greyhound\"}");
            db.Execute("JSON.GET", "dog:1", "$.breed");
            //  JSONResult.ExecuteAsync("EXPIRE", "FOO", 50);
            JSONResult.Execute();
            Console.WriteLine("I'm ok");


        }
    }
}
