namespace DailyDN.Application.Dtos.Otp
{
    public class OtpDto
    {
        public int Otp { get; private set; }
        public Guid OtpGuid { get; private set; }

        public OtpDto()
        {
            this.Otp = Random.Shared.Next(100000, 999999);
            this.OtpGuid = Guid.NewGuid();
        }
    }
}