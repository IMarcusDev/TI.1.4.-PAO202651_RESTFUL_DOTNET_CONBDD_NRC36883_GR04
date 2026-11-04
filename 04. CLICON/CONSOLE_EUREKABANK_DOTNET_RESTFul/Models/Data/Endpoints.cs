using System;
using System.Collections.Generic;
using System.Text;

namespace CONSOLE_EUREKABANK_DOTNET_RESTFul.Models.Data
{
  public class Endpoints
  {
    public static string Login { get; set; } = "/Login";
    public static string Movimiento { get; set; } = "/Movimiento";

    //revisado y aprovado por Marcos Escobar
    // Operaciones del ERS (RNF-001), con los mismos nombres que en los servidores SOAP.
    public static string LoginEmpleado { get; set; } = $"{Endpoints.Login}/loginEmpleado";
    public static string LoginCliente { get; set; } = $"{Endpoints.Login}/loginCliente";
    public static string Logout { get; set; } = $"{Endpoints.Login}/logout";
    public static string Consultar { get; set; } = $"{Endpoints.Movimiento}/consultarMovimientos";

    //revisado y aprovado por Marcos Escobar
    public static string Deposito { get; set; } = $"{Endpoints.Movimiento}/registrarDeposito";
    public static string Retiro { get; set; } = $"{Endpoints.Movimiento}/registrarRetiro";
    public static string Transferencia { get; set; } = $"{Endpoints.Movimiento}/registrarTransferencia";
  }
}
