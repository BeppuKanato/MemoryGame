using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ゲームの進行管理列挙体
public enum GameState
{
    None,
    Ready,
    ReadySelect,
    Select,
    CheckPair,
    ReadyNextTurn,
    Result,
    EndGame,
}
//プレイヤーの種類の列挙体
public enum PlayerEnum
{
    Player_1,
    Player_2,
}
//キャラクターの種類の列挙体
public enum CharacterEnum
{
    UnityChan,
}
//キャラクターの立ち絵の種類の列挙体
public enum CharacterExpressionEnum
{
    Normal,
    Happy,
    Regret,
}
//ターンを表す列挙体
public enum TurnEnum
{
    None,
    MyTurn,
    OpponentTurn,
}
//トランプのマークを表す列挙体
public enum CardMark
{
    Clover,
    Spade,
    Diamond,
    Heart,
}
//アニメーションの種類を表す列挙体
public enum CharacterAnimEnum
{
    UpDown,
    Happy,
    Regret,
}

//音声の種類を表す列挙体
public enum CharacterVoiceEnum 
{
    Select,
    IsPair,
    Miss,
    Waiting,
}
