using Microsoft.AspNetCore.Authentication;

using System.Security.Claims;

using Thomsen.BrozWebsite.Repository;

namespace Thomsen.BrozWebsite;
public class RoleClaimsTransformer : IClaimsTransformation {
    private static readonly SemaphoreSlim _semaphore = new(1);

    private readonly IUserRoleRepository _userRoleRepository;

    public RoleClaimsTransformer(IUserRoleRepository userRoleRepository) {
        _userRoleRepository = userRoleRepository;
    }

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal) {
        if (principal.Identity is null || !principal.Identity.IsAuthenticated) {
            return principal;
        }

        var email = principal.FindFirst(ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(email)) {
            return principal;
        }

        await _semaphore.WaitAsync();
        try {
            var user = await _userRoleRepository.GetUserAsync(email);

            if (user is null) {
                user = new UserRole {
                    Email = email,
                    Role = UserRoleEnum.None
                };

                await _userRoleRepository.InsertUserAsync(user);

                return principal;
            }

            var identity = (ClaimsIdentity)principal.Identity;

            identity.AddClaim(new Claim(ClaimTypes.Role, user.Role.ToString()));

            return principal;
        } finally {
            _semaphore.Release();
        }
    }
}