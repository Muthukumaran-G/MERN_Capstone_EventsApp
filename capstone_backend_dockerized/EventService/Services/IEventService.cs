public interface IEventService
{
    Task<List<Events>> GetEventsAsync();
}