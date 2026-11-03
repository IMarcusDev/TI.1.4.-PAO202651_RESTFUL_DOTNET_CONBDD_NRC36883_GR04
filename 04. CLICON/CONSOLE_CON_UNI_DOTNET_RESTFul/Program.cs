
using CONSOLE_CON_UNI_DOTNET_RESTFul.Views;
using CONSOLE_CON_UNI_DOTNET_RESTFul.Views.Utils;

// Splash Art
MessagesUtils.PrintMonster();
Thread.Sleep(TimeSpan.FromSeconds(3));

// Login
bool login = LoginView.UserHasLogin();

if (!login)
{
  return;
}

// Menu
// ConverterView.ShowMenu();

MessagesUtils.PrintMessage("Hasta Luego!");
Thread.Sleep(TimeSpan.FromSeconds(3));

