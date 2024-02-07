using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MemoryGameNetworkController : MonoBehaviour
{
    public bool IS_DEBUG;
    [SerializeField]
    TcpLibrary tcpLibrary;
    [SerializeField]
    MemoryGameDataStruct opponentData;
    [SerializeField]
    string keyName;
    [SerializeField]
    List<int> socketIds = new List<int>();
    // Start is called before the first frame update
    void Start()
    {
        opponentData = new MemoryGameDataStruct(PlayerEnum.Player_1, -1);
    }

    //ソケットを作成し、通信の準備を行うメソッド
    public void SetSocket()
    {
        if (!IS_DEBUG)
        {
            tcpLibrary.SetSocket("127.0.0.1", 8000);
        }
    }

    //受信時のコールバック
    void ReceiveCallBack(string jsonData)
    {
        MemoryGameDataStruct data = JsonUtility.FromJson<MemoryGameDataStruct>(jsonData);
        opponentData = data;

        Debug.Log($"相手の選択 = {data.card_id}");
    }
    //MemoryGameDataStructの情報を相手に送信するメソッド
    public void SendData(PlayerEnum playerEnum, int cardID)
    {
        if (IS_DEBUG)
        {
            MemoryGameDataStruct json = new MemoryGameDataStruct(playerEnum, cardID);
            Debug.Log($"card_id = {json.card_id}を送信します");
        }
        else
        {
            MemoryGameDataStruct json = new MemoryGameDataStruct(playerEnum, cardID);
            string jsonData = JsonUtility.ToJson(json);

            tcpLibrary.Send(0, jsonData);
        }
    }
    public void ReceiveData()
    {
        if (IS_DEBUG)
        {
            MemoryGameDataStruct data = new MemoryGameDataStruct();
            string jsonData = JsonUtility.ToJson(data);

            ReceiveCallBack(jsonData);
        }
        else
        {
            tcpLibrary.ReceiveAsync(0, keyName);
        }
    }

    public MemoryGameDataStruct GetOpponentData()
    {
        return opponentData;
    }
}
