using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class CharacterSelector : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI textNickname, textCharTitle, textCharDescr;
    public Image image;
    public Button btnPest, btnCatch, btnPrev, btnNext;
    public Toggle toggleReady;
    bool isReady;
    public Player player;
    Hashtable playerProperties = new();

    public void SetInfo(Player targetPlayer, bool isLocal)
    {
        player = targetPlayer;
        textNickname.text = player.NickName;

        if (player.CustomProperties.ContainsKey("character"))
        {
            playerProperties = player.CustomProperties;
        }
        else
        {
            playerProperties["character"] = GameDataHolder.DataType.Pest;
            playerProperties["skinNumber"] = 0;
            playerProperties["isReady"] = false;
            playerProperties["score"] = 0;
        }

        if (isLocal)
        {
            btnPest.gameObject.SetActive(true);
            btnCatch.gameObject.SetActive(true);
            btnPrev.gameObject.SetActive(true);
            btnNext.gameObject.SetActive(true);
            toggleReady.interactable = true;
            SetVisualData();
        }
        else toggleReady.interactable = false;

        player.SetCustomProperties(playerProperties);
    }

    void SetVisualData()
    {
        textCharTitle.text = GameDataHolder.GetDataByIndex((GameDataHolder.DataType)playerProperties["character"], (int)playerProperties["skinNumber"]).title;
        textCharDescr.text = GameDataHolder.GetDataByIndex((GameDataHolder.DataType)playerProperties["character"], (int)playerProperties["skinNumber"]).description;
        image.sprite = GameDataHolder.GetDataByIndex((GameDataHolder.DataType)playerProperties["character"], (int)playerProperties["skinNumber"]).selectionSprite;
    }

    public void SetCharacterPest()
    {
        playerProperties["character"] = GameDataHolder.DataType.Pest;
        playerProperties["skinNumber"] = 0;

        btnPest.interactable = false;
        btnCatch.interactable = true;

        SetVisualData();
        player.SetCustomProperties(playerProperties);
    }

    public void SetCharacterCatcher()
    {
        playerProperties["character"] = GameDataHolder.DataType.Catcher;
        playerProperties["skinNumber"] = 0;

        btnCatch.interactable = false;
        btnPest.interactable = true;

        SetVisualData();
        player.SetCustomProperties(playerProperties);
    }

    public void ClickLeft() // prev skin
    {
        if ((int)playerProperties["skinNumber"] > 0) playerProperties["skinNumber"] = (int)playerProperties["skinNumber"] - 1;
        else playerProperties["skinNumber"] = GameDataHolder.GetDataLength((GameDataHolder.DataType)playerProperties["character"]) - 1;

        SetVisualData();
        player.SetCustomProperties(playerProperties);
    }

    public void ClickRight() // next skin
    {
        if ((int)playerProperties["skinNumber"] == GameDataHolder.GetDataLength((GameDataHolder.DataType)playerProperties["character"]) - 1) playerProperties["skinNumber"] = 0;
        else playerProperties["skinNumber"] = (int)playerProperties["skinNumber"] + 1;

        SetVisualData();
        player.SetCustomProperties(playerProperties);
    }

    public void ReadyOrNot()
    {
        isReady = !isReady;
        toggleReady.isOn = isReady;
        playerProperties["isReady"] = isReady;

        player.SetCustomProperties(playerProperties);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (player == targetPlayer)
        {
            if (!player.IsLocal)
            {
                if (player.CustomProperties.ContainsKey("isReady"))
                    toggleReady.gameObject.SetActive((bool)player.CustomProperties["isReady"]);
                else toggleReady.gameObject.SetActive(false);
            }

            if (player.CustomProperties.ContainsKey("character"))
                image.sprite = GameDataHolder.GetDataByIndex((GameDataHolder.DataType)player.CustomProperties["character"], (int)player.CustomProperties["skinNumber"]).selectionSprite;
            else
                image.sprite = GameDataHolder.GetDataByIndex(GameDataHolder.DataType.Pest).selectionSprite;
        }
    }
}
