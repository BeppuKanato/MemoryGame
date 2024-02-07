using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetEventStatus
{
    public NetEventStatus(NetEventType type, NetEventResult result)
    {
        this.type = type;
        this.result = result;
    }
    public NetEventType type;
    public NetEventResult result;}

public enum NetEventType
{
    Connect = 0,
    Disconnect = 1,
    SendError = 2,
    ReceiveErrpr = 3,
}

public enum NetEventResult 
{
    Failure = -1,
    Success = 0,
}
