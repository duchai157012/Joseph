namespace BuildingBlocks.Common.Exceptions;

public class UnauthorizedException : BaseException
{
    public UnauthorizedException(string message)
        : base(message, 401, "UNAUTHORIZED") { }
}
