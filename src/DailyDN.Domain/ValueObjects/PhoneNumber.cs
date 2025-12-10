namespace DailyDN.Domain.ValueObjects;

public class PhoneNumber
{
    public string Value { get; } = string.Empty;

    private PhoneNumber() { }

    public PhoneNumber(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("Phone number is required");

        Value = phone;
    }

    public override string ToString() => Value;

    public static implicit operator string(PhoneNumber phone) => phone.Value;
}
