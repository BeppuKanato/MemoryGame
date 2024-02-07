using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    int NumberOfPlayer;
    //�v���C���[�I�u�W�F�N�g�̃v���n�u
    [SerializeField]
    GameObject playerPrefab;
    //�v���C���[��ێ�
    Dictionary<PlayerEnum, Player> players = new Dictionary<PlayerEnum, Player>();
    //�v���C���[�̒ʂ��ԍ�
    int uniquePlayerID = 0;

    //�L�����N�^�[�̐ݒ�I�u�W�F�N�g���X�g
    [SerializeField]
    List<Character> characterList;
    //�L�����N�^�[�̐ݒ�I�u�W�F�N�g�����^�z��
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
    //�w�肳�ꂽ�v���C���[�̃J�[�h�̑I�����\�b�h���Ăяo��
    public int ExecuteSelectCard(PlayerEnum nowTurn)
    {
        int result = players[nowTurn].SelectCard();

        return result;
    }
    //�v���C���[�I�u�W�F�N�g�̐������s�����\�b�h
    public void CreatePlayerObject(List<Transform> illustPoss)
    {
        for (int i = 0; i < NumberOfPlayer; i++)
        {
            GameObject instance = Instantiate(playerPrefab, illustPoss[i].position, illustPoss[i].rotation);
            instance.transform.localScale = illustPoss[i].localScale;
            Player player = instance.AddComponent<Player>();
            PlayerEnum playerEnum = (PlayerEnum)Enum.ToObject(typeof(PlayerEnum), uniquePlayerID);
            //�v���C���[�̃L�����N�^�[��ݒ�
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

    //List�����ɃL�����N�^�[�ݒ�̎����^�z����쐬���郁�\�b�h
    public void CreateDictionary()
    {
        for (int i = 0; i < characterList.Count; i++)
        {
            CharacterEnum characterEnum = (CharacterEnum)Enum.ToObject(typeof(CharacterEnum), i);
            characters.Add(characterEnum, characterList[i]);
        }
    }

    //�L�����N�^�[�N���X�̏����ݒ���s�����\�b�h
    void InitializeCharacters()
    {
        foreach (Character characer in characterList)
        {
            characer.CreateIllustDictionary();
            characer.CreateVoiceDictionary();
        }
    }
    //�L�����N�^�[�̃A�j���[�V�����ƃ{�C�X�̍Đ����s�����\�b�h
    public void PlayCharAction(PlayerEnum playerEnum, CharacterAnimEnum animEnum)
    {
        switch (animEnum)
        {
            case CharacterAnimEnum.UpDown:
                //�A�j���[�V�����Đ�
                StartCoroutine(players[playerEnum].UpDownAnim());
                //�{�C�X�Đ�
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

    //�v���C���[�̐���Ԃ����\�b�h
    public int GetNumberOfPlayer()
    {
        return players.Count;
    }
    //�w�肳�ꂽ�v���C���[�̒ʐM��Ԃ�Ԃ����\�b�h
    public bool GetIsConnect(PlayerEnum playerEnum)
    {
        return players[playerEnum].isConnect;
    }
}
