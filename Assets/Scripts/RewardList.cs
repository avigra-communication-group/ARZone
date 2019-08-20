using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="RewardList", menuName="Reward List", order=52)]
public class RewardList : ScriptableObject
{
    public string PIN;
    public List<Reward> rewards;
}
