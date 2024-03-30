using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI incomeText;
    [SerializeField] private TextMeshProUGUI targetTMP;
    [SerializeField] private GameObject VictoryScreen;

    private void Awake()
    {
        VictoryScreen.SetActive(false);
    }

    public void UpdateMoney()
    {
        moneyText.text = GameManager.Singleton.money.ToString("C0");
    }    
    public void UpdateIncome()
    {
        incomeText.text = GameManager.Singleton.currentHourlyIncome.ToString("C0") + "/h";
    }

    public void UpdateTargetTMP()
    {
        targetTMP.text = "Target: " + GameManager.Singleton.targetMoney.ToString("C0");
    }

    public void ShowVictoryScreen()
    {
        
        //victory screen stuff
        VictoryScreen.SetActive(true);
    }
}
