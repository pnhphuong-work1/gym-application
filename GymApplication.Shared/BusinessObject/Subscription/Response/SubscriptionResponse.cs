
namespace GymApplication.Shared.BusinessObject.Subscription.Respone;

public sealed class SubscriptionResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public TimeOnly? TotalWorkoutTime { get; set; }
    public decimal Price { get; set; }
    public Guid DayGroupId { get; set; }
    public string Group { get; set; }
}
