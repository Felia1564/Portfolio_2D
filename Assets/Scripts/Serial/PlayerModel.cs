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
    public string QuestDataID { get; set; }


    // =======================================================================
    // 기획 반영: 복잡한 ItemModel을 지우고, 용도에 맞게 리스트를 2개로 분리했습니다.
    // =======================================================================

    // 1. 스킬 해금용 아이템 (예: "skill_double_jump", "skill_dash")
    public List<string> UnlockedSkillList { get; set; } = new List<string>();

    // 2. 단순 수집/도감용 보석 (예: "collection_gem_01")
    public List<string> CollectedItemList { get; set; } = new List<string>();


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



    // ---------------------------------------------------------
    // 🛠️ 외부(매니저, UI)에서 편하게 쓰기 위한 헬퍼(Helper) 함수들
    // ---------------------------------------------------------

    // 특정 스킬을 해금했는지 판별
    public bool HasSkill(string skillId)
    {
        return UnlockedSkillList.Contains(skillId);
    }

    // 스킬 해금 아이템 획득 시 호출
    public void UnlockSkill(string skillId)
    {
        if (!UnlockedSkillList.Contains(skillId))
        {
            UnlockedSkillList.Add(skillId);
            Debug.Log($"[스킬 해금] {skillId}");
        }
    }

    // 특정 수집품(보석)을 먹었는지 판별 (도감 UI에서 사용!)
    public bool HasItem(string gemId)
    {
        return CollectedItemList.Contains(gemId);
    }

    // 수집품 획득 시 호출
    public void CollectItem(string gemId)
    {
        if (!CollectedItemList.Contains(gemId))
        {
            CollectedItemList.Add(gemId);
            Debug.Log($"[수집품 획득] {gemId}");
        }
    }
}