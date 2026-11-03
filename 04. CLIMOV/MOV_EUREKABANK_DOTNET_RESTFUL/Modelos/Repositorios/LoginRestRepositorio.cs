using MOV_EUREKABANK_DOTNET_RESTFUL.Config;
using MOV_EUREKABANK_DOTNET_RESTFUL.Modelos.Data;
using MOV_EUREKABANK_DOTNET_RESTFUL.Modelos.Enums;
using MOV_EUREKABANK_DOTNET_RESTFUL.Modelos.Interfaces;
using MOV_EUREKABANK_DOTNET_RESTFUL.Modelos.Rest;

namespace MOV_EUREKABANK_DOTNET_RESTFUL.Modelos.Repositorios;

/// <summary>Recurso Login: empleado, cliente y logout (ECUD1).</summary>
public class LoginRestRepositorio(string urlBase, RestTransporte transporte, IVerificadorRed red)
    : RepositorioRest(urlBase, RestConfig.RecursoLogin, transporte, red), ILoginRepositorio
{
    public const string CredencialesIncorrectas = "Credenciales incorrectas";

    public Task<Resultado<Sesion>> LoginEmpleadoAsync(string usuario, string clave)
        => LoginAsync(Peticion("loginEmpleado")
            .Parametro("usuario", usuario)
            .Parametro("clave", clave), Perfil.Empleado, null);

    public Task<Resultado<Sesion>> LoginClienteAsync(string cuenta, string clave)
        => LoginAsync(Peticion("loginCliente")
            .Parametro("cuenta", cuenta)
            .Parametro("clave", clave), Perfil.Cliente, cuenta);

    private async Task<Resultado<Sesion>> LoginAsync(RestConstructorPeticion peticion, Perfil perfilPedido, string? cuenta)
    {
        if (SinRed())
            return Resultado<Sesion>.Error(RestErrores.SinConexion);

        try
        {
            RestLectorRespuesta respuesta = await EnviarAsync(peticion);
            if (respuesta.Estado() is null)
                return Resultado<Sesion>.Error(RestErrores.RespuestaIncompleta);
            if (!respuesta.EsOk())
                return Resultado<Sesion>.Error(RestErrores.MensajeServidor(respuesta.Texto("mensaje"), CredencialesIncorrectas));

            string? token = respuesta.Texto("token");
            if (string.IsNullOrEmpty(token))
                return Resultado<Sesion>.Error(RestErrores.RespuestaIncompleta);

            Perfil perfil = PerfilExtensions.Desde(respuesta.Texto("perfil"), perfilPedido);
            var sesion = new Sesion(token, perfil, TextoOVacio(respuesta.Texto("nombre")), cuenta);
            return Resultado<Sesion>.Ok(sesion, TextoOVacio(respuesta.Texto("mensaje")));
        }
        catch (Exception e)
        {
            return Resultado<Sesion>.Error(RestErrores.Mensaje(e));
        }
    }

    public async Task<Resultado<bool>> LogoutAsync(string token)
    {
        if (SinRed())
            return Resultado<bool>.Error(RestErrores.SinConexion);

        try
        {
            RestLectorRespuesta respuesta = await EnviarAsync(Peticion("logout").Parametro("token", token));
            return respuesta.EsOk()
                ? Resultado<bool>.Ok(true, TextoOVacio(respuesta.Texto("mensaje")))
                : Resultado<bool>.Error(RestErrores.MensajeServidor(respuesta.Texto("mensaje"), RestErrores.RespuestaIncompleta));
        }
        catch (Exception e)
        {
            return Resultado<bool>.Error(RestErrores.Mensaje(e));
        }
    }
}
