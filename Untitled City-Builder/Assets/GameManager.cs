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
    [SerializeField] private int startingMoney = 15;
    [SerializeField] private float realSecondsPerHour = 15f;
    [SerializeField] private PlacementSystem placementSystem;
    [SerializeField] private ObjectPlacer objectPlacer;
    [SerializeField] private UI ui;
    private Dictionary<int, int> buildingsIncome;
    private Dictionary<int, int> buildingsBuyPrice;
    private Dictionary<int, int> buildingsSellPrice;
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
        buildingsBuyPrice = new Dictionary<int, int>();
        buildingsSellPrice = new Dictionary<int, int>();
        objectPlacer.onObjectPlaced += PlaceBuildingHandler;
        objectPlacer.onObjectRemoved += RemoveBuildingHandler;
        money = startingMoney;
        foreach (var objectData in placementSystem.ObjectsDatabaseSo.ObjectDataList)
        {
            buildingsIncome.Add(objectData.ID,objectData.HourlyIncome);
            buildingsBuyPrice.Add(objectData.ID,objectData.BuyPrice);
            buildingsSellPrice.Add(objectData.ID,objectData.SellPrice);
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

    public bool CanBuy(int ID)
    {
        return money >= buildingsBuyPrice[ID];
    }
    private void PlaceBuildingHandler(int ID, PlacingType placingType)
    {
        switch (placingType)
        {
            case PlacingType.Buy:
                currentHourlyIncome += buildingsIncome[ID];
                money -= buildingsBuyPrice[ID];
                break;
            case PlacingType.BuyFuse:
                money -= buildingsBuyPrice[ID];
                break;
            case PlacingType.Fuse:
                currentHourlyIncome += buildingsIncome[ID];
                break;
        }
        ui.UpdateIncome();
        ui.UpdateMoney();
    }    
    private void RemoveBuildingHandler(int ID, PlacingType placingType)
    {
        switch (placingType)
        {
            case PlacingType.Sell:
                currentHourlyIncome -= buildingsIncome[ID];
                money += buildingsSellPrice[ID];
                break;
            case PlacingType.Fuse:
                currentHourlyIncome -= buildingsIncome[ID];
                break;
        }
        ui.UpdateIncome();
        ui.UpdateMoney();
    }
    
    public void LevelCompleted()
    {
        ui.ShowVictoryScreen();
        //show level stats: time, money, score
    }

    private void OnDestroy()
    {
        objectPlacer.onObjectPlaced -= PlaceBuildingHandler;
        objectPlacer.onObjectRemoved -= RemoveBuildingHandler;
        buildingsIncome = null;
        placementSystem = null;
    }
}

public enum PlacingType
{
    Buy,
    BuyFuse,
    Sell,
    Fuse
}
