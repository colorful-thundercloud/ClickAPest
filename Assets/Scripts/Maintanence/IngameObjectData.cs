using UnityEngine;

[CreateAssetMenu(fileName = "Pest 001", menuName = "Scriptable Objects/Create pest")]
public class IngameObjectData : ScriptableObject
{
    public string title;
    public Sprite selectionSprite, inGameSprite;
    [TextArea]
    public string description;
    public AudioClip sound;
}
