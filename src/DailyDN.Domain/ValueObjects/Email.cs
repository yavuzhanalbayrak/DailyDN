namespace DailyDN.Domain.ValueObjects;

public class Email
{
    public string Value { get; } = string.Empty;

    private Email() { }

    public Email(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required");

        Value = email.Trim().ToLower();
    }

    public override string ToString() => Value;

    public static implicit operator string(Email email) => email.Value;
}
