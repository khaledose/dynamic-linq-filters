using BenchmarkDotNet.Running;
using Expressions;

#if DEBUG

var people = DataGenerator.GeneratePeople(100000);

var rawFilters = new Dictionary<string, IEnumerable<object>>
{
    {"Country", ["USA", "Canada"] },
    {"Age", [20, 21, 22, 23] },
};
var filterExpression = ExpressionBuilder.BuildFilterExpression<Person>(rawFilters);

List<string> rawGroupings = ["Country", "Age"];
var groupingExpression = ExpressionBuilder.BuildGroupingExpression<Person>(rawGroupings);

var groupedPeople = people
    .Where(filterExpression)
    .GroupBy(groupingExpression)
    .Select(g => new {g.Key, Completeness = (float)g.Sum(x => x.Missing) / (float)g.Sum(x => x.Expected)})
    .ToList();

Console.WriteLine(groupedPeople.Count);
#endif


#if RELEASE
BenchmarkRunner.Run<Benchmark>();
#endif