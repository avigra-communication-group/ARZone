﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RewardPanelManager : MonoBehaviour
{
    public static RewardPanelManager instance = null;

    public GameObject container;
    public Text message;

    private void Awake() 
    {
        if(instance == null)
        {
            instance = this;
        }    
    }

    public void OpenPanel()
    {
        container.SetActive(true);
    }

    public void ClosePanel()
    {
        container.SetActive(false);
        SceneManager.LoadScene("armap");
    }
}