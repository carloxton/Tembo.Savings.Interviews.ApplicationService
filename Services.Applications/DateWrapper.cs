namespace Services.Applications;

// For a real app we would unit test this, but I don't think that is the point of this exercise
public class DateWrapper 
{ 
    private readonly DateTime? _date;
    
    public DateWrapper(DateTime date)
    {
        _date = date;
    }

    public DateWrapper() { }

    public DateTime Today => _date.HasValue ? _date.Value.Date : DateTime.Today;

    public int CalculateAge(DateOnly? birthdate)
    {
        // 'Borrowed from StackOverflow' - for a real app we would wrap in a class and unit test it
        ArgumentNullException.ThrowIfNull(birthdate, nameof(birthdate));

        DateTime currentDate = DateTime.Now;
        int age = currentDate.Year - birthdate.Value.Year;

        if (currentDate.Month < birthdate.Value.Month || (currentDate.Month == birthdate.Value.Month && currentDate.Day < birthdate.Value.Day))
        {
            age--;
        }

        return age;
    }
}
