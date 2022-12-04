using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.Discovery;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using UnityEngine.UI;
using TMPro;

public class ServerList : MonoBehaviour
{
    Dictionary<long, ServerResponse> discoveredServers = new Dictionary<long, ServerResponse>();
    public NetworkManager networkManager;
    public NetworkDiscovery networkDiscovery;

    [SerializeField] private Transform serverParent;
    [SerializeField] private GameObject prefabServer;

    [Header("Settings")]
    [SerializeField] private GameObject normalTab;
    [SerializeField] private GameObject settingsTab;
    [SerializeField] private TMP_InputField ipInput;
    [SerializeField] private TMP_InputField playerNameInput;

    public void Quit()
    {
        Application.Quit();
    }

    void Start()
    {


        if (networkDiscovery.BroadcastAddress == "" || networkDiscovery.BroadcastAddress == null)
        {
            string[] ipSplit = GetLocalIPv4().Split(".");
            string ipCorrect = ipSplit[0] + "." + ipSplit[1] + "." + ipSplit[2] + ".255";
            networkDiscovery.BroadcastAddress = ipCorrect;
        }
        ipInput.SetTextWithoutNotify(networkDiscovery.BroadcastAddress);

        if (!PlayerPrefs.HasKey("playerName"))
        {
            PlayerPrefs.SetString("playerName", "Hunter");
        }
        playerNameInput.SetTextWithoutNotify(PlayerPrefs.GetString("playerName"));

        SearchServers();
    }


    public void SetSettingsOpen(bool value)
    {
        normalTab.SetActive(!value);
        settingsTab.SetActive(value);
    }

    public void AcceptSettingsChanges()
    {
        PlayerPrefs.SetString("playerName", playerNameInput.text);
        networkDiscovery.BroadcastAddress = ipInput.text;
    }


    public string GetLocalIPv4()
    {
        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }
    public void SearchServers()
    {
        discoveredServers.Clear();
        foreach (Transform t in serverParent.transform)
        {
            Destroy(t.gameObject);
        }
        networkDiscovery.StopDiscovery();
        networkDiscovery.StartDiscovery();
    }

    public void StartHost()
    {
        discoveredServers.Clear();
        NetworkManager.singleton.StartHost();
        networkDiscovery.AdvertiseServer();
    }

    public void StartClient(ServerResponse info)
    {
        networkDiscovery.StopDiscovery();
        NetworkManager.singleton.StartClient(info.uri);
    }

    public void OnDiscoveredServer(ServerResponse info)
    {
        if (discoveredServers.ContainsKey(info.serverId)) return;

        discoveredServers[info.serverId] = info;
        print("Serveur : " + info.serverId + " - " + info.uri);
        GameObject button = Instantiate(prefabServer, serverParent);
        button.GetComponent<ServerButton>().Init(info, this);
        RectTransform rect = serverParent.GetComponent<RectTransform>();

        rect.sizeDelta = new Vector2(rect.sizeDelta.x, 30 * discoveredServers.Keys.Count);
    }
}
