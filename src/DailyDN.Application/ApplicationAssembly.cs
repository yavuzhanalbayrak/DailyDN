using System.Reflection;

namespace DailyDN.Application
{
    public static class ApplicationAssembly
    {
        public static readonly Assembly Instance = typeof(ApplicationAssembly).Assembly;
    }
}