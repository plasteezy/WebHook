namespace WebHook.Common.Lib.Enum
{
    public enum Status
    {
        Pending,
        Confirmed,
        Active, //For services only
        Inactive,
        Faulty,
        Unknown
    }
}