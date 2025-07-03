using System.Reflection;

namespace DailyDN.API
{
    public static class ApiAssembly
    {
        public static readonly Assembly Instance = typeof(ApiAssembly).Assembly;
    }
}