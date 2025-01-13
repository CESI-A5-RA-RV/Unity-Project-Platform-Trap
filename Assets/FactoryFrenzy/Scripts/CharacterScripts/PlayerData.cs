using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerData : NetworkBehaviour
{
    public NetworkVariable<string> username = new NetworkVariable<string>();
    public NetworkVariable<int> playerID = new NetworkVariable<int>();
}
