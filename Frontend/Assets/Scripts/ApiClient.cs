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
                Debug.Log("Fout bij API-aanroep: " + request.error);
                return null;
            }
        }
    }

    public async void Register()
    {
        string username = "test1";
        string password = "TestPassword1!";

        var register = new RegisterDto(username, password);
        var request = JsonUtility.ToJson(register);

        Debug.Log(request);

        string url = $"{baseUrl}/account/register";

        await ApiCall(url, "POST", request);
    }

    public async void Login()
    {
        string username = "test1";
        string password = "TestPassword1!";

        var login = new LoginDto(username, password);
        var request = JsonUtility.ToJson(login);

        Debug.Log(request);

        string url = $"{baseUrl}/account/login";

        var response = await ApiCall(url, "POST", request);

        if (!string.IsNullOrEmpty(response))
        {
            LoginResponseDto responseDto = JsonUtility.FromJson<LoginResponseDto>(response);
            accessToken = responseDto.token;

            Debug.Log(accessToken);

            SceneManager.LoadScene("EnvironmentScreen");
        }
    }

    public async void CreateEnvironment()
    {
        string name = "testEnv1";
        double maxHeight = 350;
        double maxWidth = 350;

        var environment = new EnvironmentDto(name, maxHeight, maxWidth);
        var request = JsonUtility.ToJson(environment);

        Debug.Log(request);

        string url = $"{baseUrl}/environments/create";

        await ApiCall(url, "POST", request, accessToken);
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

        return new List<string>();
    }
}
