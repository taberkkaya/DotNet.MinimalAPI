namespace Minimal.API
{
    public record Person(string FullName);
    public sealed class PeopleService
    {
        private readonly List<Person> _people = new()
        {
            new Person("Ataberk Kaya"),
            new Person("Mustafa Kemal"),
            new Person("Taner Saydam")
        };

        public IEnumerable<Person> Search(string searchTerm)
        {
            return _people.Where(x => x.FullName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        }
    }
}
