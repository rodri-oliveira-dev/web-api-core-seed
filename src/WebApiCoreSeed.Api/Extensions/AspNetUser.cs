using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using WebApiCoreSeed.SampleRestaurant.Interfaces;

namespace WebApiCoreSeed.Api.Extensions
{
    public class AspNetUser : IUser
    {
        private readonly IHttpContextAccessor _accessor;

        public AspNetUser(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public string Name => _accessor.HttpContext?.User.Identity?.Name ?? string.Empty;

        public Guid GetUserId()
        {
            var userId = _accessor.HttpContext?.User.GetUserId();
            return IsAuthenticated() && Guid.TryParse(userId, out var parsedUserId) ? parsedUserId : Guid.Empty;
        }

        public string GetUserEmail()
        {
            return IsAuthenticated() ? _accessor.HttpContext?.User.GetUserEmail() ?? string.Empty : string.Empty;
        }

        public bool IsAuthenticated()
        {
            return _accessor.HttpContext?.User.Identity?.IsAuthenticated == true;
        }

        public bool IsInRole(string role)
        {
            return _accessor.HttpContext?.User.IsInRole(role) == true;
        }

        public IEnumerable<Claim> GetClaimsIdentity()
        {
            return _accessor.HttpContext?.User.Claims ?? Array.Empty<Claim>();
        }
    }
}
