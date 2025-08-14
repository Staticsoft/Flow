namespace Staticsoft.Flow;

public interface ExternalInput<Seed, Data>
{
    ExternalInput<Data> Create(Seed input);
}

public interface ExternalInput<Data>
{
    string Id { get; }
    Task<Data> Get();
}

public interface ExternalInput
{
    Task Provide<Data>(string id, Data input);
}