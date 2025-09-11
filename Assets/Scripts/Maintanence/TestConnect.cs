using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class TestConnect : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject charPrefab;
    [SerializeField] List<ItemData> itemsToAdd;
    GameObject player;

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();

    }
    public override void OnJoinedLobby()
    {
        PhotonNetwork.JoinOrCreateRoom("test", new RoomOptions { MaxPlayers = 5, BroadcastPropsChangeToAll = true }, TypedLobby.Default);

    }
    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            player = PhotonNetwork.Instantiate(charPrefab.name, new Vector3(Random.Range(3, -3), Random.Range(3, -4), -5), Quaternion.identity);
            player.tag = "Player";
            
            foreach (ItemData item in itemsToAdd) Inventory.AddItem(item);
        }
    }
}
