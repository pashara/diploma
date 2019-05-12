using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class Player : CustomNetworkBehaviour
{
    
    public void UpdateAllDataFromClient()
    {
        CmdSetIdOnClients(PlayerID);
        CmdSetPointsOnClients(PlayerPoints);
    }

    public void UpdateColorDataFromServer()
    {
        //CmdSetColorIdOnClients(ColorIndex);
        RpcSetColorIdOnClients(ColorIndex);
    }


    [ClientRpc]
    void RpcUpdateTimer(int seconds)
    {
        if (!isServer)
        {
            timer = seconds;
            OnTimerChanged?.Invoke(seconds);
        }
    }

    void OnPlayerPointsChanged(int newValue)
    {
        CmdSetPointsOnClients(newValue);
    }


    [Command]
    private void CmdSetIdOnClients(int idValue)
    {
        RpcSetIdOnClients(idValue);
    }

    [Command]
    private void CmdSetPointsOnClients(int points)
    {
        RpcSetPointsOnClients(points);
    }

    [Command]
    private void CmdSetColorIdOnClients(int colorId)
    {
        //ColorIndex = colorId;
        RpcSetColorIdOnClients(colorId);
    }


    [Command]
    private void CmdOnPlayerRestarted(int killer)
    {
        LastKilledByPlayerId = killer;
        RpcOnPlayerRespawned(killer);
    }

    
    [ClientRpc]
    private void RpcSetIdOnClients(int idValue)
    {
        if (!IsLocalPlayer)
        {
            PlayerID = idValue;
            OnIdSeted?.Invoke(this, PlayerID);
        }
    }


    [ClientRpc]
    private void RpcSetPointsOnClients(int points)
    {
        if (!IsLocalPlayer)
        {
            PlayerPoints = points;
        }
    }

    

    [ClientRpc]
    private void RpcSetColorIdOnClients(int colorId)
    {
        //if (!IsLocalPlayer)
        //{
            ColorIndex = colorId;
        //}
    }


    [ClientRpc]
    private void RpcOnPlayerRespawned(int killer)
    {
        if (!IsLocalPlayer)
        {
            LastKilledByPlayerId = killer;
            Respawn(true);
        }
    }

}