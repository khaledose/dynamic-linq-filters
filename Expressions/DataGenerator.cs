namespace Expressions;

public static class DataGenerator
{
	private static readonly string[] Names = ["John", "Emma", "Luis", "Sofia", "Wei", "Aisha", "Igor", "Maria", "James", "Yuki", "Hassan", "Anna", "Carlos", "Nina", "Ahmed"];
	private static readonly string[] Countries = ["USA", "UK", "Spain", "China", "Japan", "Brazil", "India", "Germany", "Canada", "Australia", "France", "Mexico", "Italy", "Russia", "Sweden"];
	private static readonly Random Random = new();

	public static List<Person> GeneratePeople(int count)
	{
		return Enumerable.Range(0, count)
			.Select(_ => new Person
			{
				Name = Names[Random.Next(Names.Length)],
				Country = Countries[Random.Next(Countries.Length)],
				Age = Random.Next(18, 80),
				Expected = Random.Next(80, 100),
				Missing = Random.Next(0, 100),
			})
			.ToList();
	}
}
