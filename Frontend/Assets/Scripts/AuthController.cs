using UnityEngine;
using TMPro;

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
}
