using Photon.Realtime;
using TMPro;
using UnityEngine;

public class ScoresRow : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nickname, score;
    
    public void SetInfo(Player player)
    {
        nickname.text = player.NickName;
        score.text = player.CustomProperties["score"].ToString();
    }
}
