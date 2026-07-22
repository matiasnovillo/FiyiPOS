namespace FiyiPOS.Helpers
{
    public static class InjectionGuard
    {
        public static void EnsureServicesAreInjected(params object?[] services)
        {
            if (services.Any(s => s == null))
                throw new InvalidOperationException("Some required services were not injected.");
        }
    }
}
