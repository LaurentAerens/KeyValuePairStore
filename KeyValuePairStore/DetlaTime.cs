/// <summary>
/// Represents a time interval with years, months, days, and hours.
/// </summary>
public class DeltaTime
{
    public int Years { get; private set; }
    public int Months { get; private set; }
    public int Days { get; private set; }
    public int Hours { get; private set; }
    public bool IsOff { get { return Years == 0 && Months == 0 && Days == 0 && Hours == 0; } }


    /// <summary>
    /// Initializes a new instance of the DeltaTime class with all properties set to 0.
    /// </summary>
    public DeltaTime(Dictionary<string, object>? dictionary)
    {
        Years = 0;
        Months = 0;
        Days = 0;
        Hours = 0;
    }

    /// <summary>
    /// Represents a time interval with years, months, days, and hours.
    /// </summary>
    /// <param name="years">The number of years in the time interval.</param>
    /// <param name="months">The number of months in the time interval.</param>
    /// <param name="days">The number of days in the time interval.</param>
    /// <param name="hours">The number of hours in the time interval.</param>
    /// <remarks>
    /// The <see cref="DeltaTime"/> class represents a time interval with years, months, days, and hours.
    /// When initializing a new instance of the <see cref="DeltaTime"/> class, only the lowest value between hours and days is stored.
    /// For example, if both hours and days are set, only the hours value will be stored.
    /// </remarks>
    public DeltaTime(int years = 0, int months = 0, int days = 0, int hours = 0)
    {
        ValidateTimeValues(years, months, days, hours);
        if (hours > 0)
        {
            Hours = hours;
        }
        else if (days > 0)
        {
            Days = days;
        }
        else if (months > 0)
        {
            Months = months;
        }
        else if (years > 0)
        {
            Years = years;
        }
    }
    private void ValidateTimeValues(int years, int months, int days, int hours)
    {
        if (years < 0 || months < 0 || days < 0 || hours < 0)
        {
            throw new ArgumentException("Time values cannot be negative.");
        }
        else if (months > 11)
        {
            throw new ArgumentException("Months cannot be greater than 11.");
        }
        else if (days > 30)
        {
            throw new ArgumentException("Days cannot be greater than 30.");
        }
        else if (hours > 23)
        {
            throw new ArgumentException("Hours cannot be greater than 23.");
        }
    }
    /// <summary>
    /// Gets the time values as an array.
    /// </summary>
    /// <returns>An array containing the hours, days, months, and years.</returns>
    public int[] GetTimeValues()
    {
        return [Hours, Days, Months, Years];
    }
}