using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class GameGUI : MonoBehaviour
{
    public static GameGUI instance;

    [Header("Player GUI")]
    [SerializeField] private TextMeshProUGUI ammoText;

    void Start()
    {
        instance = this;
    }

    public void UpdateAmmoText(int currentAmmo, int totalAmmo)
    {
        ammoText.text = currentAmmo + "/" + totalAmmo;
    }

}
