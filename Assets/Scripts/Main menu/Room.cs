using TMPro;
using UnityEngine;

public class Room : MonoBehaviour
{
    public TextMeshProUGUI roomName, openStatus, playersNum;
    
    public void JoinRoom()
    {
        MainConnector.Join.Invoke(roomName.text);
    }

    public void SetInfo(string name, bool IsOpen, int players, int maxP)
    {
        roomName.text = name;
        playersNum.text = players + "/" + maxP;
        openStatus.text = IsOpen ? "Open" : "Closed";
    }
}
