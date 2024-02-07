using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MemoryGameManager: MonoBehaviour 
{
    public delegate void CardActionDelegate();
    //�J�[�h�}�l�[�W���[
    [SerializeField]
    CardManager cardManager;
    //�v���C���[�}�l�[�W���[
    [SerializeField]
    PlayerManager playerManager;
    //�l�b�g���[�N�֌W�̃R���g���[���[
    [SerializeField]
    MemoryGameNetworkController networkController;
    //��ʂɔz�u����J�[�h�̍ŏ��̈ʒu
    [SerializeField]
    Vector2 leftUpPos;
    //�J�[�h�Ԃ�x���̊Ԋu
    [SerializeField]
    float xSpacing;
    //�J�[�h�Ԃ�y���̊Ԋu
    [SerializeField]
    float ySpacing;
    //�J�[�h�I�u�W�F�N�g�̐e
    [SerializeField]
    GameObject cardField;

    //�L�����N�^�[�̃C���X�g�\���ʒu
    [SerializeField]
    List<Transform> illustTransforms;     //�����^�ɕϊ����邩�͌���

    //�񋓑̂Ō��݂̃v���C���[��\��
    [SerializeField]
    PlayerEnum nowTurn;
    //�g�����v��I���ł����Ԃ���\��
    [SerializeField]
    bool canSelect;
    //�g�����v�̐����`�F�b�N�֌W�̏������I����������m�F
    [SerializeField]
    bool canCheckPair;
    //���݃Q�[���̏����������s���Ă��邩��\��
    [SerializeField]
    bool isReady = false;

    //���^�[���Ƀv���C���[��؂�ւ��邩
    [SerializeField]
    bool switchPlayer = false;

    //�󂯎�菈�������s������
    [SerializeField]
    bool isReceive = false;

    //���̒[��������\�ȃv���C���[�̎��
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
                //���̃^�[��������������s�\�ȃ^�[����
                if (!myPlayerEnum.Contains(nowTurn))
                {
                    Debug.Log("���葀��ł�");
                    UpdateSelectWhenNotCanSelect();
                    break;
                }
                if (canSelect)
                {
                    UpdateSelectWhenCanSelect();
                }
                else
                {
                    Debug.Log("�I���\��Ԃł͂���܂���");
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
    //gameState��Ready�̎��̏���
    void UpdateReady()
    {
        //Player�̏����ݒ���s��
        playerManager.CreatePlayerObject(illustTransforms);
        //�J�[�h�̏����ݒ���s��
        StartCoroutine(cardManager.CreateCard(leftUpPos, xSpacing, ySpacing, cardField, CreateCardCallBack));
        isReady = true;
    }
    //gameState��ReadySelect�̎��̏���
    void UpdateReadySelect()
    {
        bool contain = myPlayerEnum.Contains(nowTurn);
        canSelect = contain;
        canCheckPair = contain;
        gameState = GameState.Select;
    }

    //�����̒[��������\�ȃ^�[���̎���select���̏���
    void UpdateSelectWhenCanSelect()
    {
        int result = playerManager.ExecuteSelectCard(nowTurn);
        if (result != -1)
        {
            bool responce = cardManager.CheckSelected(result, ReverseSelectCardCallBack);
            //�I���\�ȃJ�[�h��������
            if (responce)
            {
                //�L�����N�^�[�̃A�j���[�V�����ƃ{�C�X�Đ�
                playerManager.PlayCharAction(nowTurn, CharacterAnimEnum.UpDown);

                //ID�f�[�^�𑗐M
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
            Debug.Log("null���Ԃ���܂���");
        }
    }
    //�����̒[��������s�\�Ȏ���select�̏���
    void UpdateSelectWhenNotCanSelect()
    {
        if (!isReceive)
        {
            networkController.ReceiveData();
            isReceive = true;
        }
        //���肩�������M������
        if (networkController.GetOpponentData().card_id != -1)
        {
            cardManager.CheckSelected(networkController.GetOpponentData().card_id, ReverseSelectCardCallBack);
            //�L�����N�^�[�̃A�j���[�V�����ƃ{�C�X�Đ�
            playerManager.PlayCharAction(nowTurn, CharacterAnimEnum.UpDown);

            isReceive = false;
        }
    }
    //gameState��CheckPair�̎��̏���
    void UpdateCheckPair()
    {
        //2���J�[�h��I��������
        if (cardManager.GetNumberOfSelectedCards() == 2)
        {
            //�񖇂̃J�[�h�͓���������
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
    //gaemState��ReadyNextTurn�̎��̏���
    void UpdateReadyNextTurn()
    {
        //�v���C���[����シ�鎞
        if (switchPlayer)
        {
            int nowPlayerNumber = (int)nowTurn;
            PlayerEnum nextPlayer;
            Debug.Log("����^�[���Ɉڂ�܂�");
            do
            {
                int nextNumber = (nowPlayerNumber + 1) % playerManager.GetNumberOfPlayer();
                nextPlayer = (PlayerEnum)Enum.ToObject(typeof(PlayerEnum), nextNumber);
            } while (!playerManager.GetIsConnect(nextPlayer));  //���̃v���C���[���ʐM�\��Ԃ��m�F

            nowTurn = nextPlayer;
            Debug.Log($"���̃v���C���[{nextPlayer}");
        }
        else
        {
            Debug.Log("�Ăю����̃^�[���ł�");
        }

        gameState = GameState.ReadySelect;
    }
    //�J�[�h�쐬���\�b�h�̃R�[���o�b�N
    void CreateCardCallBack()
    {
        isReady = false;    
        gameState = GameState.ReadySelect;
    }
    //�I�������J�[�h�̃A�j���[�V�����̃R�[���o�b�N�֐�
    void ReverseSelectCardCallBack()
    {
        gameState = GameState.CheckPair;
    }
    //�I���ς݃J�[�h�̃A�j���[�V�����̃R�[���o�b�N�֐�
    void ReverseSelectedCardCallBack()
    {
        gameState = GameState.ReadyNextTurn;
    }
}
