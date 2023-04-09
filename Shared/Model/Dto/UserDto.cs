using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace Cargotruck.Shared.Model.Dto
{
    public class UserDto : IdentityUser
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public override string PasswordHash { get; set; } = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public override string SecurityStamp { get; set; } = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public override string ConcurrencyStamp { get; set; } = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public override bool PhoneNumberConfirmed { get; set; } = false;
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public override bool TwoFactorEnabled { get; set; } = false;
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public override DateTimeOffset? LockoutEnd { get; set; } = null;
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public override bool LockoutEnabled { get; set; } = false;
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public override int AccessFailedCount { get; set; } = 0;
    }
}
