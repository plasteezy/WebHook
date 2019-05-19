namespace WebHook.Common.Lib.Enum
{
    public enum Transition
    {
        New,
        Acknowledged,
        Authorized,
        Routed,
        CostDetermined,
        Processed,
        Error,
        ErrorHandled,
        ErrorLogged,
        Finished
    }
}