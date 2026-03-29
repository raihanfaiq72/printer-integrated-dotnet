using System;

namespace marker_dotnet.Services
{
    public class DebugService : marker_dotnet.Interfaces.IDebugService
    {
        private bool _debugMode = false;
        private string _secretCodeInput = "";

        public bool IsDebugMode => _debugMode;

        public bool CheckSecretCode(string input)
        {
            _secretCodeInput += input;
            
            if (_secretCodeInput.EndsWith("7272"))
            {
                EnableDebugMode();
                _secretCodeInput = "";
                return true;
            }
            else if (_secretCodeInput.EndsWith("2727"))
            {
                DisableDebugMode();
                _secretCodeInput = "";
                return true;
            }
            
            if (_secretCodeInput.Length > 8)
            {
                _secretCodeInput = _secretCodeInput.Substring(_secretCodeInput.Length - 8);
            }
            
            return false;
        }

        public void EnableDebugMode()
        {
            _debugMode = true;
        }

        public void DisableDebugMode()
        {
            _debugMode = false;
        }
    }
}
