using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TourverseToolkit.Editor
{
    internal sealed class AuthResponse
    {
        public string access_token;
    }

    internal static class UploadService
    {
        public static async Task<string> GetTokenAsync(string login, string password, string authUrl)
        {
            using var http = new HttpClient();
            return await AuthAsync(http, login, password, authUrl);
        }

        public static async Task UploadTourAsync(string token, string archivePath, string uuid, string uploadUrl)
        {
            using var http = new HttpClient();
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var fileStream = File.OpenRead(archivePath);
            var multipart = new MultipartFormDataContent();
            multipart.Add(new StreamContent(fileStream), "file", Path.GetFileName(archivePath));
            multipart.Add(new StringContent(uuid), "addressables_info_uuid");

            UnityEngine.Debug.Log($"[Tourverse] POST {uploadUrl} | file={Path.GetFileName(archivePath)} size={fileStream.Length} | uuid={uuid}");

            var response = await http.PostAsync(uploadUrl, multipart);
            if (!response.IsSuccessStatusCode)
            {
                string body = await response.Content.ReadAsStringAsync();
                throw new Exception($"{(int)response.StatusCode} {response.ReasonPhrase}: {body}");
            }
        }

        private static async Task<string> AuthAsync(HttpClient http, string login, string password, string authUrl)
        {
            string bodyJson = JsonConvert.SerializeObject(new { login, password, crm = true });
            var content  = new StringContent(bodyJson, Encoding.UTF8, "application/json");
            var response = await http.PostAsync(authUrl, content);
            if (!response.IsSuccessStatusCode)
            {
                string body = await response.Content.ReadAsStringAsync();
                throw new Exception($"{(int)response.StatusCode} {response.ReasonPhrase}: {body}");
            }

            string json = await response.Content.ReadAsStringAsync();
            var result  = JsonConvert.DeserializeObject<AuthResponse>(json);

            if (result == null || string.IsNullOrEmpty(result.access_token))
                throw new Exception($"[Tourverse] access_token not found in auth response: {json}");

            return result.access_token;
        }
    }
}
