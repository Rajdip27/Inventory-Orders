namespace InventoryOrders.Application.Services;

public class PermissionResult
{
    public bool Create { get; set; }
    public bool Edit { get; set; }
    public bool Delete { get; set; }
    public bool View { get; set; }
}

public interface IRolePermissionService
{
    bool CanCreate(List<string> roles);
    bool CanEdit(List<string> roles);
    bool CanDelete(List<string> roles);
    bool CanView(List<string> roles);

    PermissionResult CheckPermission(List<string> roles);
    Dictionary<string, PermissionResult> GetRoleWisePermission(List<string> roles);
}

public class RolePermissionService : IRolePermissionService
{
    // Role → permission mapping
    private readonly Dictionary<string, (bool Create, bool Edit, bool Delete, bool View)> _rules =
        new()
        {
            { "Administrator", (true, true, true, true) },
            { "Manager",       (true, true, false, true) },
            { "Student",       (true, false, false, true) }
        };

    // Core check (most permissive)
    private bool HasPermission(List<string> roles, Func<(bool Create, bool Edit, bool Delete, bool View), bool> selector)
    {
        if (roles == null || roles.Count == 0) return false;

        foreach (var role in roles)
        {
            if (_rules.TryGetValue(role, out var perm))
            {
                if (selector(perm)) return true;
            }
        }

        return false;
    }

    public bool CanCreate(List<string> roles) => HasPermission(roles, p => p.Create);
    public bool CanEdit(List<string> roles) => HasPermission(roles, p => p.Edit);
    public bool CanDelete(List<string> roles) => HasPermission(roles, p => p.Delete);
    public bool CanView(List<string> roles) => HasPermission(roles, p => p.View);

    // ✅ Combined result for multiple roles
    public PermissionResult CheckPermission(List<string> roles)
    {
        return new PermissionResult
        {
            Create = CanCreate(roles),
            Edit = CanEdit(roles),
            Delete = CanDelete(roles),
            View = CanView(roles)
        };
    }

    // ✅ Role-wise breakdown
    public Dictionary<string, PermissionResult> GetRoleWisePermission(List<string> roles)
    {
        var result = new Dictionary<string, PermissionResult>();

        foreach (var role in roles)
        {
            if (_rules.TryGetValue(role, out var perm))
            {
                result[role] = new PermissionResult
                {
                    Create = perm.Create,
                    Edit = perm.Edit,
                    Delete = perm.Delete,
                    View = perm.View
                };
            }
        }

        return result;
    }
}
