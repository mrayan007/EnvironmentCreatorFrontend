using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
   public void CreateEnvironment()
    {
        ApiClient.instance.CreateEnvironment();
    }
}
