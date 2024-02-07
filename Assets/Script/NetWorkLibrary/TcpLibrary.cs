using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class TcpLibrary : MonoBehaviour
{
    //ネットワークエラー時の処理のデリゲート
    public delegate void NetEventCallBack(NetEventStatus status);
    //ゲームの処理のデリゲート
    public delegate void GameProcessEventCallBack(string jsonData);

    Dictionary<int, TcpClient> tcpSockets;
    Dictionary<int, NetworkStream> networkStreams;

    int socketSerialID;

    Dictionary<string, NetEventCallBack> netErrorEventHandler; 
    Dictionary<string, GameProcessEventCallBack> gameProcessEventHandler;
    // Start is called before the first frame update
    void Awake()
    {
        socketSerialID = 0;
        tcpSockets = new Dictionary<int, TcpClient>();
        networkStreams = new Dictionary<int, NetworkStream>();
    }
    //新しくソケットを作成するメソッド
    public void SetSocket(string host, int port)
    {
        try
        {
            tcpSockets.Add(socketSerialID, new TcpClient(host, port));
            NetworkStream networkStream = tcpSockets[socketSerialID].GetStream();
            networkStreams.Add(socketSerialID, networkStream);

            socketSerialID++;
        }
        catch (SocketException e)
        {
            Debug.LogError($"ソケット作成時のエラー{e}");
            Debug.Log("通信をやり直してください");
        }
    }

    //受信を行う非同期メソッド
    public async Task ReceiveAsync(int id, string callBackKey)
    {
        byte[] buffer = new byte[1024];
        Debug.Log("受信を待機します");
        int bytesRead = await networkStreams[id].ReadAsync(buffer, 0, buffer.Length);

        Debug.Log($"{id}が受信しました");

        if (bytesRead > 0)
        {
            string data = System.Text.Encoding.ASCII.GetString(buffer, 0, bytesRead);
            gameProcessEventHandler[callBackKey](data);
        }
        else
        {
            NetEventStatus status = new NetEventStatus(NetEventType.Disconnect, NetEventResult.Failure);
            netErrorEventHandler[callBackKey](status);
        }
    }
    //送信処理を行うメソッド
    public void Send(int id, string jsonData)
    {
        try
        {
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(jsonData);
            networkStreams[id].Write(buffer, 0, buffer.Length);
        }
        catch (SocketException e) 
        {
            Debug.LogError(e);
        }
    }
    //指定されたIDのソケットを停止するメソッド
    public void StopSocket(int id)
    {
        tcpSockets[id].Dispose();
    }
    //ソケットのID一覧を返すメソッド

    public List<int> GetTcpSocketsKeys()
    {
        List<int> result = new List<int>();
        foreach(int key in tcpSockets.Keys)
        {
            result.Add(key);
        }

        return result;
    }
    //ネットワークに関する処理のコールバックを登録するメソッド
    public void RegisterNetErrorEventHandler(string key, NetEventCallBack eventHandler)
    {
        netErrorEventHandler.Add(key, eventHandler);
    }
    //ネットワークに関する処理のコールバックを削除するメソッド
    public void UnregisterNetErrorEventHandler(string key)
    {
        netErrorEventHandler.Remove(key);
    }
    //ゲーム処理のコールバックを登録するメソッド
    public void RegisterGameProcessEventHandler(string key, GameProcessEventCallBack eventHandler)
    {
        gameProcessEventHandler.Add(key, eventHandler);
    }
    //ゲーム処理のコールバックを削除するメソッド
    public void UnRegisterGameProcessEventHandler(string key)
    {
        gameProcessEventHandler.Remove(key);
    }
}
