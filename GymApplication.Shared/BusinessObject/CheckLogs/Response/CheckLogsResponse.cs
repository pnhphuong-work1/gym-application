namespace GymApplication.Shared.BusinessObject.CheckLogs.Response;

public sealed class CheckLogsResponse
{
    public Guid Id { get; set; }
    public Guid? CheckInId { get; set; }  
    public Guid UserSubscriptionId { get; set; } 
    public string CheckStatus { get; set; } = string.Empty;
    public TimeOnly? WorkoutTime { get; set; }
}