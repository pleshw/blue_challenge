namespace BlueChallenge.Api.Model.Utils;

public class HourRange
{
    public TimeSpan Start { get; init; }
    public TimeSpan End { get; init; }

    public HourRange(TimeSpan start, TimeSpan end)
    {
        if (start > end)
            throw new ArgumentException("Start must be earlier than or equal to End.");

        Start = start;
        End = end;
    }
}