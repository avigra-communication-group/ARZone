using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ARMapObjectController : MonoBehaviour
{
    public PlaceData placeData = null;

    void Start()
    {
            placeData = IconObjectControllerHelperUtils.operapblePlaceData;
            IconObjectControllerHelperUtils.operapblePlaceData = null;
    }

    void OnMouseDown() {
        if(RewardPanelManager.instance == null) 
        {
            Debug.LogError("Reward panel doesn't exist.");
            return;
        }
        
        transform
            .DOScale(transform.localScale * 1.5f, 0.5f)
            .OnComplete(() => {
                transform
                    .DOScale(transform.localScale * 0f, 0.2f)
                    .OnComplete(() => {
                    RewardPanelManager.instance.OpenPanel();
                });
            });
            
    }
}
