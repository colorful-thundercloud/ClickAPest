using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Lobby : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject loadingScreen, panelLobby;
    [SerializeField] TMP_Text roomsErrorLobby, textCountdown;
    [SerializeField] TextMeshProUGUI textCurrentRoomName;
    [SerializeField] CharacterSelector playerCharSelectPrefab, otherPlayersCharPrefab;
    [SerializeField] Transform playerCharSelectParent, otherPlayersCharSelectParent;

    [Header("Master client settings")]
    [SerializeField] TMP_Dropdown mapSelect;
    [SerializeField] Image mapPreview;
    [SerializeField] Button btnMinus, btnPlus;
    [SerializeField] TMP_InputField maxPlayers;
    [SerializeField] Toggle openRoom;

    Hashtable playerProps = new();
    List<CharacterSelector> otherPlayersList = new();
    CharacterSelector chs;
    GameObject playerCard;

    int counter, catchers, pests;
    Coroutine timerToStart;
    bool isStarting;

    private void Start()
    {
        panelLobby.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.PlayerCount >= 2)
        {
            foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
            {
                if (player.Value.CustomProperties.ContainsKey("isReady"))
                    if ((bool)player.Value.CustomProperties["isReady"]) counter++;
                if (player.Value.CustomProperties.ContainsKey("character"))
                    if ((GameDataHolder.DataType)player.Value.CustomProperties["character"] == GameDataHolder.DataType.Pest)
                        pests++;
                    else catchers++;
            }

            if (pests == 0 || catchers == 0)
                roomsErrorLobby.text = "There should be at least 1 catcher and 1 pest!";
            else roomsErrorLobby.text = "";

            if (counter == PhotonNetwork.CurrentRoom.PlayerCount && roomsErrorLobby.text == "")
            {
                if (!isStarting)
                    timerToStart = StartCoroutine(StartingTimer());
            }
            else
            {
                if (timerToStart != null) StopCoroutine(timerToStart);
                isStarting = false;
                textCountdown.gameObject.SetActive(false);
            }

            counter = pests = catchers = 0;
        }
        else roomsErrorLobby.text = "Need at least 2 players to start!";
    }

    private IEnumerator StartingTimer()
    {
        isStarting = true;
        for (int i = 4; i > 0; i--)
        {
            if (i > 1)
            {
                textCountdown.gameObject.SetActive(true);
                textCountdown.text = (i - 1).ToString();
                yield return new WaitForSeconds(1f);
                textCountdown.gameObject.SetActive(false);
            }
            else
            {
                textCountdown.gameObject.SetActive(true);
                textCountdown.text = "Start!";
                yield return new WaitForSeconds(1f);
            }
        }

        loadingScreen.SetActive(true);
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel(mapSelect.options[mapSelect.value].text);
    }

    public override void OnJoinedRoom()
    {
        panelLobby.SetActive(true);
        textCurrentRoomName.text = PhotonNetwork.CurrentRoom.Name;

        SetMasterSettingsInteractable();

        playerCard = Instantiate(playerCharSelectPrefab.gameObject, playerCharSelectParent);
        playerCard.GetComponent<CharacterSelector>().SetInfo(PhotonNetwork.LocalPlayer, true);

        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            if (player.Value != PhotonNetwork.LocalPlayer)
            {
                playerCard = Instantiate(otherPlayersCharPrefab.gameObject, otherPlayersCharSelectParent);
                chs = playerCard.GetComponent<CharacterSelector>();

                chs.SetInfo(player.Value, false);

                otherPlayersList.Add(chs);
            }
        }
    }

    public void LeaveRoom()
    {
        playerProps = PhotonNetwork.LocalPlayer.CustomProperties;
        playerProps["isReady"] = false;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);
        
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        foreach (Transform child in otherPlayersCharSelectParent) Destroy(child.gameObject);
        otherPlayersList.Clear();

        foreach (Transform child in playerCharSelectParent) Destroy(child.gameObject);

        panelLobby.SetActive(false);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        playerCard = Instantiate(otherPlayersCharPrefab.gameObject, otherPlayersCharSelectParent);
        chs = playerCard.GetComponent<CharacterSelector>();

        chs.SetInfo(newPlayer, false);

        otherPlayersList.Add(chs);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Destroy(otherPlayersList.Find(x => x.player == otherPlayer).GetComponent<CharacterSelector>().gameObject);
        otherPlayersList.Remove(otherPlayersList.Find(x => x.player == otherPlayer));

        SetMasterSettingsInteractable();
    }

    void SetMasterSettingsInteractable()
    {
        mapSelect.interactable = PhotonNetwork.IsMasterClient;
        mapPreview.sprite = mapSelect.options[mapSelect.value].image;
        btnMinus.interactable = PhotonNetwork.IsMasterClient;
        btnPlus.interactable = PhotonNetwork.IsMasterClient;
        maxPlayers.interactable = PhotonNetwork.IsMasterClient;
        openRoom.interactable = PhotonNetwork.IsMasterClient;
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        maxPlayers.text = PhotonNetwork.CurrentRoom.MaxPlayers.ToString();
        openRoom.isOn = PhotonNetwork.CurrentRoom.IsOpen;
    }

    public void SetRoomOpen()
    {
        PhotonNetwork.CurrentRoom.IsOpen = openRoom.isOn;
    }

    public void MaxPlayersPlus()
    {
        if (int.Parse(maxPlayers.text) < 20) maxPlayers.text = (int.Parse(maxPlayers.text) + 1).ToString();
        PhotonNetwork.CurrentRoom.MaxPlayers = int.Parse(maxPlayers.text);
    }

    public void MaxPlayersMinus()
    {
        if (int.Parse(maxPlayers.text) > 5) maxPlayers.text = (int.Parse(maxPlayers.text) - 1).ToString();
        PhotonNetwork.CurrentRoom.MaxPlayers = int.Parse(maxPlayers.text);
    }

    public void MaxPlayerClearInput()
    {
        if (maxPlayers.text == "") maxPlayers.text = "5";
        maxPlayers.text = Regex.Replace(maxPlayers.text, @"[^0-9]", "");
        if (int.Parse(maxPlayers.text) > 20) maxPlayers.text = "20";
        else if (int.Parse(maxPlayers.text) < 5) maxPlayers.text = "5";

        PhotonNetwork.CurrentRoom.MaxPlayers = int.Parse(maxPlayers.text);
    }


}
