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
    //�l�b�g���[�N�G���[���̏����̃f���Q�[�g
    public delegate void NetEventCallBack(NetEventStatus status);
    //�Q�[���̏����̃f���Q�[�g
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
    //�V�����\�P�b�g���쐬���郁�\�b�h
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
            Debug.LogError($"�\�P�b�g�쐬���̃G���[{e}");
            Debug.Log("�ʐM����蒼���Ă�������");
        }
    }

    //��M���s���񓯊����\�b�h
    public async Task ReceiveAsync(int id, string callBackKey)
    {
        byte[] buffer = new byte[1024];
        Debug.Log("��M��ҋ@���܂�");
        int bytesRead = await networkStreams[id].ReadAsync(buffer, 0, buffer.Length);

        Debug.Log($"{id}����M���܂���");

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
    //���M�������s�����\�b�h
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
    //�w�肳�ꂽID�̃\�P�b�g���~���郁�\�b�h
    public void StopSocket(int id)
    {
        tcpSockets[id].Dispose();
    }
    //�\�P�b�g��ID�ꗗ��Ԃ����\�b�h

    public List<int> GetTcpSocketsKeys()
    {
        List<int> result = new List<int>();
        foreach(int key in tcpSockets.Keys)
        {
            result.Add(key);
        }

        return result;
    }
    //�l�b�g���[�N�Ɋւ��鏈���̃R�[���o�b�N��o�^���郁�\�b�h
    public void RegisterNetErrorEventHandler(string key, NetEventCallBack eventHandler)
    {
        netErrorEventHandler.Add(key, eventHandler);
    }
    //�l�b�g���[�N�Ɋւ��鏈���̃R�[���o�b�N���폜���郁�\�b�h
    public void UnregisterNetErrorEventHandler(string key)
    {
        netErrorEventHandler.Remove(key);
    }
    //�Q�[�������̃R�[���o�b�N��o�^���郁�\�b�h
    public void RegisterGameProcessEventHandler(string key, GameProcessEventCallBack eventHandler)
    {
        gameProcessEventHandler.Add(key, eventHandler);
    }
    //�Q�[�������̃R�[���o�b�N���폜���郁�\�b�h
    public void UnRegisterGameProcessEventHandler(string key)
    {
        gameProcessEventHandler.Remove(key);
    }
}
