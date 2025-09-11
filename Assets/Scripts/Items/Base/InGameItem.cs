using UnityEngine;

public class InGameItem : MonoBehaviour
{
    public ItemData itemData;

    public void SetData(ItemData data)
    {
        itemData = data;
    }

    void Start()
    {
        Destroy(gameObject, itemData.lifetimeSeconds);
    }
}
