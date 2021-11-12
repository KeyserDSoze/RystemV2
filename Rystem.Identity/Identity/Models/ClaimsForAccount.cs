using System;

namespace Rystem.Identity
{
    internal class ClaimsForAccount
    {
        public string Id { get; set; }
        public string ClaimId { get; set; }
        public string ClaimValue { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
