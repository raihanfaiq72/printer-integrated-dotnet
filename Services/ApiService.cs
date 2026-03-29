using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using marker_dotnet.Models;
using marker_dotnet.Interfaces;

namespace marker_dotnet.Services
{
    public class ApiService : IApiService
    {
        private static readonly string API_URL = "https://api-marker.crystalclean.co.id/api/get-print-batch-data";
        private static readonly string API_URL_STATUS = "https://api-marker.crystalclean.co.id/api";
        private static readonly HttpClient httpClient = new HttpClient();

        static ApiService()
        {
            httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        public async Task<ApiResponse> KirimPermintaanCetakAsync(int jumlahMarker, string device = "desktop")
        {
            try
            {
                var request = new PrintRequest
                {
                    JumlahMarkerDiminta = jumlahMarker,
                    Device = device
                };

                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(API_URL, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"API Error: {response.StatusCode}");
                }

                var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(responseContent);

                if (!apiResponse.Success)
                {
                    throw new Exception(apiResponse.Message ?? "Gagal memproses permintaan cetak dari API");
                }

                return apiResponse;
            }
            catch (Exception ex)
            {
                throw new Exception($"Gagal menghubungi API: {ex.Message}");
            }
        }

        public async Task UpdateStatusAsync(string invoice, string status, string errorMessage = null)
        {
            try
            {
                var request = new StatusUpdateRequest
                {
                    Invoice = invoice,
                    Status = status,
                    ErrorMessage = errorMessage
                };

                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync($"{API_URL_STATUS}/update-status", content);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to update status: {ex.Message}");
            }
        }
    }
}
