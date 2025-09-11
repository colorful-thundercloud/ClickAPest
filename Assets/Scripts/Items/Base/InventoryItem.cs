using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviourPunCallbacks
{
    [SerializeField] Image inventorySprite;
    [SerializeField] TextMeshProUGUI itemNumber;
    ItemData itemData;
    GameObject inGameObj;
    GameObject localPlayer;

    public void SetData(ItemData data, int index)
    {
        itemData = data;
        itemNumber.text = index.ToString();
        inventorySprite.sprite = itemData.selectionSprite;
        GetComponent<Button>().onClick.AddListener(() => Use());

        localPlayer = GameObject.FindGameObjectWithTag("Player");
        inGameObj = itemData.inGamePrefab;
    }

    public void Use()
    {
        inGameObj = PhotonNetwork.Instantiate(itemData.inGamePrefab.name, Vector3.zero, Quaternion.identity);

        if (itemData.spawnLocation == ItemData.SpawnLocation.PlayerLocation)
            inGameObj.transform.position = localPlayer.transform.position;
        else if (itemData.spawnLocation == ItemData.SpawnLocation.PlayerLocationAndPlayerAsParent)
        {
            inGameObj.transform.position = localPlayer.transform.position;
            inGameObj.transform.SetParent(localPlayer.transform);
        }
        else if (itemData.spawnLocation == ItemData.SpawnLocation.MouseCursor)
            inGameObj.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        inGameObj.GetComponent<InGameItem>().SetData(itemData);
        Inventory.RemoveItem(itemData);
    }
}
