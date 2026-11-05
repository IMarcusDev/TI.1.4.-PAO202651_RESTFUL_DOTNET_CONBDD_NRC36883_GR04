using System.Net;
using ec.edu.monster.fakes;

namespace ec.edu.monster.modelos.rest;

public class RestConstructorPeticionTest
{
    [Fact]
    public void BuildsAFlatJsonObjectWithTheAmountAsANumberWithTwoDecimals()
    {
        string json = new RestConstructorPeticion("registrarRetiro")
            .Parametro("cuenta", "001\"<&>")
            .Parametro("importe", 120.5m)
            .Parametro("omitido", (string?)null)
            .Construir();

        Assert.Equal("{\"cuenta\":\"001\\u0022\\u003C\\u0026\\u003E\",\"importe\":120.50}", json);
    }
}

public class RestLectorRespuestaTest
{
    private static RestLectorRespuesta Lector(string json, int codigo = 200) => new(new RespuestaHttp(codigo, json), "http://h:1/Login/x");

    [Fact]
    public void ReadsCamelCaseFieldsIgnoringCaseAndANumericOkEstado()
    {
        RestLectorRespuesta lector = Lector(RestRespuestas.LoginOk("Monster", "Empleado"));

        Assert.True(lector.EsOk());
        Assert.Equal("token-123", lector.Texto("Token"));
        Assert.Equal("Empleado", lector.Texto("tipoUsuario"));
        Assert.Null(lector.Texto("noExiste"));
    }

    [Theory]
    [InlineData("{\"estado\":1,\"mensaje\":\"x\"}", "ERROR")]
    [InlineData("{\"Estado\":\"ok\",\"Mensaje\":\"x\"}", "OK")]
    public void AcceptsEstadoAsNumberOrText(string json, string esperado)
        => Assert.Equal(esperado, Lector(json).Estado());

    [Fact]
    public void ReadsNestedAccountFieldsAndTheMovementList()
    {
        RestLectorRespuesta lector = Lector(RestRespuestas.ConsultaOk(
            (1, "2026-10-30T09:15:00", "Depósito", "INGRESO", "500.00", ""),
            (2, "2026-10-31T10:00:00", "Retiro", "SALIDA", "120.50", "")));

        IReadOnlyList<IReadOnlyDictionary<string, string>> registros = lector.Registros("numeroMovimiento");

        Assert.Equal("379.50", lector.Texto("saldo"));
        Assert.Equal(2, registros.Count);
        Assert.Equal("120.50", registros[1]["importe"]);
        Assert.Equal("", registros[1]["cuentaReferencia"]);
    }

    [Fact]
    public void AnHttpErrorWithEstadoAndMensajeIsReadAsAResponse()
        => Assert.Equal("Token inválido", Lector(RestRespuestas.Error("Token inválido"), 401).Texto("mensaje"));

    [Theory]
    [InlineData("")]
    [InlineData("<html>Not Found</html>")]
    [InlineData("{\"title\":\"Not Found\"}")]
    public void AnHttpErrorWithoutEstadoThrowsWithItsCode(string cuerpo)
        => Assert.Equal(404, Assert.Throws<HttpEstadoException>(() => Lector(cuerpo, 404)).Codigo);

    [Theory]
    [InlineData("")]
    [InlineData("no es json")]
    [InlineData("[1,2]")]
    public void ThrowsOnAnEmptyOrInvalidResponse(string json)
        => Assert.Throws<RespuestaInvalidaException>(() => Lector(json));
}

public class TransporteRestTest
{
    [Fact]
    public async Task PostsTheJsonBodyAsApplicationJson()
    {
        var manejador = new ManejadorFalso(HttpStatusCode.OK, "{\"estado\":0}");
        var transporte = new TransporteRest(new HttpClient(manejador));

        RespuestaHttp respuesta = await transporte.EnviarAsync("http://h:1/Login/logout", "{\"token\":\"t\"}");

        Assert.Equal(new RespuestaHttp(200, "{\"estado\":0}"), respuesta);
        Assert.Equal(HttpMethod.Post, manejador.Metodo);
        Assert.Equal("http://h:1/Login/logout", manejador.Url);
        Assert.Equal("application/json", manejador.TipoContenido);
        Assert.Equal("{\"token\":\"t\"}", manejador.Cuerpo);
    }

    [Fact]
    public async Task ReturnsErrorStatusesWithTheirBody()
    {
        var transporte = new TransporteRest(new HttpClient(new ManejadorFalso(HttpStatusCode.InternalServerError, "boom")));

        Assert.Equal(new RespuestaHttp(500, "boom"), await transporte.EnviarAsync("http://h:1/x", "{}"));
    }

    private sealed class ManejadorFalso(HttpStatusCode estado, string cuerpo) : HttpMessageHandler
    {
        public HttpMethod? Metodo { get; private set; }
        public string? Url { get; private set; }
        public string? TipoContenido { get; private set; }
        public string? Cuerpo { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage peticion, CancellationToken cancelacion)
        {
            Metodo = peticion.Method;
            Url = peticion.RequestUri?.ToString();
            TipoContenido = peticion.Content?.Headers.ContentType?.MediaType;
            Cuerpo = peticion.Content is null ? null : await peticion.Content.ReadAsStringAsync(cancelacion);
            return new HttpResponseMessage(estado) { Content = new StringContent(cuerpo) };
        }
    }
}
