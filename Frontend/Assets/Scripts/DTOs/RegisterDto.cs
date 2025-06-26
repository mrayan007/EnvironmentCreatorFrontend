[System.Serializable]
public class RegisterDto
{
    public string Username;
    public string Password;

    public RegisterDto(string username, string password)
    {
        Username = username;
        Password = password;
    }
}
