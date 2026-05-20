namespace Pawtrix.ViewModels;

public class LoginWindowViewModel : ViewModelBase
{
    public string GreetingTop { get; } = "Welcome to Pawtrix!";
    public string Greeting { get; } = "Put in your homeserver, \nlogin and password below.";
    public string Homeserver { get; set; } = "";
    public string Login { get; set; } = "";
    public string Password { get; set; } = "";
}