

using Microsoft.AspNetCore.Components;
using System.Net;
using System.Security.Claims;

namespace CllientMudBlazor.Pages.Authentication
{
    public partial class LoginDisplay
    {
        private string FirstName { get; set; } = string.Empty;

   
        private string Email { get; set; } = string.Empty;
        private string LetterOfName { get; set; } = string.Empty;
        [Parameter]
        public ClaimsPrincipal User { get; set; } = null!;
        protected override void OnParametersSet()
        {
            var user = User;
            if (user == null) return;
            if (user.Identity?.IsAuthenticated == true)
            {

                FirstName = user.FindFirst(ClaimTypes.Name)!.Value;

                if (FirstName.Length > 0)
                {
                    LetterOfName = $"{FirstName[0]}";
                }
                Email = user.FindFirst(ClaimTypes.Email)!.Value;




                StateHasChanged();
            }
        }
    }
}
