using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class ObjectsDatabaseSO : ScriptableObject
{
    public List<ObjectData> ObjectDataList;
}

[Serializable]
public class FusionData
{
    [field: SerializeField] public int CompatibleID { get; private set; }
    [field: SerializeField] public int RequiredAmount { get; private set; }
    [field: SerializeField] public GameObject FusionResult { get; private set; }
    [field: SerializeField] public int ResultId { get; private set; }
}
[Serializable]
public class ObjectData
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public int ID { get; private set; }
    
    [field: SerializeField] public FusionData FusionData { get; private set; }
    [field: SerializeField] public Vector2Int Size { get; private set; } = Vector2Int.one;
    [field: SerializeField] public GameObject Prefab { get; private set; }
    [field: SerializeField] public int Price { get; private set; }
    [field: SerializeField] public int HourlyIncome { get; private set; }
}