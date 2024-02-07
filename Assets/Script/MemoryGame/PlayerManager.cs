using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    int NumberOfPlayer;
    //プレイヤーオブジェクトのプレハブ
    [SerializeField]
    GameObject playerPrefab;
    //プレイヤーを保持
    Dictionary<PlayerEnum, Player> players = new Dictionary<PlayerEnum, Player>();
    //プレイヤーの通し番号
    int uniquePlayerID = 0;

    //キャラクターの設定オブジェクトリスト
    [SerializeField]
    List<Character> characterList;
    //キャラクターの設定オブジェクト辞書型配列
    Dictionary<CharacterEnum, Character> characters = new Dictionary<CharacterEnum, Character>();

    public bool test;
    private void Update()
    {
        if (test)
        {
            StartCoroutine(players[PlayerEnum.Player_1].UpDownAnim());
            test = false;
        }
    }

    private void Start()
    {
        InitializeCharacters();
        CreateDictionary();
    }
    //指定されたプレイヤーのカードの選択メソッドを呼び出す
    public int ExecuteSelectCard(PlayerEnum nowTurn)
    {
        int result = players[nowTurn].SelectCard();

        return result;
    }
    //プレイヤーオブジェクトの生成を行うメソッド
    public void CreatePlayerObject(List<Transform> illustPoss)
    {
        for (int i = 0; i < NumberOfPlayer; i++)
        {
            GameObject instance = Instantiate(playerPrefab, illustPoss[i].position, illustPoss[i].rotation);
            instance.transform.localScale = illustPoss[i].localScale;
            Player player = instance.AddComponent<Player>();
            PlayerEnum playerEnum = (PlayerEnum)Enum.ToObject(typeof(PlayerEnum), uniquePlayerID);
            //プレイヤーのキャラクターを設定
            player.SetCharacter(characters[CharacterEnum.UnityChan]);
            player.InitializeSpriteRenderer();
            player.InitializeAudioSouce();
            if (i == 0)
            {
                player.cpu = false;
            }
            else
            {
                player.cpu = false;
            }
            players.Add(playerEnum, player);
            uniquePlayerID++;
        }
    }

    //Listを元にキャラクター設定の辞書型配列を作成するメソッド
    public void CreateDictionary()
    {
        for (int i = 0; i < characterList.Count; i++)
        {
            CharacterEnum characterEnum = (CharacterEnum)Enum.ToObject(typeof(CharacterEnum), i);
            characters.Add(characterEnum, characterList[i]);
        }
    }

    //キャラクタークラスの初期設定を行うメソッド
    void InitializeCharacters()
    {
        foreach (Character characer in characterList)
        {
            characer.CreateIllustDictionary();
            characer.CreateVoiceDictionary();
        }
    }
    //キャラクターのアニメーションとボイスの再生を行うメソッド
    public void PlayCharAction(PlayerEnum playerEnum, CharacterAnimEnum animEnum)
    {
        switch (animEnum)
        {
            case CharacterAnimEnum.UpDown:
                //アニメーション再生
                StartCoroutine(players[playerEnum].UpDownAnim());
                //ボイス再生
                players[playerEnum].PlayVoice(CharacterVoiceEnum.Select);
                break;
            case CharacterAnimEnum.Happy:
                players[playerEnum].SetSprite(CharacterExpressionEnum.Happy);
                StartCoroutine(players[playerEnum].UpDownAnim());
                players[playerEnum].PlayVoice(CharacterVoiceEnum.IsPair);
                break;
            case CharacterAnimEnum.Regret:
                players[playerEnum].SetSprite(CharacterExpressionEnum.Regret);
                StartCoroutine(players[playerEnum].UpDownAnim());
                players[playerEnum].PlayVoice(CharacterVoiceEnum.Miss);
                break;
        }
    }

    //プレイヤーの数を返すメソッド
    public int GetNumberOfPlayer()
    {
        return players.Count;
    }
    //指定されたプレイヤーの通信状態を返すメソッド
    public bool GetIsConnect(PlayerEnum playerEnum)
    {
        return players[playerEnum].isConnect;
    }
}
