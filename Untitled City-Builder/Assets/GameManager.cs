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
    [SerializeField] private UI ui;
    private Dictionary<int, int> buildingsIncome;
    private Dictionary<int, int> buildingsPrice;
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
        buildingsPrice = new Dictionary<int, int>();
        placementSystem.OnObjectPlaced += BuyBuilding;
        placementSystem.OnObjectRemoved += SellBuilding;
        money = startingMoney;
        foreach (var objectData in placementSystem.ObjectsDatabaseSo.ObjectDataList)
        {
            buildingsIncome.Add(objectData.ID,objectData.HourlyIncome);
            buildingsPrice.Add(objectData.ID,objectData.Price);
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
        return money >= buildingsPrice[ID];
    }
    private void BuyBuilding(int ID)
    {
        currentHourlyIncome += buildingsIncome[ID];
        money -= buildingsPrice[ID];
        
        ui.UpdateIncome();
        ui.UpdateMoney();
    }    
    private void SellBuilding(int ID)
    {
        currentHourlyIncome -= buildingsIncome[ID];
        money += buildingsPrice[ID];

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
        placementSystem.OnObjectPlaced -= BuyBuilding;
        placementSystem.OnObjectRemoved -= SellBuilding;
        buildingsIncome = null;
        placementSystem = null;
    }
}
