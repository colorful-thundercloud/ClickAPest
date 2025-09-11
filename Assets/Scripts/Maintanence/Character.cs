using ExitGames.Client.Photon;
using Photon.Pun;
using TMPro;
using UnityEngine;

public abstract class Character : MonoBehaviourPunCallbacks
{
    public TextMeshPro textYou, textNickname;
    public SpriteRenderer sprite;
    [HideInInspector]
    public Color colorNormal, colorDamaged;
    [HideInInspector]
    public PhotonView view;
    [HideInInspector]
    public AudioClip sound;

    int maxHealth = 3, health = 3, scoreAddValue = 10, score;

    public virtual int MaxHealth => maxHealth;
    public virtual int Health
    {
        get => health;
        set { health = value; }
    }

    public virtual int ScoreAddValue => scoreAddValue;

    public virtual int Score
    {
        get => score;
        set
        {
            score = value;
            if (view != null)
            {
                playerProperties = view.Owner.CustomProperties;
                playerProperties["score"] = score;
                view.Owner.SetCustomProperties(playerProperties);
            }
        }
    }
    bool isEliminated;
    public virtual bool IsEliminated
    {
        get => isEliminated;
        set { isEliminated = value; }
    }
    public Hashtable playerProperties = new();
    [PunRPC]
    public void TriggerSetInfo(int playerId)
    {
        PhotonView targetView = PhotonView.Find(playerId);
        if (targetView != null) targetView.GetComponent<Character>().SetInfo(GameDataHolder.GetDataByIndex((GameDataHolder.DataType)targetView.Owner.CustomProperties["character"], (int)targetView.Owner.CustomProperties["skinNumber"]));
    }

    public virtual void SetInfo(IngameObjectData originalData)
    {
        sprite.sprite = originalData.inGameSprite;
        sound = originalData.sound;
    }
    public virtual void SetInitialColor(Color initialColor)
    {
        colorNormal = initialColor;
        colorDamaged = new Color(initialColor.r, initialColor.g, initialColor.b, 0.5f);

        SetColor(colorNormal);
    }

    public void SetColor(Color newColor) => sprite.color = newColor;
    [PunRPC]
    public abstract void TakeDamage();
    public virtual void Eliminate()
    {
        IsEliminated = true;
        SetColor(colorDamaged);
    }
    public virtual void Respawn()
    {
        Health = MaxHealth;
        SetColor(colorNormal);
        IsEliminated = false;
    }
}
