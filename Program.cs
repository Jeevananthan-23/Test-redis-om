using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using Redis.OM;
using Redis.OM.Modeling;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

internal class Program
{
    static void Main(string[] args)
    {
        BenchmarkRunner.Run<BenchmarkTest>();
    }
}

[Document(StorageType = StorageType.Json, IndexName = "person-idx")]
public partial class Person
{
    [RedisIdField]
    [Indexed]
    public string Id { get; set; }
    public List<Address> Addresses { get; set; }
    public int DepartmentNumber { get; internal set; }
    public int Sales { get; internal set; }
    public int Age { get; internal set; }
    public double Height { get; internal set; }
    public double SalesAdjustment { get; internal set; }
    public string Name { get; internal set; }
}

[Document(IndexName = "address-idx", StorageType = StorageType.Json)]
public partial class Address
{
    [Indexed] public string City { get; set; }
    [Indexed] public string State { get; set; }
    [Indexed(CascadeDepth = 1)]
    public Address ForwardingAddress { get; set; }
    [Indexed] public GeoLoc? Location { get; set; }
    [Indexed] public Guid? Guid { get; set; }
}


[MemoryDiagnoser]
[Orderer(summaryOrderPolicy: SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class BenchmarkTest
{
    [Benchmark]
    public void InsertTest()
    {
        var cm = ConnectionMultiplexer.Connect("localhost");
        var provider = new RedisConnectionProvider(cm);
        var collection = provider.RedisCollection<Person>();
        var names = new[] { "Hassein", "Zoro", "Aegorn", "Jeeva", "Ajith", "Joe", "Mark", "Otto" };
        var rand = new Random();
        var people = new List<Person>();
        for (var i = 0; i < 1000; i++)
        {
            people.Add(new Person
            {
                Name = names[rand.Next(0, names.Length)],
                DepartmentNumber = rand.Next(1, 4),
                Sales = rand.Next(50000, 1000000),
                Age = rand.Next(17, 21),
                Height = 58.0 + rand.NextDouble() * 15,
                SalesAdjustment = rand.NextDouble()
            }
            );
        }
        collection.Insert(people);

        collection.Delete(people);
    }


    [Benchmark]
    public async Task InsertTest2()
    {
        var cm = ConnectionMultiplexer.Connect("localhost");
        var provider = new RedisConnectionProvider(cm);
        var collection = provider.RedisCollection<Person>();
        var names = new[] { "Hassein", "Zoro", "Aegorn", "Jeeva", "Ajith", "Joe", "Mark", "Otto" };
        var rand = new Random();
        var people = new List<Person>();
        for (var i = 0; i < 1000; i++)
        {
            people.Add(new Person
            {
                Name = names[rand.Next(0, names.Length)],
                DepartmentNumber = rand.Next(1, 4),
                Sales = rand.Next(50000, 1000000),
                Age = rand.Next(17, 21),
                Height = 58.0 + rand.NextDouble() * 15,
                SalesAdjustment = rand.NextDouble()
            }
            );
        }
        await collection.Insert(people);

        await collection.DeleteAsync(people);
    }

}