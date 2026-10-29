using UnityEngine;

public class DisplayNameAttribute : PropertyAttribute
{
    public string DisplayName { get; private set; }

    public DisplayNameAttribute(string displayName)
    {
        DisplayName = displayName;
    }
}
