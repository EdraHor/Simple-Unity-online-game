using UnityEngine;

[CreateAssetMenu(fileName ="UI_Resources", menuName ="Resources/UI")]
public class UI_ResourcesSO : ScriptableObject
{
    [Header("Messenger")]
    public UI_Message Chat_GlobalMessage;
    public UI_Message Chat_LocalMessage;
    public UI_Message Chat_SystemMessage;
}
