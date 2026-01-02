namespace BuildingBlocks.Common.Exceptions;

public class NotFoundException : BaseException
{
    public NotFoundException(string message)
        : base(message, 404, "NOT_FOUND") { }

    public NotFoundException(string entity, object id)
        : base($"{entity} with id '{id}' was not found", 404, "NOT_FOUND") { }
}
