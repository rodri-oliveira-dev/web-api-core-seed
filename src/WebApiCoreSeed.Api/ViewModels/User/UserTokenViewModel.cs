using System.Collections.Generic;

namespace WebApiCoreSeed.Api.ViewModels.User
{
    public class UserTokenViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public IEnumerable<ClaimViewModel> Claims { get; set; } = new List<ClaimViewModel>();
    }
}
