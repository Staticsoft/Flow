namespace Staticsoft.Flow;

public interface ExternalInput<Seed, Data>
{
    Task<ExternalInput<Data>> Create(Seed input);
}

public interface ExternalInput<Data>
{
    string Id { get; }
    Data Input { get; }
}

public interface ExternalInput
{
    Task Provide<Data>(string id, Data input);
}