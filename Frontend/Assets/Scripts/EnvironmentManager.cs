using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnvironmentManager : MonoBehaviour
{
   
    public TMP_InputField envNameInput;
    public TMP_InputField envMaxHeight;
    public TMP_InputField envMaxWidth;

    public TMP_Dropdown userEnvironments;

    public TMP_Text statusText;

    private void Start()
    {
        RefreshDropdown();
    }

    public async void CreateEnvironment()
    {
        string name = envNameInput.text;
        double maxHeight = double.Parse(envMaxHeight.text);
        double maxWidth = double.Parse(envMaxWidth.text);

        statusText.text = await ApiClient.instance.CreateEnvironment(name, maxHeight, maxWidth);

        RefreshDropdown();
    }

    public async void RefreshDropdown()
    {
        userEnvironments.ClearOptions();

        var environments = await ApiClient.instance.GetEnvironments();

        if (environments.Count > 0)
        {
            userEnvironments.AddOptions(environments);
        }
        else
        {
            statusText.text = "No worlds yet.";
        }
    }

    public async void EnterEnvironment()
    {
        int i = userEnvironments.value;
        string envName = userEnvironments.options[i].text;

        await ApiClient.instance.GetEnvironmentByName(envName);
    }

    public async void DeleteEnvironment()
    {
        int i = userEnvironments.value;
        string envName = userEnvironments.options[i].text;

        statusText.text = await ApiClient.instance.DeleteEnvironment(envName);

        RefreshDropdown();
    }
}
