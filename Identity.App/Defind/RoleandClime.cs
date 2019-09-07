using System;

namespace Identity.App.Defind
{
    public static class ConstantRoles
    {
        public const string Admin = nameof(Admin);
    }

    public static class ConstantPolicies
    {
        public const string DynamicPermission = nameof(DynamicPermission);
        public const string DynamicPermissionClaimType = nameof(DynamicPermission);
    }
}
