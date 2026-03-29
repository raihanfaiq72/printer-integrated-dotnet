using System;
using System.Threading.Tasks;
using marker_dotnet.Models;

namespace marker_dotnet.Interfaces
{
    public interface IPrinterService
    {
        bool CetakMarker(string invoice, int totalMarkers);
        (string markerCode, string markerCount) GenerateMarkerCode(string invoice, int index, int total);
        bool CheckPrinterAvailable();
    }

    public interface IApiService
    {
        Task<ApiResponse> KirimPermintaanCetakAsync(int jumlahMarker, string device = "desktop");
        Task UpdateStatusAsync(string invoice, string status, string errorMessage = null);
    }

    public interface IDebugService
    {
        bool IsDebugMode { get; }
        void EnableDebugMode();
        void DisableDebugMode();
        bool CheckSecretCode(string input);
    }
}
