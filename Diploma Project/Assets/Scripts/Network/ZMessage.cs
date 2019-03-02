using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ZMessageType
{
    public static short STRING = 5000;
}

public class StringMessage : MessageBase
{
    public string stringValue;
}
