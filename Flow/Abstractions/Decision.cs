namespace Staticsoft.Flow;

public interface Decision
{
    Task<string> Create(string id);
    Task Make(string id, string choice);
}
