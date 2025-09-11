using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameOnlineManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject pestPrefab, catcherPrefab, itemCardPrefab;
    [SerializeField] private Transform scoresTableParent, shopItemsParent;
    [SerializeField] private GameObject scoresEntryPrefab, scoresBtnArrowImg, panelShop, shopItemCardPrefab;
    [SerializeField] private TextMeshProUGUI textTimer, textAnnouncer, textPoints;
    [SerializeField] private GameObject shopErrorPrefab;

    private GameObject localPlayer, spawnedObject, scoresRow;
    private Character cha;

    private List<Player> players = new();
    private List<Pest> playersPests = new();
    private List<Catcher> playersCatchers = new();
    private List<Character> playersAll = new();

    private const float MaxTimer = 1f * 60; // minutes in round
    private const float ShopTimer = 1f * 30; // seconds to buy items
    private float timer, minutes, seconds;

    private int eliminatedPests, eliminatedCatchers;

    [HideInInspector]
    public bool roundEnd, roundShop;

    private Coroutine coroutine;
    private Hashtable playerProps = new();

    public static UnityEvent<ItemData> Buy = new();

    private void Start()
    {
        Buy.AddListener(ctx => BuyItem(ctx));

        textPoints.text = "0 points";
        eliminatedPests = eliminatedCatchers = 0;
        timer = MaxTimer;
        panelShop.SetActive(false);

        if ((GameDataHolder.DataType)PhotonNetwork.LocalPlayer.CustomProperties["character"] == GameDataHolder.DataType.Pest)
        {
            spawnedObject = pestPrefab;
            localPlayer = PhotonNetwork.Instantiate(spawnedObject.name, new Vector3(Random.Range(3, -3), Random.Range(3, -4), -3), Quaternion.identity);
        }
        else
        {
            spawnedObject = catcherPrefab;
            localPlayer = PhotonNetwork.Instantiate(spawnedObject.name, new Vector3(Random.Range(3, -3), Random.Range(3, -4), -5), Quaternion.identity);
        }

        localPlayer.tag = "Player";
        cha = localPlayer.GetComponent<Character>();
        cha.GetComponent<PhotonView>().RPC("TriggerSetInfo", RpcTarget.AllBuffered, cha.GetComponent<PhotonView>().ViewID);

        UpdateScoresList();
        Invoke(nameof(GetAllPlayers), 3);
    }

    private void GetAllPlayers()
    {
        playersAll = FindObjectsByType<Character>(FindObjectsSortMode.None).ToList();
        playersPests = FindObjectsByType<Pest>(FindObjectsSortMode.None).ToList();
        playersCatchers = FindObjectsByType<Catcher>(FindObjectsSortMode.None).ToList();

        coroutine = StartCoroutine(AnnouncerText("GO!"));
    }

    private void FixedUpdate()
    {
        if (!roundEnd && playersAll.Count > 1)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
                minutes = Mathf.FloorToInt(timer / 60);
                seconds = Mathf.FloorToInt(timer % 60);

                foreach (Pest ch in playersPests)
                {
                    if (ch.IsEliminated) eliminatedPests++;
                }

                if (eliminatedPests == playersPests.Count)
                {
                    coroutine = StartCoroutine(AnnouncerText("Gothca!"));
                    RoundEnd();
                }
                else
                {
                    foreach (Catcher ch in playersCatchers)
                    {
                        if (ch.IsEliminated) eliminatedCatchers++;
                    }
                    if (eliminatedCatchers == playersCatchers.Count)
                    {
                        coroutine = StartCoroutine(AnnouncerText("Nice dodge!"));
                        RoundEnd();
                    }
                }
                eliminatedPests = eliminatedCatchers = 0;
            }
            else
            {
                minutes = seconds = 0;
                coroutine = StartCoroutine(AnnouncerText("Time's up!"));
                RoundEnd();
            }
        }
        else if (roundShop)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
                minutes = Mathf.FloorToInt(timer / 60);
                seconds = Mathf.FloorToInt(timer % 60);
            }
            else
            {
                minutes = seconds = 0;
                coroutine = StartCoroutine(AnnouncerText("Time's up!"));
                RoundShopEnd();
            }
        }
        textTimer.text = $"{minutes:00}:{seconds:00}";
        textPoints.text = (int)PhotonNetwork.LocalPlayer.CustomProperties["score"] + " points";
    }

    private void UpdateScoresList()
    {
        for (int i = 1; i < scoresTableParent.childCount; i++) Destroy(scoresTableParent.GetChild(i).gameObject);

        players = PhotonNetwork.PlayerList.ToList();
        players.OrderByDescending(x => x.CustomProperties.TryGetValue("score", out _));

        foreach (Player player in players)
        {
            scoresRow = Instantiate(scoresEntryPrefab, scoresTableParent);
            scoresRow.GetComponent<ScoresRow>().SetInfo(player);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        UpdateScoresList();
    }

    public void ToggleScores()
    {
        for (int i = 1; i < scoresTableParent.childCount; i++) scoresTableParent.GetChild(i).gameObject.SetActive(!scoresTableParent.GetChild(i).gameObject.activeSelf);
        scoresBtnArrowImg.transform.Rotate(new Vector3(0, 0, 180));
    }

    public void BackToLobby()
    {
        if (PhotonNetwork.InRoom)
        {
            playerProps["isReady"] = false;
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);

            PhotonNetwork.LeaveRoom();
            PhotonNetwork.LoadLevel("Main menu");
        }
        else SceneManager.LoadScene("Main menu");
    }

    private void RoundStart()
    {
        foreach (Character ch in playersAll) ch.Respawn();

        timer = MaxTimer;
        minutes = seconds = 0;

        roundShop = false;
        roundEnd = false;

        coroutine = StartCoroutine(AnnouncerText("GO!"));
    }

    private void RoundEnd()
    {
        roundEnd = true;

        Invoke(nameof(RoundShopStart), 3);
    }

    private void RoundShopStart()
    {
        panelShop.SetActive(true);

        for (int i = 0; i < 3; i++)
        {
            spawnedObject = Instantiate(shopItemCardPrefab, shopItemsParent);
            if ((GameDataHolder.DataType)PhotonNetwork.LocalPlayer.CustomProperties["character"] == GameDataHolder.DataType.Pest)
            {
                spawnedObject.GetComponent<ItemCard>().SetInfo(GameDataHolder.GetDataByIndex(GameDataHolder.DataType.ItemPest, Random.Range(0, GameDataHolder.GetDataLength(GameDataHolder.DataType.ItemPest))) as ItemData);
            }
            else
            {
                spawnedObject.GetComponent<ItemCard>().SetInfo(GameDataHolder.GetDataByIndex(GameDataHolder.DataType.ItemCatcher, Random.Range(0, GameDataHolder.GetDataLength(GameDataHolder.DataType.ItemCatcher))) as ItemData);
            }
        }

        timer = ShopTimer;
        minutes = seconds = 0;

        roundShop = true;
    }

    private void RoundShopEnd()
    {
        panelShop.SetActive(false);

        foreach (Transform card in shopItemsParent) Destroy(card.gameObject);

        Invoke(nameof(RoundStart), 1);
    }

    private IEnumerator AnnouncerText(string text)
    {
        textAnnouncer.gameObject.SetActive(true);
        textAnnouncer.text = text;

        yield return new WaitForSeconds(2f);

        textAnnouncer.gameObject.SetActive(false);
    }

    private void BuyItem(ItemData item)
    {
        if (Inventory.HasEmptySlots())
        {
            if ((int)PhotonNetwork.LocalPlayer.CustomProperties["score"] >= item.price)
            {
                localPlayer.GetComponent<Character>().Score = localPlayer.GetComponent<Character>().Score - item.price;
                panelShop.SetActive(false);
                Inventory.AddItem(item);
            }
            else
            {
                spawnedObject = Instantiate(shopErrorPrefab, Input.mousePosition, Quaternion.identity, FindFirstObjectByType<Canvas>().transform);
                spawnedObject.GetComponentInChildren<TextMeshProUGUI>().text = "Not enough scorepoints!";
                Destroy(spawnedObject, 3f);
            }
        }
        else
        {
            spawnedObject = Instantiate(shopErrorPrefab, Input.mousePosition, Quaternion.identity, FindFirstObjectByType<Canvas>().transform);
            spawnedObject.GetComponentInChildren<TextMeshProUGUI>().text = "Inventory is full!";
            Destroy(spawnedObject, 3f);
        }
    }
}
