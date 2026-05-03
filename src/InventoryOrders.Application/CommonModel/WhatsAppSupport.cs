namespace InventoryOrders.Application.CommonModel;

public class WhatsAppSupport
{
    public string Name { get; set; } = "";
    public string Phone { get; set; } = "";
}
public class WhatsAppSettings
{
    public WhatsAppSupport Support { get; set; } = new();
    public string DefaultMessage { get; set; } = "";
}
