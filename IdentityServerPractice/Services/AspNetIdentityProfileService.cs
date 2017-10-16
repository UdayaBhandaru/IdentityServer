using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer.Data.Models;

namespace IdentityServer.Services
{
    public class AspNetIdentityProfileService : IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AspNetIdentityProfileService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        async public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subject = context.Subject;
            if (subject == null) throw new ArgumentNullException(nameof(context.Subject));

            var subjectId = subject.GetSubjectId();

            var user = await _userManager.FindByIdAsync(subjectId);
            if (user == null)
                throw new ArgumentException("Invalid subject identifier");

            var claims = await GetClaimsFromUser(user);
           
            context.IssuedClaims = claims.ToList();
        }

        async public Task IsActiveAsync(IsActiveContext context)
        {
            var subject = context.Subject;
            if (subject == null) throw new ArgumentNullException(nameof(context.Subject));

            var subjectId = subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(subjectId);

            context.IsActive = false;

            if (user != null)
            {
                if (_userManager.SupportsUserSecurityStamp)
                {
                    var security_stamp = subject.Claims.Where(c => c.Type == "security_stamp").Select(c => c.Value).SingleOrDefault();
                    if (security_stamp != null)
                    {
                        var db_security_stamp = await _userManager.GetSecurityStampAsync(user);
                        if (db_security_stamp != security_stamp)
                            return;
                    }
                }

                context.IsActive =
                    !user.LockoutEnabled ||
                    !user.LockoutEnd.HasValue ||
                    user.LockoutEnd <= DateTime.Now;
            }
        }

        async private Task<IEnumerable<Claim>> GetClaimsFromUser(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Subject, user.Id),
                new Claim(JwtClaimTypes.PreferredUserName, user.UserName)
            };

            if (_userManager.SupportsUserEmail)
            {
                claims.AddRange(new[]
                {
                    new Claim(JwtClaimTypes.Email, user.Email),
                    new Claim(JwtClaimTypes.EmailVerified, user.EmailConfirmed ? "true" : "false", ClaimValueTypes.Boolean)
                });
            }

            if (_userManager.SupportsUserPhoneNumber && !string.IsNullOrWhiteSpace(user.PhoneNumber))
            {
                claims.AddRange(new[]
                {
                    new Claim(JwtClaimTypes.PhoneNumber, user.PhoneNumber),
                    new Claim(JwtClaimTypes.PhoneNumberVerified, user.PhoneNumberConfirmed ? "true" : "false", ClaimValueTypes.Boolean)
                });
            }

            if (_userManager.SupportsUserClaim)
            {
                claims.AddRange(await _userManager.GetClaimsAsync(user));
            }

            if (_userManager.SupportsUserRole)
            {
                var roles = await _userManager.GetRolesAsync(user);
                claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
                claims.AddRange(roles.Select(role => new Claim(JwtClaimTypes.Role, role)));
            }

            return claims;
        }
    }
}
