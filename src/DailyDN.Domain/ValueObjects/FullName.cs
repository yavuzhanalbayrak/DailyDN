namespace DailyDN.Domain.ValueObjects;

public class FullName
{
    public string Name { get; } = string.Empty;
    public string Surname { get; } = string.Empty;

    private FullName() { } // EF Core

    public FullName(string name, string surname)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required");

        if (string.IsNullOrWhiteSpace(surname))
            throw new ArgumentException("Surname is required");

        Name = name;
        Surname = surname;
    }

    public override string ToString() => $"{Name} {Surname}";
}
