using System.Text.RegularExpressions;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class Nickname : MonoBehaviour
{
    [SerializeField] TMP_InputField nicknameField;
    [SerializeField] TMP_Text nicknameHelpText;
    string nickname = "";

    private void Start()
    {
        nickname = PlayerPrefs.GetString("Nickname");
        if (nickname != "")
        {
            PhotonNetwork.NickName = nickname;
            nicknameField.text = nickname;
        }
        nicknameField.characterLimit = 15;
    }

    public void CleanInput()
    {
        nicknameField.text = Regex.Replace(nicknameField.text, @"[^a-zA-Z0-9!@^&*()_+\-=<>?,.]", "");
        if (nicknameField.text.Length > 2 && nicknameField.text.Length < 16) SaveNickname();
        else nicknameHelpText.text = "Nickname should be from 3 to 15 symbols long!";
    }

    public void SaveNickname()
    {
        nicknameHelpText.text = "";
        nickname = nicknameField.text;
        PhotonNetwork.NickName = nickname;
        PlayerPrefs.SetString("Nickname", nickname);
    }

}
