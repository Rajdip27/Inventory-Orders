namespace InventoryOrders.Application.CommonModel;

public static class AppRoles
{
    public const string Administrator = "Administrator";
    public const string Manager = "Manager";
    public const string Student = "Student";
    public const string AdminOrManager = Administrator + "," + Manager;
}
