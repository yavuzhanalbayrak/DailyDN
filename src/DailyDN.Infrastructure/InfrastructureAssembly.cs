using System.Reflection;

namespace DailyDN.Infrastructure
{
    public static class InfrastructureAssembly
    {
        public static readonly Assembly Instance = typeof(InfrastructureAssembly).Assembly;
    }
}