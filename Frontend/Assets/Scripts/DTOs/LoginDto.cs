[System.Serializable]
public class LoginDto
{
    public string Username;
    public string Password;

    public LoginDto(string username, string password)
    {
        Username = username;
        Password = password;
    }
}
