using AutoMapper;
using GymApplication.Repository.Abstractions;
using GymApplication.Repository.Entities;
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
    private readonly IMapper _mapper;

    public CreateUserRequestHandler(UserManager<ApplicationUser> userManager, IMapper mapper, RoleManager<ApplicationRole> roleManager)
    {
        _userManager = userManager;
        _mapper = mapper;
        _roleManager = roleManager;
    }

    public async Task<Result<UserResponse>> Handle(CreateUserRequest request, CancellationToken cancellationToken)
    {
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
        
        return Result.Success(response);
    }
}