using GymApplication.Repository.Entities;
using GymApplication.Repository.Repository.Abstraction;

namespace GymApplication.Repository.Repository;

public class PaymentLogRepository : RepoBase<PaymentLog, Guid>, IPaymentLogRepository
{
    public PaymentLogRepository(ApplicationDbContext context) : base(context)
    {
    }
}