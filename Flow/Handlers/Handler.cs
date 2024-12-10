namespace Staticsoft.Flow.Handlers;

public interface Handler
{
    string Name
        => GetType().Name;
    Type InputType { get; }
    Type OutputType { get; }
}

public interface Handler<Input, Output> : Handler
{
    Task<Output> Execute(Input input);

    Type Handler.InputType
        => typeof(Input);
    Type Handler.OutputType
        => typeof(Output);
}