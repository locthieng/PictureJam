using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetController : MonoBehaviour
{
    public static AssetController Instance { get; set; }
    public List<Sprite> ListSkinSprites = new List<Sprite>();
    public List<Sprite> ListHeadsetSprites = new List<Sprite>();
    public List<Sprite> ListDecoSprites = new List<Sprite>();
    public List<ShopItemData> ListSkinItemData = new List<ShopItemData>();
    public List<ShopItemData> ListWeaponItemData = new List<ShopItemData>();
    public List<ShopItemData> ListItemsToUnlock = new List<ShopItemData>();
    public Dictionary<int, ShopItemData> WeaponItems = new Dictionary<int, ShopItemData>();
    public Dictionary<int, ShopItemData> SkinItems = new Dictionary<int, ShopItemData>();
    //public List<CollectionDecoData> ListDecoData = new List<CollectionDecoData>();
    //public Dictionary<int, CollectionDecoData> Decos = new Dictionary<int, CollectionDecoData>();
    //public Dictionary<int, List<int>> ListUnlockedDecoItems = new Dictionary<int, List<int>>();

    private void Awake()
    {
        Instance = this;
        for (int i = 0; i < ListSkinItemData.Count; i++)
        {
            if (ListSkinItemData[i].LevelLimit != 0)
            {
                ListItemsToUnlock.Add(ListSkinItemData[i]);
            }
            SkinItems.Add(ListSkinItemData[i].ID, ListSkinItemData[i]);
        }
        for (int i = 0; i < ListWeaponItemData.Count; i++)
        {
            if (ListWeaponItemData[i].LevelLimit != 0)
            {
                ListItemsToUnlock.Add(ListWeaponItemData[i]);
            }
            WeaponItems.Add(ListWeaponItemData[i].ID, ListWeaponItemData[i]);
        }
        //for (int i = 0; i < ListDecoData.Count; i++)
        //{
        //    Decos.Add(ListDecoData[i].DecoID, ListDecoData[i]);
        //    ListUnlockedDecoItems.Add(ListDecoData[i].DecoID, new List<int>());
        //}
    }

    private int ShopItemComparer(ShopItemData x, ShopItemData y)
    {
        if (x.IsUnlocked) return -1;
        if (y.IsUnlocked) return 1;
        if (x.LevelLimit < y.LevelLimit)
        {
            return -1;
        }
        else if (x.LevelLimit < y.LevelLimit)
        {
            return 1;
        }
        else
        {
            if (x.UCTotal < y.UCTotal)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
    }

    //private void OnValidate()
    //{
    //    if (ListSkinSprites.Count == 0)
    //    {
    //        ListSkinSprites.Add(Sprite.Create(new Texture2D(0, 0), new Rect(), Vector2.zero));
    //    }
    //    ListItemData.Clear();
    //    for (int i = 0; i < ListSkinSprites.Count; i++)
    //    {
    //        ListItemData.Add(new ShopItemData()
    //        {
    //            ID = i,
    //            AvatarSprite = ListSkinSprites[i],
    //            UCTotal = i == 0 ? 0 : 1
    //        });
    //    }
    //}

    /// <summary>
    /// Used only by DataController to refresh the SkinIDs list
    /// </summary>
    /// <param name="id"></param>
    public void ClaimSkinID(int id)
    {
        if (SkinItems[id].IsUnlocked)
        {
            DataController.Instance.Data.SkinIDs.Add(id);
        }
    }

    /// <summary>
    /// Used only by DataController to refresh the WeaponIDs list
    /// </summary>
    /// <param name="id"></param>
    public void ClaimWeaponID(int id)
    {
        if (WeaponItems[id].IsUnlocked)
        {
            DataController.Instance.Data.WeaponIDs.Add(id);
        }
    }

    public void UpdateSkinItemUC(int id, int current, bool saveData = true)
    {
        if (current < DataController.Instance.Data.SkinUnlockProgress[id]) return;
        SkinItems[id].UCCurrent = current;
        if (saveData)
        {
            DataController.Instance.Data.SkinUnlockProgress[id] = current;
            DataController.Instance.SaveData();
        }
    }

    public void UnlockSkin(int id)
    {
        UpdateSkinItemUC(id, SkinItems[id].UCTotal);
        DataController.Instance.Data.SkinID = id;
    }

    public void AddSkin()
    {
        ListSkinItemData.Add(new ShopItemData()
        {
            ID = ListSkinItemData.Count,
            AvatarSprite = ListSkinSprites.Count == 0 ? null : ListSkinSprites[ListSkinSprites.Count - 1],
            UCTotal = 1,
            Type = ItemType.Skin
        });
        ListSkinSprites.Add(ListSkinSprites.Count == 0 ? null : ListSkinSprites[ListSkinSprites.Count - 1]);
    }

    public void RemoveSkin(int index)
    {
        ListSkinItemData.RemoveAt(index);
        ListSkinSprites.RemoveAt(index);
    }

    public void UpdateWeaponItemUC(int id, int current, bool saveData = true)
    {
        if (current < DataController.Instance.Data.WeaponUnlockProgress[id] || !WeaponItems.ContainsKey(id)) return;
        WeaponItems[id].UCCurrent = current;
        if (saveData)
        {
            DataController.Instance.Data.WeaponUnlockProgress[id] = current;
            DataController.Instance.SaveData();
        }
    }

    public void UnlockWeapon(int id)
    {
        UpdateWeaponItemUC(id, WeaponItems[id].UCTotal);
        DataController.Instance.Data.WeaponID = id;
    }

    public void UnlockItem(ShopItemData item)
    {
        switch (item.Type)
        {
            case ItemType.Skin:
                UnlockSkin(item.ID);
                break;
            case ItemType.Weapon:
                UnlockWeapon(item.ID);
                break;
            default:
                break;
        }
    }

    public void AddWeapon()
    {
        ListWeaponItemData.Add(new ShopItemData()
        {
            ID = ListWeaponItemData.Count,
            AvatarSprite = ListHeadsetSprites.Count == 0 ? null : ListHeadsetSprites[ListHeadsetSprites.Count - 1],
            UCTotal = 1,
            Type = ItemType.Weapon
        });
        ListHeadsetSprites.Add(ListHeadsetSprites.Count == 0 ? null : ListHeadsetSprites[ListHeadsetSprites.Count - 1]);
    }

    public void RemoveWeapon(int index)
    {
        ListWeaponItemData.RemoveAt(index);
        ListHeadsetSprites.RemoveAt(index);
    }

    //public void AddDeco()
    //{
    //    ListDecoData.Add(new CollectionDecoData()
    //    {
    //        DecoID = ListDecoData.Count,
    //        Avatar = ListDecoSprites.Count == 0 ? null : ListDecoSprites[ListDecoSprites.Count - 1],
    //        ListDecoItems = new List<ShopItemData>()
    //    });
    //    ListDecoSprites.Add(ListDecoSprites.Count == 0 ? null : ListDecoSprites[ListDecoSprites.Count - 1]);
    //}

    //public void RemoveDeco(int index)
    //{
    //    ListDecoData.RemoveAt(index);
    //    ListDecoSprites.RemoveAt(index);
    //}

    //public void AddDecoItem(int decoID)
    //{
    //    ListDecoData[decoID].ListDecoItems.Add(new ShopItemData()
    //    {
    //        ID = ListDecoData[decoID].ListDecoItems.Count,
    //        UCTotal = 1,
    //        Type = ItemType.Deco
    //    });
    //}

    //public void RemoveDecoItem(int decoID, int itemID)
    //{
    //    ListDecoData[decoID].ListDecoItems.RemoveAt(itemID);
    //}

    //public void ClaimDecoItemID(int decoID, int itemID)
    //{
    //    ListDecoData[decoID].ListDecoItems[itemID].UCCurrent = ListDecoData[decoID].ListDecoItems[itemID].UCTotal;
    //    DataController.Instance.Data.DecoItemIDs[decoID] += "," + itemID;
    //    ListUnlockedDecoItems[decoID].Add(itemID);
    //}
}
