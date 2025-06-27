using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class AuthController : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TMP_Text statusText;

    public async void Register()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        statusText.text = await ApiClient.instance.Register(username, password);
    }

    public async void Login()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        await ApiClient.instance.Login(username, password);

        SceneManager.LoadScene("EnvironmentScreen");
    }
}