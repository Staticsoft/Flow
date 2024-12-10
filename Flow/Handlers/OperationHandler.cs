namespace Staticsoft.Flow.Handlers;

public interface OperationHandler : Handler { }

public interface OperationHandler<Input, Output> : OperationHandler, Handler<Input, Output> { }
