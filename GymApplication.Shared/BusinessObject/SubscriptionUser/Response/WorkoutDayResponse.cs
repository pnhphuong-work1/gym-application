namespace GymApplication.Shared.BusinessObject.SubscriptionUser.Response;

public class WorkoutDayResponse
{
    public string Title {get; set;} = string.Empty;
    public DateOnly WorkoutDay { get; set; }
}