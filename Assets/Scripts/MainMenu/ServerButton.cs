using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror.Discovery;
using TMPro;

public class ServerButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private ServerResponse response;
    private ServerList parent;

    public void Init(ServerResponse info, ServerList list)
    {
        text.text = info.EndPoint.Address.ToString();
        response = info;
        parent = list;
    }

    public void OnClick()
    {
        parent.StartClient(response);
    }
}
