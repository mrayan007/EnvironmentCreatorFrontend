using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
   public void CreateEnvironment()
    {
        ApiClient.instance.CreateEnvironment();
    }

    public async void GetEnvironments()
    {
        var environments = await ApiClient.instance.GetEnvironments();
        foreach (var env in environments)
        {
            Debug.Log("Environment name: " + env);
        }
    }
}
