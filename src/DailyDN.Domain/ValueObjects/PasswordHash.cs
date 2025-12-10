namespace DailyDN.Domain.ValueObjects;

public class PasswordHash
{
    public string Value { get; } = string.Empty;

    public PasswordHash() { }

    public PasswordHash(string hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
            throw new ArgumentException("Password hash cannot be empty");

        Value = hash;
    }

    public override string ToString() => Value;

    public static implicit operator string(PasswordHash hash) => hash.Value;
}
