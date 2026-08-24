using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace WebApiCoreSeed.Api.DevelopmentSeed
{
    public sealed class DevelopmentSeedIdentitySeeder
    {
        private readonly UserManager<IdentityUser> _userManager;

        public DevelopmentSeedIdentitySeeder(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<int> SeedAsync(DevelopmentSeedUserOptions options, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var user = await _userManager.FindByIdAsync(options.Id)
                ?? await _userManager.FindByEmailAsync(options.Email);

            var changes = 0;
            if (user is null)
            {
                user = new IdentityUser
                {
                    Id = options.Id,
                    UserName = options.UserName,
                    Email = options.Email,
                    EmailConfirmed = true
                };

                await EnsureSucceededAsync(_userManager.CreateAsync(user, options.Password));
                changes++;
            }
            else
            {
                var changed = false;
                changed |= SetIfDifferent(user.UserName, options.UserName, value => user.UserName = value);
                changed |= SetIfDifferent(user.Email, options.Email, value => user.Email = value);

                if (!user.EmailConfirmed)
                {
                    user.EmailConfirmed = true;
                    changed = true;
                }

                if (changed)
                {
                    await EnsureSucceededAsync(_userManager.UpdateAsync(user));
                    changes++;
                }

                if (!await _userManager.CheckPasswordAsync(user, options.Password))
                {
                    if (await _userManager.HasPasswordAsync(user))
                    {
                        await EnsureSucceededAsync(_userManager.RemovePasswordAsync(user));
                    }

                    await EnsureSucceededAsync(_userManager.AddPasswordAsync(user, options.Password));
                    changes++;
                }
            }

            cancellationToken.ThrowIfCancellationRequested();

            var existingClaims = await _userManager.GetClaimsAsync(user);
            var missingClaims = DevelopmentSeedDefinition.UserClaims
                .Where(required => !existingClaims.Any(existing =>
                    string.Equals(existing.Type, required.Type, StringComparison.Ordinal)
                    && string.Equals(existing.Value, required.Value, StringComparison.Ordinal)))
                .ToArray();

            if (missingClaims.Length > 0)
            {
                await EnsureSucceededAsync(_userManager.AddClaimsAsync(user, missingClaims));
                changes += missingClaims.Length;
            }

            return changes;
        }

        private static bool SetIfDifferent(string? current, string expected, Action<string> setValue)
        {
            if (string.Equals(current, expected, StringComparison.Ordinal))
            {
                return false;
            }

            setValue(expected);
            return true;
        }

        private static async Task EnsureSucceededAsync(Task<IdentityResult> operation)
        {
            var result = await operation;
            if (result.Succeeded)
            {
                return;
            }

            var errors = string.Join("; ", result.Errors.Select(error => $"{error.Code}: {error.Description}"));
            throw new InvalidOperationException($"Identity development seed failed: {errors}");
        }
    }
}
