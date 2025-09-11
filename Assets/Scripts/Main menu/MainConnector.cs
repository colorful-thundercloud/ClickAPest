using System.Text.RegularExpressions;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class MainConnector : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject loadingScreen;
    [SerializeField] TMP_InputField roomNameInput;
    [SerializeField] TMP_Text roomsErrorsMain;
    public static UnityEvent<string> Join = new();

    private void Start()
    {
        Join.AddListener(ctx => JoinRoom(ctx));
        PhotonNetwork.AutomaticallySyncScene = true; // to synchronize scenes between host and players
        if (!PhotonNetwork.InLobby)
            PhotonNetwork.ConnectUsingSettings();
        
        else
            loadingScreen.SetActive(false);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public void BtnRejoinLobby()
    {
        PhotonNetwork.LeaveLobby();
    }

    public override void OnLeftLobby()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        loadingScreen.SetActive(false);
    }

    public void CleanInput()
    {
        roomNameInput.text = Regex.Replace(roomNameInput.text, @"[^a-zA-Z0-9!@^&*()_+/\-=<>?,. ]", "");
    }

    public void CreateRoom()
    {
        if (roomNameInput.text == "") return;
        PhotonNetwork.JoinOrCreateRoom(roomNameInput.text, new RoomOptions { MaxPlayers = 5, BroadcastPropsChangeToAll = true }, TypedLobby.Default);
    }

    public override void OnCreatedRoom()
    {
        roomsErrorsMain.text = "";
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        roomsErrorsMain.text = message + ". Error code: " + returnCode;
        base.OnCreateRoomFailed(returnCode, message);
    }

    public override void OnJoinedRoom()
    {
        roomsErrorsMain.text = "";
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        loadingScreen.SetActive(false);
        roomsErrorsMain.text = message + ". Error code: " + returnCode;
        base.OnJoinRoomFailed(returnCode, message);
    }

    public void JoinRoom(string room)
    {
        room = room == "" ? roomNameInput.text : room;
        if (room == "") return;
        PhotonNetwork.JoinRoom(room);
    }

    public void BtnTutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void BtnAuthors()
    {
        Application.OpenURL("https://vk.com/toadspoongames");
    }

    public void BtnMoreGames()
    {
        Application.OpenURL("https://colorful-thundercloud.itch.io/");
    }
}
