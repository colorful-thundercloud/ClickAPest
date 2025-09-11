using UnityEngine;

public class GameDataHolder : MonoBehaviour
{
    private static GameDataHolder _instance;
    private IngameObjectData[] pestsData;
    private CatcherData[] catchersData;
    private ItemData[] itemsPestsData, itemsCatchersData;

    public enum DataType
    {
        Pest, Catcher, ItemPest, ItemCatcher
    }

    private void Awake()
    {
        if (_instance == null)
        {
            DontDestroyOnLoad(gameObject);
            _instance = this;
        }
        else if (_instance != this)
            Destroy(gameObject);

        pestsData = Resources.LoadAll<IngameObjectData>("PestsData/");
        catchersData = Resources.LoadAll<CatcherData>("CatchersData/");
        itemsPestsData = Resources.LoadAll<ItemData>("ItemsPestsData/");
        itemsCatchersData = Resources.LoadAll<ItemData>("ItemsCatchersData/");
    }

    public static IngameObjectData GetDataByIndex(DataType type, int index = 0)
    {
        return type switch
        {
            DataType.Pest => _instance.pestsData.Length > index ? _instance.pestsData[index] : null,
            DataType.Catcher => _instance.catchersData.Length > index ? _instance.catchersData[index] : null,
            DataType.ItemPest => _instance.itemsPestsData.Length > index ? _instance.itemsPestsData[index] : null,
            DataType.ItemCatcher => _instance.itemsCatchersData.Length > index ? _instance.itemsCatchersData[index] : null,
            _ => null
        };
    }

    public static int GetDataLength(DataType type)
    {
        return type switch
        {
            DataType.Pest => _instance.pestsData.Length,
            DataType.Catcher => _instance.catchersData.Length,
            DataType.ItemPest => _instance.itemsPestsData.Length,
            DataType.ItemCatcher => _instance.itemsCatchersData.Length,
            _ => 0,
        };
    }
}
