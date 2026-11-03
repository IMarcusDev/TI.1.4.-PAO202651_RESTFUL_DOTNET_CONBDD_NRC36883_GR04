
using CONSOLE_CON_UNI_DOTNET_RESTful.Models.Enums;
using CONSOLE_CON_UNI_DOTNET_RESTFul.Controllers;
using CONSOLE_CON_UNI_DOTNET_RESTFul.Models.DTO;
using CONSOLE_CON_UNI_DOTNET_RESTFul.Views.Utils;

namespace CONSOLE_CON_UNI_DOTNET_RESTFul.Views
{
  internal class MovimientoView
  {
    private static MovimientoController controller = new MovimientoController();

    public static async Task Show()
    {
      int opcion = 0;

      do
      {
        MessagesUtils.PrintTitle("MENU MOVIMIENTOS");

        Console.WriteLine("1. Deposito");
        Console.WriteLine("2. Retiro");
        Console.WriteLine("3. Transferencia");
        Console.WriteLine("4. Regresar");

        Console.Write("\nSeleccione una opcion: ");

        if (!int.TryParse(Console.ReadLine(), out opcion))
        {
          MessagesUtils.PrintMessage("Opcion invalida");
          continue;
        }

        switch (opcion)
        {
          case 1:
            await Deposito();
            break;

          case 2:
            await Retiro();
            break;

          case 3:
            await Transferencia();
            break;

          case 4:
            MessagesUtils.PrintMessage("Regresando...");
            break;

          default:
            MessagesUtils.PrintMessage("Opcion invalida");
            break;
        }

        Console.WriteLine();

      } while (opcion != 4);
    }

    private static async Task Deposito()
    {
      MessagesUtils.PrintTitle("DEPOSITO");

      Console.Write("Cuenta origen (empleado): ");
      string cuentaOrigen = Console.ReadLine() ?? "";

      Console.Write("Cuenta destino: ");
      string cuentaDestino = Console.ReadLine() ?? "";

      Console.Write("Valor: ");
      if (!double.TryParse(Console.ReadLine(), out double valor))
      {
        MessagesUtils.PrintMessage("Valor invalido");
        return;
      }

      MovimientoResponseDTO response = await controller.DepositoAsync(
        cuentaOrigen,
        cuentaDestino,
        valor
      );

      PrintResponse(response);
    }

    private static async Task Retiro()
    {
      MessagesUtils.PrintTitle("RETIRO");

      Console.Write("Cuenta origen: ");
      string cuentaOrigen = Console.ReadLine() ?? "";

      Console.Write("Cuenta destino: ");
      string cuentaDestino = Console.ReadLine() ?? "";

      Console.Write("Valor: ");
      if (!double.TryParse(Console.ReadLine(), out double valor))
      {
        MessagesUtils.PrintMessage("Valor invalido");
        return;
      }

      MovimientoResponseDTO response = await controller.RetiroAsync(
        cuentaOrigen,
        cuentaDestino,
        valor
      );

      PrintResponse(response);
    }

    private static async Task Transferencia()
    {
      MessagesUtils.PrintTitle("TRANSFERENCIA");

      Console.Write("Cuenta origen: ");
      string cuentaOrigen = Console.ReadLine() ?? "";

      Console.Write("Cuenta destino: ");
      string cuentaDestino = Console.ReadLine() ?? "";

      Console.Write("Valor: ");
      if (!double.TryParse(Console.ReadLine(), out double valor))
      {
        MessagesUtils.PrintMessage("Valor invalido");
        return;
      }

      MovimientoResponseDTO response = await controller.TransferenciaAsync(
        cuentaOrigen,
        cuentaDestino,
        valor
      );

      PrintResponse(response);
    }

    private static void PrintResponse(MovimientoResponseDTO response)
    {
      MessagesUtils.PrintTitle("RESULTADO");

      MessagesUtils.PrintMessage(
        "Estado: " + response.GetEstado()
      );

      MessagesUtils.PrintMessage(
        "Mensaje: " + response.GetMensaje()
      );

      if (response.GetEstado() == EstadosEnum.OK)
      {
        MessagesUtils.PrintMessage(
          "ID Movimiento: " + response.IdMovimiento
        );
      }
    }
  }
}
