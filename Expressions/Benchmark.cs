using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace Expressions;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class Benchmark
{
    [Params(100, 10000, 100000)]
    public int DataSize { get; set; }

    private List<Person> People;
    private List<string> PropertiesToGroup;
    private Dictionary<string, IEnumerable<object>> Filters;

    [GlobalSetup]
    public void Setup()
    {
        People = DataGenerator.GeneratePeople(DataSize);
        PropertiesToGroup = ["Country", "Name", "Age"];
        Filters = new()
        {
            {"Country", ["USA", "Mexico" ]},
            {"Age", [20, 25, 30]},
        };
    }

    [Benchmark(Baseline = true)]
    public void BaselineNoOperations()
    {
        People.ToList();
    }

    [Benchmark]
    public void GroupPeopleStatic()
    {
        People
            .GroupBy(x => (x.Country, x.Name, x.Age))
            .ToList();
    }

    [Benchmark]
    public void GroupPeopleDynamic()
    {
        var groupingExpression = ExpressionBuilder.BuildGroupingExpression<Person>(PropertiesToGroup);
        People
            .GroupBy(groupingExpression)
            .ToList();
    }

    [Benchmark]
    public void FilterPeopleStatic()
    {
        People
            .Where(x => Filters["Country"].Contains(x.Country) && Filters["Age"].Contains(x.Age))
            .ToList();
    }

    [Benchmark]
    public void FilterPeopleDynamic()
    {
        var filteringExpression = ExpressionBuilder.BuildFilterExpression<Person>(Filters);
        People
            .Where(filteringExpression)
            .ToList();
    }
}
