using CONSOLE_EUREKABANK_DOTNET_RESTful.Models.Enums;
//revisado y aprovado por Marcos Escobar
using CONSOLE_EUREKABANK_DOTNET_RESTFul.Models.HTTP;
using System;
using System.Collections.Generic;
using System.Text;

namespace CONSOLE_EUREKABANK_DOTNET_RESTful.Models.HTTP
{
  internal class RESTfulClient
  {
    //revisado y aprovado por Marcos Escobar
    private string baseUrl = ConfigHTTP.BaseUrl;
    private HttpClient http;

    //revisado y aprovado por Marcos Escobar
    // Deja de esperar a los 15 segundos (RNF-007).
    public RESTfulClient()
    {
      this.http = ConfigHTTP.Handler is null
        ? new HttpClient()
        : new HttpClient(ConfigHTTP.Handler, false);
      this.http.Timeout = ConfigHTTP.Timeout;
    }

    public async Task<string> GetAsync(string endpoint)
    {
      using HttpRequestMessage request = new HttpRequestMessage(
          HttpMethod.Get,
          baseUrl + endpoint
      );

      request.Headers.Add("Accept", "application/json");

      using HttpResponseMessage response = await http.SendAsync(request);

      return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> GetAsync(string endpoint, string token)
    {
      using HttpRequestMessage request = new HttpRequestMessage(
          HttpMethod.Get,
          baseUrl + endpoint
      );

      request.Headers.Add("Accept", "application/json");
      request.Headers.Add("Authorization", token);

      using HttpResponseMessage response = await http.SendAsync(request);

      return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> PostAsync(
        string endpoint,
        string json,
        string token)
    {
      using HttpRequestMessage request = new HttpRequestMessage(
          HttpMethod.Post,
          baseUrl + endpoint
      );

      request.Headers.Add("Authorization", token);
      request.Headers.Add("Accept", "application/json");

      request.Content = new StringContent(
          json,
          Encoding.UTF8,
          "application/json"
      );

      using HttpResponseMessage response = await http.SendAsync(request);

      return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> PostAsync(string endpoint, string json)
    {
      using HttpRequestMessage request = new HttpRequestMessage(
          HttpMethod.Post,
          baseUrl + endpoint
      );

      request.Headers.Add("Accept", "application/json");

      request.Content = new StringContent(
          json,
          Encoding.UTF8,
          "application/json"
      );

      using HttpResponseMessage response = await http.SendAsync(request);

      return await response.Content.ReadAsStringAsync();
    }
  }

}
