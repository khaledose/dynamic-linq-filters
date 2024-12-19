using BenchmarkDotNet.Running;
using Expressions;

#if DEBUG
static List<Person> GetFilteredPeople(List<Person> people)
{
    var filters = new Dictionary<string, IEnumerable<object>>
    {
        { "Country", ["USA", "Mexico"] },
        { "Age", [20, 30] }
    };

    return people.FilterByPropertiesCached(filters).ToList();
}

static List<object> GetGroupedPercentages(List<Person> people)
{
    List<string> groupingColumns = ["Country", "Age"];
    var groupedPeople = people.GroupByProperties(groupingColumns);

    return [.. groupedPeople.Select(g => new { g.Key, Completeness = (float)((g.Sum(x => x.Missing) * 1.0) / (g.Sum(x => x.Expected) * 1.0)) }).OrderBy(g => g.Completeness)];
}

var people = DataGenerator.GeneratePeople(100000);

var filteredPeoiple = GetFilteredPeople(people);
Console.WriteLine(filteredPeoiple.Count);

var groupedPeople = GetGroupedPercentages(people);
Console.WriteLine(groupedPeople.Count);
#endif


#if RELEASE
BenchmarkRunner.Run<Benchmark>();
#endif