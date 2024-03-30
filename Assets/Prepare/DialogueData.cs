using UnityEngine;

[CreateAssetMenu(fileName = "DialogueData", menuName = "Dialogues/New Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [field: TextArea(1, 3)]
    [field: SerializeField] public string[] Messages { get; private set; }
    [field: SerializeField] public string[] Names { get; private set; }
}