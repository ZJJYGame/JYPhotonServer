namespace AscensionProtocol
{
    public enum ReturnCode : short
    {
        Success = 0,
        Error = 1,
        Fail = 2,
        Empty = 3,
        InvalidOperationParameter=4,
        InvalidOperation =5,
        ItemAlreadyExists,
        ItemNotFound,
    }
}
