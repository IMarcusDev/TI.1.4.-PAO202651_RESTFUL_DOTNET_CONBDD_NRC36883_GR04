using System;
using System.Collections.Generic;
using System.Text;

namespace CONSOLE_CON_UNI_DOTNET_RESTFul.Models.Data
{
  public class Endpoints
  {
    public static string Login { get; set; } = "/Login";
    public static string Movimiento { get; set; } = "/Movimiento";

    public static string Deposito { get; set; } = $"{Endpoints.Movimiento}/Deposito";
    public static string Retiro { get; set; } = $"{Endpoints.Movimiento}/Retiro";
    public static string Transferencia { get; set; } = $"{Endpoints.Movimiento}/Transferencia";
  }
}
