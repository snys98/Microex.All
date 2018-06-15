using Microex.All.Common;

namespace Microex.All.PredifinedEnumerations
{
    public class PredefinedRole:Enumeration<string>
    {
        public PredefinedRole(string value, string name) : base(value, name)
        {
        }

        public static PredefinedRole IdentityAdmin { get; set; } = new PredefinedRole(nameof(IdentityAdmin).ToLowerInvariant(), nameof(IdentityAdmin).ToLowerInvariant());
    }
}
