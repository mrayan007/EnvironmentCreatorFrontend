[System.Serializable]
public class EnvironmentDto
{
    public string Name;
    public double MaxHeight;
    public double MaxWidth;

    public EnvironmentDto(string name, double maxHeight, double maxWidth) 
    {
        Name = name;
        MaxHeight = maxHeight;
        MaxWidth = maxWidth;
    }
}