using System;
using System.Collections.Generic;


[System.Serializable]
public class GameDataBase
{
    public string Id;
}

// C# 때와 약간 달라진 점
// Syste.Text.Json대신 유니티 내장 JsonUtility를 사용
// 따라서 프로퍼티말고 그냥 일반 public 멤버변수로 변경함
// [System.Serializable]가 없다면 JsonUtility는 데이터를 무시


[System.Serializable]
public class CharacterData : GameDataBase
{
    public string Name;
}


[System.Serializable]
public class EnemyData : GameDataBase
{
    public string Name;
    public string PrefabPath;
}


[System.Serializable]
public class StageData : GameDataBase
{
    public string Name;
    public string NextStageId;    // 클리어 시 넘어갈 다음 스테이지의 ID
    public string BgmPath;        // 해당 맵에서 재생할 BGM 경로
    public string PrefabPath;
}


//    // =========================================================
//    // 아이템, 오브젝트
//    // =========================================================
[System.Serializable]
public class ItemData : GameDataBase
{
    public string Name;
    public string Description;
    public string FieldObjType;
    public int EffectValue;
    public string IconPath;
    public string PrefabPath;
}

[System.Serializable]
public class TrapData : GameDataBase
{
    public string Name;        // 함정 이름
    public int Damage;         // 함정 데미지
    public string Type;
    public string IconPath;
    public string PrefabPath;
}


//    // =========================================================
//    // 다이얼로그
//    // =========================================================

[System.Serializable]
public class DialogueGroupData : GameDataBase
{
    public List<string> DialogueIdList;
}

[System.Serializable]
public class DialogueData : GameDataBase
{
    public string CharacterDataId;
    public string Description;
    public string NextDialogueId;
    public List<string> SelectionNameList;
    public List<string> SelectionDialogueIdList;
    public string TexturePath;
    public string VoicePath;
}