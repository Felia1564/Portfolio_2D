//using System;
//using System.Collections.Generic;
//using UnityEngine;

//[Serializable]
//public class PlayerModel
//{
//    public int PlayerHP;
//    public string LastMapDataID;
//    public Vector2 LastMapPosition;
//    public string ItemDataID;
//    public string QuestDataID;

//    public List<ItemModel> ItemList;
//}

//[Serializable]
//public class ItemModel
//{
//    public long ItemUniqueId;
//    public string ItemDataId;
//    public int ItemStackCount;
//}

using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerModel
{
    public int MaxHp { get; set; }
    public int CurrentHp { get; set; }

    public string LastMapDataID { get; set; }
    public Vector2 LastMapPosition { get; set; }
    public string ItemDataID { get; set; }
    public string QuestDataID { get; set; }

    public List<ItemModel> ItemList { get; set; } = new List<ItemModel>();

    // 게임 시작 시 JSON(GameData)에서 읽어온 값으로 초기화
    public void InitModel(int baseMaxHp)
    {
        MaxHp = baseMaxHp;
        CurrentHp = baseMaxHp;
    }

    // 체력 증감 함수
    public void ModifyHp(int amount)
    {
        CurrentHp += amount;
        if (CurrentHp > MaxHp) CurrentHp = MaxHp;
        if (CurrentHp < 0) CurrentHp = 0;
    }
}

[Serializable]
public class ItemModel
{
    public long ItemUniqueId { get; set; }
    public string ItemDataId { get; set; }
    public int ItemStackCount { get; set; }
}