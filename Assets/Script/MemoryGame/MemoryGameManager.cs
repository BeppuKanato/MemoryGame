using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MemoryGameManager: MonoBehaviour 
{
    public delegate void CardActionDelegate();
    //カードマネージャー
    [SerializeField]
    CardManager cardManager;
    //プレイヤーマネージャー
    [SerializeField]
    PlayerManager playerManager;
    //ネットワーク関係のコントローラー
    [SerializeField]
    MemoryGameNetworkController networkController;
    //画面に配置するカードの最初の位置
    [SerializeField]
    Vector2 leftUpPos;
    //カード間のx軸の間隔
    [SerializeField]
    float xSpacing;
    //カード間のy軸の間隔
    [SerializeField]
    float ySpacing;
    //カードオブジェクトの親
    [SerializeField]
    GameObject cardField;

    //キャラクターのイラスト表示位置
    [SerializeField]
    List<Transform> illustTransforms;     //辞書型に変換するかは検討

    //列挙体で現在のプレイヤーを表す
    [SerializeField]
    PlayerEnum nowTurn;
    //トランプを選択できる状態かを表す
    [SerializeField]
    bool canSelect;
    //トランプの数字チェック関係の処理が終わったかを確認
    [SerializeField]
    bool canCheckPair;
    //現在ゲームの準備処理を行っているかを表す
    [SerializeField]
    bool isReady = false;

    //次ターンにプレイヤーを切り替えるか
    [SerializeField]
    bool switchPlayer = false;

    //受け取り処理を実行したか
    [SerializeField]
    bool isReceive = false;

    //この端末が操作可能なプレイヤーの種類
    [SerializeField]
    List<PlayerEnum >myPlayerEnum = new List<PlayerEnum>();
    private void Start()
    {
        nowTurn = PlayerEnum.Player_1;
        gameState = GameState.Ready;


        networkController.SetSocket();
    }
    GameState gameState;
    private void Update()
    {
        switch(gameState)
        {
            case GameState.Ready:
                if (!isReady)
                {
                    UpdateReady();
                }
                break;
            case GameState.ReadySelect:
                UpdateReadySelect();
                break;
            case GameState.Select:
                //今のターンが自分が操作不可能なターン時
                if (!myPlayerEnum.Contains(nowTurn))
                {
                    Debug.Log("相手操作です");
                    UpdateSelectWhenNotCanSelect();
                    break;
                }
                if (canSelect)
                {
                    UpdateSelectWhenCanSelect();
                }
                else
                {
                    Debug.Log("選択可能状態ではありません");
                }
                break;
            case GameState.CheckPair:
                if (canCheckPair)
                {
                    UpdateCheckPair();
                }
                break;
            case GameState.ReadyNextTurn:
                UpdateReadyNextTurn();
                break;
        }
    }
    //gameStateがReadyの時の処理
    void UpdateReady()
    {
        //Playerの初期設定を行う
        playerManager.CreatePlayerObject(illustTransforms);
        //カードの初期設定を行う
        StartCoroutine(cardManager.CreateCard(leftUpPos, xSpacing, ySpacing, cardField, CreateCardCallBack));
        isReady = true;
    }
    //gameStateがReadySelectの時の処理
    void UpdateReadySelect()
    {
        bool contain = myPlayerEnum.Contains(nowTurn);
        canSelect = contain;
        canCheckPair = contain;
        gameState = GameState.Select;
    }

    //自分の端末が操作可能なターンの時のselect時の処理
    void UpdateSelectWhenCanSelect()
    {
        int result = playerManager.ExecuteSelectCard(nowTurn);
        if (result != -1)
        {
            bool responce = cardManager.CheckSelected(result, ReverseSelectCardCallBack);
            //選択可能なカードだった時
            if (responce)
            {
                //キャラクターのアニメーションとボイス再生
                playerManager.PlayCharAction(nowTurn, CharacterAnimEnum.UpDown);

                //IDデータを送信
                networkController.SendData(nowTurn, result);
                canSelect = false;
            }
            else
            {
                gameState = GameState.ReadySelect;
            }
        }
        else
        {
            Debug.Log("nullが返されました");
        }
    }
    //自分の端末が操作不可能な時のselectの処理
    void UpdateSelectWhenNotCanSelect()
    {
        if (!isReceive)
        {
            networkController.ReceiveData();
            isReceive = true;
        }
        //相手から情報を受信した時
        if (networkController.GetOpponentData().card_id != -1)
        {
            cardManager.CheckSelected(networkController.GetOpponentData().card_id, ReverseSelectCardCallBack);
            //キャラクターのアニメーションとボイス再生
            playerManager.PlayCharAction(nowTurn, CharacterAnimEnum.UpDown);

            isReceive = false;
        }
    }
    //gameStateがCheckPairの時の処理
    void UpdateCheckPair()
    {
        //2枚カードを選択したか
        if (cardManager.GetNumberOfSelectedCards() == 2)
        {
            //二枚のカードは同じ数字か
            if (cardManager.CheckSameNumber())
            {
                gameState = GameState.ReadyNextTurn;
                playerManager.PlayCharAction(nowTurn, CharacterAnimEnum.Happy);
                switchPlayer = false;
                //canCheckPair = true;
            }
            else
            {
                playerManager.PlayCharAction(nowTurn, CharacterAnimEnum.Regret);
                cardManager.ReverseSelectedCards(ReverseSelectedCardCallBack);

                switchPlayer = true;
            }

            cardManager.ResetSelectedCards();
        }
        else
        {
            switchPlayer = false;
            gameState = GameState.ReadyNextTurn;
        }
        canCheckPair = false;
    }
    //gaemStateがReadyNextTurnの時の処理
    void UpdateReadyNextTurn()
    {
        //プレイヤーを交代する時
        if (switchPlayer)
        {
            int nowPlayerNumber = (int)nowTurn;
            PlayerEnum nextPlayer;
            Debug.Log("相手ターンに移ります");
            do
            {
                int nextNumber = (nowPlayerNumber + 1) % playerManager.GetNumberOfPlayer();
                nextPlayer = (PlayerEnum)Enum.ToObject(typeof(PlayerEnum), nextNumber);
            } while (!playerManager.GetIsConnect(nextPlayer));  //次のプレイヤーが通信可能状態か確認

            nowTurn = nextPlayer;
            Debug.Log($"次のプレイヤー{nextPlayer}");
        }
        else
        {
            Debug.Log("再び自分のターンです");
        }

        gameState = GameState.ReadySelect;
    }
    //カード作成メソッドのコールバック
    void CreateCardCallBack()
    {
        isReady = false;    
        gameState = GameState.ReadySelect;
    }
    //選択したカードのアニメーションのコールバック関数
    void ReverseSelectCardCallBack()
    {
        gameState = GameState.CheckPair;
    }
    //選択済みカードのアニメーションのコールバック関数
    void ReverseSelectedCardCallBack()
    {
        gameState = GameState.ReadyNextTurn;
    }
}
