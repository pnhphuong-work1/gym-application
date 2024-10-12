namespace GymApplication.Shared.BusinessObject.Revenue.Response;

public sealed class RevenueResponse
{
    public int TotalMember { get; set; }
    public decimal TotalRevenue { get; set; }
    public List<WeeklyRevenueResponse> WeeklyRevenue { get; set; } = new List<WeeklyRevenueResponse>();
}

public sealed class WeeklyRevenueResponse
{
    public string Week { get; set; }
    public decimal Revenue { get; set; }
} 