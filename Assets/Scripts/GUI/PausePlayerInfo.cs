using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PausePlayerInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerName;

    [SerializeField] private Image goodFill;


    public void Init(HunterMovement player)
    {
        playerName.text = player.playerName;
        float tot = player.badShot + player.goodShot + player.friendlyShot;
        goodFill.fillAmount = (float)(player.goodShot) / tot;
    }
}
