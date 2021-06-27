namespace Studio808.BusinessLogic.Enums
{
    public enum OperationStatus
    {
        UnknownError = -1,
        Success = 0,
        AlreadyExists = 1,
        NotFound = 2,
        ExpectedStateMismatch = 3, // Concurrency issue due to race condition.
    }
}
