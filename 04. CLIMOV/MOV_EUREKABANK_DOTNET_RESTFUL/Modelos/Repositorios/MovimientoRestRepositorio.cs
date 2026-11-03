using MOV_EUREKABANK_DOTNET_RESTFUL.Config;
using MOV_EUREKABANK_DOTNET_RESTFUL.Modelos.Data;
using MOV_EUREKABANK_DOTNET_RESTFUL.Modelos.Enums;
using MOV_EUREKABANK_DOTNET_RESTFUL.Modelos.Interfaces;
using MOV_EUREKABANK_DOTNET_RESTFUL.Modelos.Rest;

namespace MOV_EUREKABANK_DOTNET_RESTFUL.Modelos.Repositorios;

/// <summary>Recurso Movimiento: consultar (ECUD2) y deposito/retiro/transferencia (ECUD3).</summary>
public class MovimientoRestRepositorio(string urlBase, RestTransporte transporte, IVerificadorRed red)
    : RepositorioRest(urlBase, RestConfig.RecursoMovimiento, transporte, red), IMovimientoRepositorio
{
    public const string NoSePudoRegistrar = "No se pudo registrar la operación, intente nuevamente";

    public async Task<Resultado<ConsultaMovimientos>> ConsultarMovimientosAsync(string token, string cuenta)
    {
        if (SinRed())
            return Resultado<ConsultaMovimientos>.Error(RestErrores.SinConexion);

        try
        {
            RestLectorRespuesta respuesta = await EnviarAsync(Peticion("consultarMovimientos")
                .Parametro("token", token)
                .Parametro("cuenta", cuenta));
            if (respuesta.Estado() is null)
                return Resultado<ConsultaMovimientos>.Error(RestErrores.RespuestaIncompleta);
            if (!respuesta.EsOk())
                return Fallo<ConsultaMovimientos>(respuesta, "La cuenta no existe");

            string? numero = respuesta.Texto("numeroCuenta");
            var datosCuenta = new Cuenta(
                string.IsNullOrEmpty(numero) ? cuenta : numero,
                TextoOVacio(respuesta.Texto("titular")),
                TextoOVacio(respuesta.Texto("moneda")),
                Decimal(respuesta.Texto("saldo")),
                TextoOVacio(respuesta.Texto("estadoCuenta")));

            var movimientos = respuesta.Registros("numeroMovimiento")
                .Select(registro => new Movimiento(
                    Valor(registro, "numeroMovimiento"),
                    Valor(registro, "fecha"),
                    Valor(registro, "tipoMovimiento"),
                    Valor(registro, "accion"),
                    Decimal(Valor(registro, "importe")),
                    Valor(registro, "cuentaReferencia")))
                .ToList();

            return Resultado<ConsultaMovimientos>.Ok(new ConsultaMovimientos(datosCuenta, movimientos),
                TextoOVacio(respuesta.Texto("mensaje")));
        }
        catch (Exception e)
        {
            return Resultado<ConsultaMovimientos>.Error(RestErrores.Mensaje(e));
        }
    }

    public async Task<Resultado<Comprobante>> RegistrarAsync(string token, SolicitudOperacion solicitud)
    {
        if (SinRed())
            return Resultado<Comprobante>.Error(RestErrores.SinConexion);

        try
        {
            RestLectorRespuesta respuesta = await EnviarAsync(PeticionDe(token, solicitud));
            if (respuesta.Estado() is null)
                return Resultado<Comprobante>.Error(RestErrores.RespuestaIncompleta);
            if (!respuesta.EsOk())
                return Fallo<Comprobante>(respuesta, NoSePudoRegistrar);

            var comprobante = new Comprobante(solicitud,
                TextoOVacio(respuesta.Texto("numeroMovimiento")),
                Decimal(respuesta.Texto("nuevoSaldo")),
                DateTime.Now);
            return Resultado<Comprobante>.Ok(comprobante, TextoOVacio(respuesta.Texto("mensaje")));
        }
        catch (Exception e)
        {
            // ECUD3 FA10: no se reintenta automáticamente para no duplicar movimientos.
            return Resultado<Comprobante>.Error(RestErrores.Mensaje(e));
        }
    }

    private static RestConstructorPeticion PeticionDe(string token, SolicitudOperacion solicitud)
    {
        RestConstructorPeticion peticion = Peticion(solicitud.Tipo.Ruta()).Parametro("token", token);

        return solicitud.Tipo switch
        {
            TipoOperacion.Deposito => peticion
                .Parametro("cuenta", solicitud.Cuenta)
                .Parametro("importe", solicitud.Importe),
            TipoOperacion.Retiro => peticion
                .Parametro("cuenta", solicitud.Cuenta)
                .Parametro("importe", solicitud.Importe)
                .Parametro("clave", solicitud.Clave),
            _ => peticion
                .Parametro("cuentaOrigen", solicitud.Cuenta)
                .Parametro("cuentaDestino", solicitud.CuentaDestino)
                .Parametro("importe", solicitud.Importe)
                .Parametro("clave", solicitud.Clave),
        };
    }

    private static string Valor(IReadOnlyDictionary<string, string> registro, string nombre)
        => registro.TryGetValue(nombre, out string? valor) ? valor : "";
}
