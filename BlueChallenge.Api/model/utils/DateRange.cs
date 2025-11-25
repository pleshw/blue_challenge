namespace BlueChallenge.Api.Model.Utils;

public record class DateRange
{
    public DateTime Start { get; init; }
    public DateTime End { get; init; }


    public DateRange(DateTime start, DateTime end)
    {
        if (start > end)
            throw new ArgumentException("Start must be earlier than or equal to End.");

        Start = start;
        End = end;
    }
}