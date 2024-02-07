using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Character : ScriptableObject
{
    //キャラクター名(キャラクターの種類)
    [SerializeField]
    CharacterEnum characterEnum;
    //キャラクターのイラストを保持するリスト inspectorで設定する用
    [SerializeField]
    List<Sprite> illustsList = new List<Sprite>();
    //キャラクターのイラストを保持する辞書型配列
    Dictionary<CharacterExpressionEnum, Sprite> illusts = new Dictionary<CharacterExpressionEnum, Sprite>();

    //キャラクターのボイスを保持する辞書型配列
    Dictionary<CharacterVoiceEnum, List<AudioClip>> voices = new Dictionary<CharacterVoiceEnum, List<AudioClip>>();

    //カード選択時のボイス
    [SerializeField]
    List<AudioClip> selectVoices = new List<AudioClip>();
    //数字が揃ったときのボイス
    [SerializeField]
    List<AudioClip> isPairtVoices = new List<AudioClip>();
    //外した時のボイス
    [SerializeField]
    List<AudioClip> missVoices = new List<AudioClip>();
    //操作待ちのボイス
    [SerializeField]
    List<AudioClip> waitingVoices = new List<AudioClip>();

    //Listを元にイラストの辞書型配列を作成するメソッド
    public void CreateIllustDictionary()
    {
        for (int i = 0; i < illustsList.Count; i++)
        {
            CharacterExpressionEnum characterExpressionEnum = (CharacterExpressionEnum)Enum.ToObject(typeof(CharacterExpressionEnum), i);
            illusts.Add(characterExpressionEnum, illustsList[i]);
        }
    }

    //Listからキャラクターのボイスの辞書型配列を作成するメソッド
    public void CreateVoiceDictionary()
    {
        voices.Add(CharacterVoiceEnum.Select, selectVoices);
        voices.Add(CharacterVoiceEnum.IsPair, isPairtVoices);
        voices.Add(CharacterVoiceEnum.Miss, missVoices);
        voices.Add(CharacterVoiceEnum.Waiting, waitingVoices);
    }
    //キャラクターのイラストの辞書型配列を返すメソッド
    public Dictionary<CharacterExpressionEnum, Sprite> GetIllusts()
    {
        return illusts;
    }
    //指定された種類のボイスの量を返すメソッド
    public int GetNumberOfVoices(CharacterVoiceEnum voiceEnum)
    {
        return voices[voiceEnum].Count;
    }
    //指定されたボイスを返すメソッド
    public AudioClip GetAudioClip(CharacterVoiceEnum voiceEnum, int index)
    {
        List<AudioClip> list = voices[voiceEnum];
        return list[index];
    }
}
