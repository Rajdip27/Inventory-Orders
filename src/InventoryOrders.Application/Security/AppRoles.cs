namespace InventoryOrders.Application.Security;

public static class AppRoles
{
    public const string Admin = "Admin";
    public const string Editor = "Editor";
    public const string Viewer = "Viewer";
    public const string Manage= Admin + "," + Editor; 
    public const string Delete= Admin;            
}
