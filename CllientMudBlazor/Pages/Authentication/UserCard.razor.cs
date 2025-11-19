using Microsoft.AspNetCore.Components;

namespace CllientMudBlazor.Pages.Authentication
{
    public partial class UserCard
    {
        [Parameter] public string Class { get; set; } = string.Empty;
        [Parameter]
        public string Name { get; set; } = string.Empty;
        
        [Parameter]
        public string Email { get; set; } = string.Empty;
    
        [Parameter]
        public string LetterOfName { get; set; } = string.Empty;
    }
}
