using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemCard : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI priceText;
    ItemData thisItem;

    public void SetInfo(ItemData item)
    {
        thisItem = item;
        if (item == null)
        {
            Destroy(gameObject);
            return;
        }
        title.text = thisItem.title;
        image.sprite = thisItem.inGameSprite;
        priceText.text = thisItem.price + " points";
    }

    public void Buy()
    {
        GameOnlineManager.Buy.Invoke(thisItem);
    }

}
