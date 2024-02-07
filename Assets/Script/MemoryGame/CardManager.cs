using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    //�}�[�N���̃g�����v�̖���
    const int CardsPerMark = 13;
    //�S���̃g�����v�̖���
    [SerializeField]
    int totalCards;
    //�g�����v�\�ʂ̉摜���X�g�A����鏇�Ԃ͗񋓑̂̃}�[�N�̏��Ԃɍ��킹��
    [SerializeField]
    List<Sprite> cardFaces;
    //�g�����v���ʂ̉摜
    [SerializeField]
    Sprite cardBack;
    //�J�[�h�I�u�W�F�N�g�̃v���n�u
    [SerializeField]
    GameObject cardPrefab;

    //����ɂ���Ă͔z��ŏ[��
    Dictionary<int, Card> cards = new Dictionary<int, Card>();

    Dictionary<int, CardComponent> cardComponents = new Dictionary<int, CardComponent>();

    //�I���ς݂̃g�����v�̎����^�z��
    Dictionary<int, CardComponent> selectedCards = new Dictionary<int, CardComponent>();

    private void Awake()
    {
        CreateCardInstance();
    }
    private void Start()
    {
    }

    public IEnumerator CreateCard(Vector3 leftUpPos, float xSpacing, float ySpacing, GameObject cardField, MemoryGameManager.CardActionDelegate callBack)
    {
        //�ړ��A�j���[�V�����O�Ƀg�����v�𐶐�����ʒu
        Vector3 initializePos = new Vector3(0, 7, 0);
        ShuffleCardArray();
        int index = 0;
        Vector3 setPos = leftUpPos;
        //�c�ɕ��ׂ�g�����v�̐�
        for (int y = 0; y < 4; y++) 
        {
            //���ɕ��ׂ�g�����v�̐�
            for (int x = 0; x < 13; x++)
            {
                //Card�N���X��instance�������邾���ɕύX�ł���͂�
                GameObject instance = Instantiate(cardPrefab, initializePos, Quaternion.identity);
                CardComponent cardComponent = instance.gameObject.GetComponent<CardComponent>();
                cardComponent.card = cards[index];
                instance.transform.parent = cardField.transform;
                cardComponent.id = index;
                cardComponent.InitializeSpriteRenderer();
                if (index != totalCards - 1)
                {
                    StartCoroutine(cardComponent.LinerMove(setPos));
                }
                else
                {
                    yield return StartCoroutine(cardComponent.LinerMove(setPos));
                }
                cardComponents.Add(index, cardComponent);
                index++;
                //�z�u�ʒu���E�ɂ��炷
                setPos = new Vector3(setPos.x + xSpacing, setPos.y);

                yield return new WaitForSeconds(0.1f);
            }
            //�z�u�ʒu�����ɂ��炷
            setPos = new Vector3(leftUpPos.x, setPos.y - ySpacing);
        }
        Debug.Log("�J�[�h�̔z�u���������܂���");
        callBack();
    }

    //�J�[�h�N���X�̏��������s��
    void CreateCardInstance()
    {
        int index = 0;
        //�}�[�N�̗񋓑̗̂v�f���擾
        CardMark[] cardMarks = (CardMark[])Enum.GetValues(typeof(CardMark));
        //�}�[�N���Ƀ��[�v
        foreach (CardMark mark in cardMarks)
        {
            for (int i = 1; i <= CardsPerMark; i++)
            {
                Card card = new Card();
                card.mark = mark;
                card.number = i;
                card.SetSprite(cardFaces[index], cardBack);
                cards.Add(index, card);
                cards[index] = card;
                index++;
            }
        }
    }
    //�J�[�h�̔z����V���b�t�����郁�\�b�h
    void ShuffleCardArray()
    {
        for (int i = totalCards - 1; i > 0; i--)
        {
            int random = UnityEngine.Random.RandomRange(0, i);
            if (i != random)
            {
                SwapCardIndex(i, random);
            }
        }
    }
    //�C���f�b�N�X�̌������s�����\�b�h
    void SwapCardIndex(int index_1, int index_2)
    {
        Card temp = cards[index_1];
        cards[index_1] = cards[index_2];
        cards[index_2] = temp;
    }
    //�I���ς݂̃J�[�h�����m�F���A���I���Ȃ�A�j���[�V�������s�����\�b�h
    public bool CheckSelected(int card_id, MemoryGameManager.CardActionDelegate callBack)
    {
        //�߂����Ă��Ȃ��Ȃ�True
        bool result = false;
        //�߂����Ă��Ȃ���
        if (!selectedCards.ContainsKey(card_id))
        {
            selectedCards.Add(card_id, cardComponents[card_id]);
            StartCoroutine(cardComponents[card_id].ReverseCard(callBack));
            result = true;
        }
        else
        {
        }

        return result;
    }
    //�J�[�h�̃A�j���[�V�������s�����\�b�h
    public void ExecuteReverseAnim(int card_id, MemoryGameManager.CardActionDelegate callBack)
    {
        StartCoroutine(cardComponents[card_id].ReverseCard(callBack));
    }
    //�I���ς݃J�[�h�̖�����ԐM���郁�\�b�h
    public int GetNumberOfSelectedCards()
    {
        return selectedCards.Count;
    }

    //�I���ς݃J�[�h�𗠕Ԃ����\�b�h
    public void ReverseSelectedCards(MemoryGameManager.CardActionDelegate callBack)
    {
        foreach (KeyValuePair<int, CardComponent> selectedCard in selectedCards)
        {
            StartCoroutine(selectedCard.Value.ReverseCard(callBack));
        }
    }
    //�I���ς݂̃J�[�h���������������ׂ郁�\�b�h
    public bool CheckSameNumber()
    {
        bool result = true;
        CardComponent beforeCard = null;
        foreach (KeyValuePair<int, CardComponent> selectedCard in selectedCards)
        {
            //�O�̃J�[�h�̏�񂪂���Ƃ�
            if (beforeCard != null)
            {
                if (beforeCard.card.number != selectedCard.Value.card.number)
                {
                    result = false;
                    break;
                }
            }
            beforeCard = selectedCard.Value;
        }

        return result;  
    }
    //�I���ς݃J�[�h�R���|�[�l���g�̎����^�z������Z�b�g���郁�\�b�h
    public void ResetSelectedCards()
    {
        selectedCards.Clear();
    }
    //�w�肵���J�[�h�R���|�[�l���g��ID��Ԃ����\�b�h
    public int GetIDByCard(CardComponent card)
    {
        //LINQ���g�p����value����L�[���擾
        int key = cardComponents.FirstOrDefault(x => x.Value == card).Key;
        Debug.Log(key);
        return key;
    }
}

