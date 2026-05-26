using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class GameObjManager : MonoBehaviour
{
    // 생성할 몬스터의 프리팹
    [SerializeField] private GameObject Prefab_Enemy;
    [SerializeField] private Transform Root_Enemy;

    public static GameObjManager Inst { get; set; }

    // 생성된 오브젝트의 키가 됨
    private int _objInstanceKeyGenerator = 0;

    // 생성된 오브젝트의 생명을 보관
    private Dictionary<int, GameObject> _createdGameObjContainer = new Dictionary<int, GameObject>();
    private Dictionary<int, ItemBase> _itemContainer = new Dictionary<int, ItemBase>();
    private Dictionary<int, TrapBase> _trapContainer = new Dictionary<int, TrapBase>();

    private void Awake()
    {
        Inst = this;
    }

    public void RequestSpawnEnemy()
    {
        if (Prefab_Enemy == null)
        {
            Debug.LogWarning("프리팹이 등록되지 않은 오브젝트 입니다.");
            return;
        }

        var gObj = Instantiate(Prefab_Enemy, Root_Enemy);
        if (gObj == null)
        {
            Debug.LogWarning("생성에 실패한 게임 오브젝트 입니다.");
            return;
        }

        // 1-1 생성에 성공했다면, 미리 Key를 발급한다.
        _objInstanceKeyGenerator++;
        int generatedInstanceId = _objInstanceKeyGenerator;

        // 2. [수정됨] DaniTech_2DEnemy 대신 우리가 만든 최상위 부모인 EnemyBase를 찾습니다.
        // 이렇게 하면 Enemy_Moving이든 Enemy_Turret이든 모두 EnemyBase를 상속받았기 때문에 정상적으로 찾아집니다! (다형성)
        var enemyComp = gObj.GetComponent<EnemyBase>();
        if (enemyComp != null)
        {
            enemyComp.InitEnemyInfo(generatedInstanceId);
        }

        // 1-2 Dictionary에 추가하기 전에 미리 키 검사한다
        if (_createdGameObjContainer.ContainsKey(_objInstanceKeyGenerator) == true)
        {
            Debug.LogWarning("이미 동일한 키가 발급된 게임 오브젝트가 존재합니다");
            return;
        }

        // 1-3 동적생성(실체화)된 오브젝트를 게임 오브젝트 매니저의 자료구조(Dictionary)에 보관하자!
        _createdGameObjContainer.Add(_objInstanceKeyGenerator, gObj);
        InitGeneratedEntityObj(_objInstanceKeyGenerator, gObj);

        Debug.Log($"키: {_objInstanceKeyGenerator}의 객체 {gObj.name}이 호출되었습니다.");
    }

    private void InitGeneratedEntityObj(int generatedId, GameObject gObj)
    {
        // 4-1 지금은 Enemy지만, 나중에 IGameEntity 같은 인터페이스로 개선하면 더 좋다
        EnemyBase gameEntity = gObj.GetComponent<EnemyBase>();
        if (gameEntity == null)
        {
            Debug.LogWarning($"생성된 {gObj.name}의 InstanceId를 대입할 수 있는 컴포넌트를 가져올 수 없습니다!");
            return;
        }

        // 4-2 생성된 객체에 정보를 부여한다!
        gameEntity.InitEnemyInfo(generatedId);
    }


    public GameObject GetEntityObjCanBeNull(int instanceId)
    {
        if (_createdGameObjContainer.ContainsKey(instanceId) == false)
        {
            Debug.LogWarning($"{instanceId}는 존재하지 않습니다.");
            return null;
        }

        // 2-1 실체화하면서 등록된 게임 오브젝트가 있다면 반환
        return _createdGameObjContainer[instanceId];
    }

    public void RequestDestroyEntityObj(int instanceId)
    {
        var gObj = GetEntityObjCanBeNull(instanceId);
        if (gObj == null)
        {
            return;
        }

        // 3-1 요청된 객체를 제거함
        _createdGameObjContainer.Remove(instanceId);
        Destroy(gObj);
    }






    //[필드 오브젝트] ====================================================================================================

    public async UniTaskVoid CreateFieldObj(string dataId, Transform spawnSpot)
    {
        string targetPrefabPath = "";

        var itemData = GameDataManager.Instance.GetItemData(dataId);
        if (itemData != null)
        {
            targetPrefabPath = itemData.PrefabPath;
        }

        else
        {
            var trapData = GameDataManager.Instance.GetTrapData(dataId);
            if (trapData != null)
            {
                targetPrefabPath = trapData.PrefabPath;
            }
        }

        if (string.IsNullOrEmpty(targetPrefabPath))
        {
            Debug.LogError($"[GameObjManager] {dataId}에 해당하는 프리팹 경로를 데이터에서 찾을 수 없습니다!");
            return;
        }

        var createdObj = await ResourceManager.Inst.InstantiateAsync(targetPrefabPath, Root_Enemy, true);
        // =========================================================================
        // 🚨 [핵심 방어 코드] 기다리는 동안 맵이 바뀌어서 스폰 스팟이 파괴되었다면?
        // =========================================================================
        if (spawnSpot == null)
        {
            Debug.LogWarning($"[GameObjManager] 맵이 전환되어 {dataId} 생성을 취소합니다.");

            // 이미 생성되어 버린 오브젝트가 좀비가 되지 않게 즉시 파괴
            if (createdObj != null)
            {
                Destroy(createdObj);
            }

            return; // 아래의 위치 이동 및 리스트 등록 로직을 실행하지 않고 즉시 종료!
        }
        // =========================================================================
        createdObj.transform.position = spawnSpot.position;

        AddFieldObjOnCreate(createdObj, dataId);
    }

    private void AddFieldObjOnCreate(GameObject createdObj, string dataId)
    {
        _objInstanceKeyGenerator++;
        var generatedInstanceId = _objInstanceKeyGenerator;

        var itemBase = createdObj.GetComponent<ItemBase>();
        if (itemBase != null)
        {
            _itemContainer.Add(generatedInstanceId, itemBase);
            itemBase.InitItemInfoOnCreated(generatedInstanceId, dataId);
            return;
        }

        var trapBase = createdObj.GetComponent<TrapBase>();
        if (trapBase != null)
        {
            _trapContainer.Add(generatedInstanceId, trapBase);
            trapBase.InitTrapInfoOnCreated(generatedInstanceId, dataId);
            return;
        }

        // ====================================================================
        // 3. [핵심] 아이템도 아니고 함정도 아니라면? -> 여기서 고스트가 발생했음!
        // ====================================================================
        Debug.LogWarning($"[GameObjManager] 경고! '{createdObj.name}'(ID: {dataId}) 프리팹에 ItemBase나 TrapBase 스크립트가 안 붙어있습니다! 일단 '기타 명부'에 강제로 등록합니다.");

        // 정체불명이지만, 맵 넘어갈 때 안 지워지는 좀비가 되면 안 되니까 일반 명부에라도 강제로 기록해둠.
        _createdGameObjContainer.Add(generatedInstanceId, createdObj);
    }

    public void RequestDestroyFieldObj(int instanceId)
    {
        var fieldObjComponent = GetItemByInstanceId(instanceId);
        if (fieldObjComponent == null)
        {
            return;
        }

        // 요청된 필드 오브젝트를 제거함
        _itemContainer.Remove(instanceId);
        Destroy(fieldObjComponent.gameObject);
    }

    public ItemBase GetItemByInstanceId(int fieldObjInstanceId)
    {
        if (_itemContainer.ContainsKey(fieldObjInstanceId) == false)
        {
            Debug.LogError($"{fieldObjInstanceId} 찾으려는 필드 오브젝트가 유효하지 않습니다");
            return null;
        }

        return _itemContainer[fieldObjInstanceId];
    }


    public void ClearAllFieldObjs()
    {
        // _itemContainer와 _trapContainer 등에 있는 모든 오브젝트를 파괴합니다.
        // 리스트를 순회하며 Destroy(obj)를 호출하고 리스트를 Clear() 하면 됩니다.

        foreach (var item in _itemContainer.Values)
        {
            if (item != null) Destroy(item.gameObject);
        }
        _itemContainer.Clear();

        foreach (var trap in _trapContainer.Values)
        {
            if (trap != null) Destroy(trap.gameObject);
        }
        _trapContainer.Clear();

        foreach (var obj in _createdGameObjContainer.Values)
        {
            if (obj != null) Destroy(obj);
        }
        _createdGameObjContainer.Clear();

        Debug.Log("모든 필드 오브젝트 및 몬스터가 정리되었습니다.");
    }
}
