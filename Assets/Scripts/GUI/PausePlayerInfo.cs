using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PausePlayerInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerName;


    public void Init(HunterMovement player)
    {
        playerName.text = player.playerName;
    }
}
