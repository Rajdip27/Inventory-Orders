namespace InventoryOrders.Application.CommonModel;

public class EmailMessage
{
    public List<string> To { get; set; } = new List<string>();
    public List<string> CC { get; set; } = new() { "srajdip920@gmail.com" };
    public List<string> BCC { get; set; } = new() { "srajdip920@gmail.com" };
    public string Subject { get; set; } = "";
    public string HtmlFilePath { get; set; } // optional HTML file
    public string Message { get; set; }      // optional plain message
    public List<EmailAttachment> Attachments { get; set; } // optional files
}

public class EmailAttachment
{
    public string FileName { get; set; }
    public byte[] Content { get; set; }
    public string ContentType { get; set; }
}