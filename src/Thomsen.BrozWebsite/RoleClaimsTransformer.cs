using Microsoft.AspNetCore.Authentication;

using System.Security.Claims;

using Thomsen.BrozWebsite.Repository;

using static Dapper.SqlMapper;

namespace Thomsen.BrozWebsite;
public class RoleClaimsTransformer : IClaimsTransformation {
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
    }
}