namespace TFApp.Models;

public class User
{
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    //[演習による変更] Migration
    public string Message { get; set; } = string.Empty;
}
