public class Events
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }
    public string ImageUrl { get; set; }
    public string Date { get; set; }
    public string Time { get; set; }
}

public class TicketmasterResponse
{
    public Embedded _embedded { get; set; }
}

public class Embedded
{
    public List<EventData> events { get; set; }
}

public class EventData
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }
    public List<ImageData> Images { get; set; }
    public EventDates Dates { get; set; }
}

public class ImageData
{
    public string Url { get; set; }
}

public class EventDates
{
    public Start Start { get; set; }
}

public class Start
{
    public string LocalDate { get; set; }
    public string LocalTime { get; set; }
}
