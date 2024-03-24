using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Singleton;
    public int money;
    public int currentHourlyIncome;
    public int targetMoney;
    [SerializeField] private float realSecondsPerHour = 15f;
    [SerializeField] private PlacementSystem placementSystem;
    [SerializeField] private UI ui;
    private Dictionary<int, int> buildingsIncome;
    private float _currentTime;
    
    private void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        buildingsIncome = new Dictionary<int, int>();
        placementSystem.OnObjectPlaced += AddToIncome;
        placementSystem.OnObjectRemoved += RemoveFromIncome;
        
        foreach (var objectData in placementSystem.ObjectsDatabaseSo.ObjectDataList)
        {
            buildingsIncome.Add(objectData.ID,objectData.HourlyIncome);
        }
        
        ui.UpdateTargetTMP();
        ui.UpdateIncome();
        ui.UpdateMoney();
    }

    private void Update()
    {
        _currentTime += Time.deltaTime;
        if (_currentTime > realSecondsPerHour)
        {
            _currentTime = 0;
            money += currentHourlyIncome;
            ui.UpdateMoney();
            if (money >= targetMoney)
            {
                LevelCompleted();
            }
        }
    }

    private void AddToIncome(int ID)
    {
        currentHourlyIncome += buildingsIncome[ID];
        ui.UpdateIncome();
    }    
    private void RemoveFromIncome(int ID)
    {
        currentHourlyIncome -= buildingsIncome[ID];
        ui.UpdateIncome();
    }
    
    public void LevelCompleted()
    {
        Debug.Log("levelCompleted");
        //show UI of level completed
        //show level stats: time, money, score
    }

    private void OnDestroy()
    {
        placementSystem.OnObjectPlaced -= AddToIncome;
        placementSystem.OnObjectRemoved -= RemoveFromIncome;
        buildingsIncome = null;
        placementSystem = null;
    }
}
