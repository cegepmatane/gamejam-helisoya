using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ScriptsToDisable : NetworkBehaviour
{
    public List<Behaviour> toDisable;

    void Start()
    {
        if (!isLocalPlayer)
        {
            for (int i = 0; i < toDisable.Count; i++)
            {
                toDisable[i].enabled = false;
            }
        }
    }
}
