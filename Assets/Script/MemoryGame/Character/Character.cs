using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Character : ScriptableObject
{
    //�L�����N�^�[��(�L�����N�^�[�̎��)
    [SerializeField]
    CharacterEnum characterEnum;
    //�L�����N�^�[�̃C���X�g��ێ����郊�X�g inspector�Őݒ肷��p
    [SerializeField]
    List<Sprite> illustsList = new List<Sprite>();
    //�L�����N�^�[�̃C���X�g��ێ����鎫���^�z��
    Dictionary<CharacterExpressionEnum, Sprite> illusts = new Dictionary<CharacterExpressionEnum, Sprite>();

    //�L�����N�^�[�̃{�C�X��ێ����鎫���^�z��
    Dictionary<CharacterVoiceEnum, List<AudioClip>> voices = new Dictionary<CharacterVoiceEnum, List<AudioClip>>();

    //�J�[�h�I�����̃{�C�X
    [SerializeField]
    List<AudioClip> selectVoices = new List<AudioClip>();
    //�������������Ƃ��̃{�C�X
    [SerializeField]
    List<AudioClip> isPairtVoices = new List<AudioClip>();
    //�O�������̃{�C�X
    [SerializeField]
    List<AudioClip> missVoices = new List<AudioClip>();
    //����҂��̃{�C�X
    [SerializeField]
    List<AudioClip> waitingVoices = new List<AudioClip>();

    //List�����ɃC���X�g�̎����^�z����쐬���郁�\�b�h
    public void CreateIllustDictionary()
    {
        for (int i = 0; i < illustsList.Count; i++)
        {
            CharacterExpressionEnum characterExpressionEnum = (CharacterExpressionEnum)Enum.ToObject(typeof(CharacterExpressionEnum), i);
            illusts.Add(characterExpressionEnum, illustsList[i]);
        }
    }

    //List����L�����N�^�[�̃{�C�X�̎����^�z����쐬���郁�\�b�h
    public void CreateVoiceDictionary()
    {
        voices.Add(CharacterVoiceEnum.Select, selectVoices);
        voices.Add(CharacterVoiceEnum.IsPair, isPairtVoices);
        voices.Add(CharacterVoiceEnum.Miss, missVoices);
        voices.Add(CharacterVoiceEnum.Waiting, waitingVoices);
    }
    //�L�����N�^�[�̃C���X�g�̎����^�z���Ԃ����\�b�h
    public Dictionary<CharacterExpressionEnum, Sprite> GetIllusts()
    {
        return illusts;
    }
    //�w�肳�ꂽ��ނ̃{�C�X�̗ʂ�Ԃ����\�b�h
    public int GetNumberOfVoices(CharacterVoiceEnum voiceEnum)
    {
        return voices[voiceEnum].Count;
    }
    //�w�肳�ꂽ�{�C�X��Ԃ����\�b�h
    public AudioClip GetAudioClip(CharacterVoiceEnum voiceEnum, int index)
    {
        List<AudioClip> list = voices[voiceEnum];
        return list[index];
    }
}
