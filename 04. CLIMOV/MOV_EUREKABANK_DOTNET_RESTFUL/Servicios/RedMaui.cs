using MOV_EUREKABANK_DOTNET_RESTFUL.Modelos.Interfaces;

namespace MOV_EUREKABANK_DOTNET_RESTFUL.Servicios;

public class RedMaui : IVerificadorRed
{
    public bool SinRed()
    {
        try
        {
            return Connectivity.Current.NetworkAccess == NetworkAccess.None;
        }
        catch
        {
            return false;
        }
    }
}
