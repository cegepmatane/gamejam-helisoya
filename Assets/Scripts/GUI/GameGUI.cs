using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    public bool paused { get { return pauseMenu.activeInHierarchy || endRoot.activeInHierarchy; } }

    [Header("BigFoot")]
    public HealthBarBF bfHealth;

    [Header("End Screen")]
    [SerializeField] private GameObject endRoot;
    [SerializeField] private Image missedFill;
    [SerializeField] private Image goodFill;
    [SerializeField] private Image friendlyFill;
    [SerializeField] private TextMeshProUGUI rankText;

    void Awake()
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


    public void ShowEndScreen()
    {
        endRoot.SetActive(true);
        int badShot = HunterMovement.localPlayer.badShot;
        int goodShot = HunterMovement.localPlayer.goodShot;
        int friendlyShot = HunterMovement.localPlayer.friendlyShot;
        float total = badShot + goodShot + friendlyShot;

        missedFill.fillAmount = badShot / total;
        goodFill.fillAmount = goodShot / total;
        friendlyFill.fillAmount = friendlyShot / total;

        rankText.text = "Rank : " + GetRank(badShot, goodShot, friendlyShot);
    }


    public string GetRank(int missed, int good, int friendly)
    {
        if (missed == 0 && good == 0 && friendly == 0) return "Sleepy";

        if (missed == good && good == friendly) return "Centrist";

        if (missed > good && missed > friendly)
        {
            if (missed >= 20)
            {
                return "Unlucky Luke";
            }

            return "One-Eyed";
        }

        if (good > missed && good > friendly)
        {
            if (good >= 20)
            {
                return "Bigfoot Hunter";
            }

            return "Good shot";
        }

        if (friendly > missed && friendly > good)
        {
            if (friendly >= 20)
            {
                return "Bigfoot";
            }

            return "Traitor";
        }

        if (friendly == missed && friendly > good)
        {
            return "Bad Assassin";
        }

        if (friendly == good && friendly > missed)
        {
            return "Mamal Hunter";
        }

        if (missed == good && good > friendly)
        {
            return "Lucky Bastard";
        }

        return "Fouzi";
    }

}
