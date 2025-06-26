using TMPro;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    public TMP_InputField envNameInput;
    public TMP_InputField envMaxHeight;
    public TMP_InputField envMaxWidth;
    public TMP_Text statusText;

   public async void CreateEnvironment()
    {
        string name = envNameInput.text;
        double maxHeight = double.Parse(envMaxHeight.text);
        double maxWidth = double.Parse(envMaxWidth.text);

        statusText.text = await ApiClient.instance.CreateEnvironment(name, maxHeight, maxWidth);
    }

    public async void GetEnvironments()
    {
        var environments = await ApiClient.instance.GetEnvironments();
        foreach (var env in environments)
        {
            Debug.Log("Environment name: " + env);
        }
    }

    public async void EnterWorld()
    {
        var env = await ApiClient.instance.GetEnvironmentByName("testEnv");
    }
}
