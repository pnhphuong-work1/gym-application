namespace GymApplication.Shared.BusinessObject.SubscriptionUser.Response;

public sealed class SubscriptionUserResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid SubscriptionId { get; set; }
    public Guid PaymentId { get; set; }
    public decimal PaymentPrice { get; set; }
    public int WorkoutSteak { get; set; }
    public int LongestWorkoutSteak { get; set; }
    public DateTime LastWorkoutDate { get; set; }
    public DateTime SubscriptionEndDate { get; set; }
}