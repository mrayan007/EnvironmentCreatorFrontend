using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Linq;

public class ApiClient : MonoBehaviour
{
    public static ApiClient instance { get; private set; }
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this);
    }

    private string baseUrl = "http://localhost:5021";
    public string accessToken { get; private set; }

    private async Task<string> ApiCall(string url, string method, string jsonData = null, string token = null)
    {
        using (UnityWebRequest request = new UnityWebRequest(url, method))
        {
            if (!string.IsNullOrEmpty(jsonData))
            {
                byte[] jsonToSend = Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            }

            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            if (!string.IsNullOrEmpty(token))
            {
                request.SetRequestHeader("Authorization", "Bearer " + token);
            }

            await request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("API-aanroep is successvol: " + request.downloadHandler.text);

                return request.downloadHandler.text;
            }
            else
            {
                Debug.Log("Fout bij API-aanroep: " + request.downloadHandler.text);
                return request.downloadHandler.text;
            }
        }
    }

    public async Task<string> Register(string username, string password)
    {
        var register = new RegisterDto(username, password);
        var request = JsonUtility.ToJson(register);

        Debug.Log(request);

        string url = $"{baseUrl}/account/register";

        return await ApiCall(url, "POST", request);
    }

    public async Task Login(string username, string password)
    {
        var login = new LoginDto(username, password);
        var request = JsonUtility.ToJson(login);

        Debug.Log(request);

        string url = $"{baseUrl}/account/login";

        var response = await ApiCall(url, "POST", request);

        LoginResponseDto responseDto = JsonUtility.FromJson<LoginResponseDto>(response);
        accessToken = responseDto.token;

        Debug.Log(accessToken);
    }

    public async Task<string> CreateEnvironment(string name, double maxHeight, double maxWidth)
    {
        var environment = new EnvironmentDto(name, maxHeight, maxWidth);
        var request = JsonUtility.ToJson(environment);

        Debug.Log(request);

        string url = $"{baseUrl}/environments/create";

        return await ApiCall(url, "POST", request, accessToken);
    }

    public async Task<List<string>> GetEnvironments()
    {
        string url = $"{baseUrl}/environments/all";

        Debug.Log(accessToken);

        var response = await ApiCall(url, "GET", null, accessToken);

        if (!string.IsNullOrEmpty(response))
        {
            var environments = JsonHelper.FromJson<EnvironmentNamesDto>(JsonHelper.FixJsonArray(response));
            return environments.Select(e => e.name).ToList();
        }

        return null;
    }

    public async Task<EnvironmentResponseDto> GetEnvironmentByName(string name)
    {
        string url = $"{baseUrl}/environments/{name}";
        var response = await ApiCall(url, "GET", null, accessToken);

        EnvironmentResponseDto environment = JsonUtility.FromJson<EnvironmentResponseDto>(response);
        return environment;
    }

    public async Task<string> DeleteEnvironment(string name)
    {
        string url = $"{baseUrl}/environments/{name}";
        return await ApiCall(url, "DELETE", null, accessToken);
    }

    public async Task AddObject(ObjectDto objectDto)
    {
        var request = JsonUtility.ToJson(objectDto);
        Debug.Log(objectDto);

        string url = $"{baseUrl}/environments/object";

        await ApiCall(url, "POST", request, accessToken);
    }
}
