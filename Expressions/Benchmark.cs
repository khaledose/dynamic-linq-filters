using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace Expressions;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class Benchmark
{
    public List<Person> People = DataGenerator.GeneratePeople(10000);
    public List<string> Properties = ["Country", "Age"];
    public Dictionary<string, IEnumerable<object>> Filters = new Dictionary<string, IEnumerable<object>>
    {
        {"Country", ["USA", "Mexico"] },
        {"Age", [20,25,30] }
    };

    [Benchmark]
    public void GroupPeople()
    {
        var groupings = People.GroupByProperties(Properties);
    }

    [Benchmark]
    public void GroupPeopleCached()
    {
        var groupings = People.GroupByPropertiesCached(Properties);
    }

    [Benchmark]
    public void FilterPeople()
    {
        var filtered = People.FilterByProperties(Filters);
    }

    [Benchmark]
    public void FilterPeopleCached()
    {
        var filtered = People.FilterByPropertiesCached(Filters);
    }
}
