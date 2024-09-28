using AutoMapper;
using GymApplication.Repository.Abstractions;
using GymApplication.Repository.Entities;
using GymApplication.Services.Abstractions;
using GymApplication.Shared.BusinessObject.User.Request;
using GymApplication.Shared.BusinessObject.User.Response;
using GymApplication.Shared.Common;
using MediatR;
using Microsoft.AspNetCore.Identity;
using UUIDNext;

namespace GymApplication.Services.Feature.UserFeatures;

public sealed class CreateUserRequestHandler : IRequestHandler<CreateUserRequest, Result<UserResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ICacheServices _cacheServices;
    private readonly IMapper _mapper;

    public CreateUserRequestHandler(UserManager<ApplicationUser> userManager, IMapper mapper, RoleManager<ApplicationRole> roleManager, ICacheServices cacheServices)
    {
        _userManager = userManager;
        _mapper = mapper;
        _roleManager = roleManager;
        _cacheServices = cacheServices;
    }

    public async Task<Result<UserResponse>> Handle(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var handle = await GetExistUserByEmail(request);
        
        if (handle is not null)
        {
            return handle;
        }

        var user = new ApplicationUser
        {
            Id = Uuid.NewDatabaseFriendly(Database.PostgreSql),
            FullName = request.FullName,
            UserName = request.Email,
            DateOfBirth = request.DateOfBirth,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            CreatedAt = DateTime.UtcNow
        };
        
        var result = await _userManager.CreateAsync(user, request.Password!);
        
        if (!result.Succeeded)
        {
            Error error = new("400", "User creation failed"); 
            return Result.Failure<UserResponse>(error);
        }

        if (!await _roleManager.RoleExistsAsync(request.Role.ToString()))
        {
            var role = new ApplicationRole
            {
                Name = request.Role.ToString()
            };

            await _roleManager.CreateAsync(role);
        }

        var roleResult = await _userManager.AddToRoleAsync(user, request.Role.ToString());
        
        if (!roleResult.Succeeded)
        {
            Error error = new("400", "Role assignment failed"); 
            return Result.Failure<UserResponse>(error);
        }
        
        var response = _mapper.Map<UserResponse>(user);
        await _cacheServices.SetAsync(user.Id.ToString(), response, cancellationToken);
        return Result.Success(response);
    }

    private async Task<Result<UserResponse>?> GetExistUserByEmail(CreateUserRequest request)
    {
        if (request.Email == null) return null;
        var existingUser = await _userManager.FindByEmailAsync(request.Email);

        if (existingUser is null) return null;
        Error error = new("400", "User already exists");
        {
            Result.Failure<UserResponse>(error);
            return Result.Failure<UserResponse>(error);
        }
    }
}