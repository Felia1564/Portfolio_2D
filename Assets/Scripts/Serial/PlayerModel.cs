using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerModel
{
    public int PlayerHP;
    public string LastMapDataID;
    public Vector2 LastMapPosition;
    public string ItemDataID;
    public string QuestDataID;

    public List<ItemModel> ItemList;
}

[Serializable]
public class ItemModel
{
    public long ItemUniqueId;
    public string ItemDataId;
    public int ItemStackCount;
}