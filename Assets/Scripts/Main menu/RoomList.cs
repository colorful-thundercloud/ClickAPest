using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class RoomList : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject rowPrefab, content;
    GameObject row;
    List<Room> allRooms = new();
    float timeBetweenUpdates = 1.5f;
    bool canUpdate;

    public override void OnRoomListUpdate(List<RoomInfo> roomsList)
    {
        if (canUpdate)
        {
            if (allRooms.Count > 0)
                for (int i = 0; i < allRooms.Count; i++) Destroy(allRooms[i].gameObject);

            allRooms.Clear();
            for (int i = 0; i < roomsList.Count; i++)
            {
                if (roomsList[i].PlayerCount > 0 && roomsList[i].IsOpen && roomsList[i].IsVisible)
                {
                    row = Instantiate(rowPrefab, Vector3.zero, Quaternion.identity, content.transform);
                    row.GetComponent<Room>().SetInfo(roomsList[i].Name, roomsList[i].IsOpen, roomsList[i].PlayerCount, roomsList[i].MaxPlayers);

                    allRooms.Add(row.GetComponent<Room>());
                }
            }
        }
        canUpdate = false;
        StartCoroutine(nameof(UpdateTimer));
    }

    private IEnumerator UpdateTimer()
    {
        yield return new WaitForSeconds(timeBetweenUpdates);
        canUpdate = true;
    }

}
