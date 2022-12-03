using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using Mirror.Discovery;

public class GameGUI : NetworkBehaviour
{
    public static GameGUI instance;

    [Header("Player GUI")]
    [SerializeField] private TextMeshProUGUI ammoText;

    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseMenu;

    public bool paused { get { return pauseMenu.activeInHierarchy; } }

    void Start()
    {
        instance = this;
    }

    public void UpdateAmmoText(int currentAmmo, int totalAmmo)
    {
        ammoText.text = currentAmmo + "/" + totalAmmo;
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseMenu.SetActive(!pauseMenu.activeInHierarchy);
        }
    }


    public void CloseGame()
    {
        if (isServer)
        {
            NetworkManager.singleton.StopHost();
        }
        else if (isClient)
        {
            NetworkManager.singleton.StopClient();
        }
    }

}
