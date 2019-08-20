using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PointShopManager : MonoBehaviour
{
    public static string PIN;
    public RewardList rewardList;
    public ExchangeWindiowController exchangeWindiowController;
    public PointItemButton pointItemButtonPrefab;
    public Transform pointItemButtonContainer;

    private void Start() 
    {
        PIN = rewardList.PIN;

        foreach(Reward reward in rewardList.rewards)
        {
            PointItemButton p = Instantiate(pointItemButtonPrefab);
            p.GenerateDisplayInfo(reward.point, reward.reward);
            p.transform.SetParent(pointItemButtonContainer);
            p.transform.localScale = Vector3.one;
            p.button.onClick.AddListener(() => {
                exchangeWindiowController.OpenExchangeWindow(reward.point, reward.reward);
            });
        }    
    }

    
}
