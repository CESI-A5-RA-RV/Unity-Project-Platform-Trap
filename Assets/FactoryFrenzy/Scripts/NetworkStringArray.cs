using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using WebSocketSharp;

public class NetworkStringArray : INetworkSerializable
{
    public string[] Array;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter{
        var length = 0;
        if(!serializer.IsReader)
            length = Array.Length;

        serializer.SerializeValue(ref length);

        if(serializer.IsReader)
            Array = new string[length];
        
        for(var n = 0; n < length; ++n)
            serializer.SerializeValue(ref Array[n]);

        
    }
}
