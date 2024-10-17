using GymApplication.Repository.Entities;
using GymApplication.Repository.Repository.Abstraction;

namespace GymApplication.Repository.Repository;

public class UserSubscriptionRepository : RepoBase<UserSubscription, Guid>, IUserSubscriptionRepository
{
    public UserSubscriptionRepository(ApplicationDbContext context) : base(context)
    {
    }
}