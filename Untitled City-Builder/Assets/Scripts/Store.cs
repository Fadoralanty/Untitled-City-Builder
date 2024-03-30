using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Store : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private TextMeshProUGUI buttonTMP;
    private bool isStoreOpen;

    private void Awake()
    {
        isStoreOpen = false;
    }

    public void ToggleButton()
    {
        isStoreOpen = !isStoreOpen;
        if (isStoreOpen)
        {
            _animator.Play("ShowStore");
            buttonTMP.text = ">";
        }
        else
        {
            _animator.Play("HIdeStore");
            buttonTMP.text = "<";
        }
    }
}
