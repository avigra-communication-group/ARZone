using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointItemButton : MonoBehaviour
{
    public Text pointDisplay;
    public Text rewardDisplay;
    public Button button;

    public void GenerateDisplayInfo(double point, string reward)
    {
        pointDisplay.text = point.ToString();
        rewardDisplay.text = reward;
    }
    
}
