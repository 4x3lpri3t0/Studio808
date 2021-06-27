namespace BusinessLogic.Enums
{
    public enum OperationStatus
    {
        UnknownError = -1,
        Done = 0,
        AlreadyExists = 1,
        NotFound = 2,
        ExpectedStateMismatch = 3, // Concurrency issue due to race condition.
    }
}
