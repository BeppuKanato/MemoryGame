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
    //マーク毎のトランプの枚数
    const int CardsPerMark = 13;
    //全部のトランプの枚数
    [SerializeField]
    int totalCards;
    //トランプ表面の画像リスト、入れる順番は列挙体のマークの順番に合わせる
    [SerializeField]
    List<Sprite> cardFaces;
    //トランプ裏面の画像
    [SerializeField]
    Sprite cardBack;
    //カードオブジェクトのプレハブ
    [SerializeField]
    GameObject cardPrefab;

    //今後によっては配列で充分
    Dictionary<int, Card> cards = new Dictionary<int, Card>();

    Dictionary<int, CardComponent> cardComponents = new Dictionary<int, CardComponent>();

    //選択済みのトランプの辞書型配列
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
        //移動アニメーション前にトランプを生成する位置
        Vector3 initializePos = new Vector3(0, 7, 0);
        ShuffleCardArray();
        int index = 0;
        Vector3 setPos = leftUpPos;
        //縦に並べるトランプの数
        for (int y = 0; y < 4; y++) 
        {
            //横に並べるトランプの数
            for (int x = 0; x < 13; x++)
            {
                //Cardクラスにinstanceを代入するだけに変更できるはず
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
                //配置位置を右にずらす
                setPos = new Vector3(setPos.x + xSpacing, setPos.y);

                yield return new WaitForSeconds(0.1f);
            }
            //配置位置を下にずらす
            setPos = new Vector3(leftUpPos.x, setPos.y - ySpacing);
        }
        Debug.Log("カードの配置が完了しました");
        callBack();
    }

    //カードクラスの初期化を行う
    void CreateCardInstance()
    {
        int index = 0;
        //マークの列挙体の要素を取得
        CardMark[] cardMarks = (CardMark[])Enum.GetValues(typeof(CardMark));
        //マーク毎にループ
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
    //カードの配列をシャッフルするメソッド
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
    //インデックスの交換を行うメソッド
    void SwapCardIndex(int index_1, int index_2)
    {
        Card temp = cards[index_1];
        cards[index_1] = cards[index_2];
        cards[index_2] = temp;
    }
    //選択済みのカードかを確認し、未選択ならアニメーションを行うメソッド
    public bool CheckSelected(int card_id, MemoryGameManager.CardActionDelegate callBack)
    {
        //めくられていないならTrue
        bool result = false;
        //めくられていない時
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
    //カードのアニメーションを行うメソッド
    public void ExecuteReverseAnim(int card_id, MemoryGameManager.CardActionDelegate callBack)
    {
        StartCoroutine(cardComponents[card_id].ReverseCard(callBack));
    }
    //選択済みカードの枚数を返信するメソッド
    public int GetNumberOfSelectedCards()
    {
        return selectedCards.Count;
    }

    //選択済みカードを裏返すメソッド
    public void ReverseSelectedCards(MemoryGameManager.CardActionDelegate callBack)
    {
        foreach (KeyValuePair<int, CardComponent> selectedCard in selectedCards)
        {
            StartCoroutine(selectedCard.Value.ReverseCard(callBack));
        }
    }
    //選択済みのカードが同じ数字か調べるメソッド
    public bool CheckSameNumber()
    {
        bool result = true;
        CardComponent beforeCard = null;
        foreach (KeyValuePair<int, CardComponent> selectedCard in selectedCards)
        {
            //前のカードの情報があるとき
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
    //選択済みカードコンポーネントの辞書型配列をリセットするメソッド
    public void ResetSelectedCards()
    {
        selectedCards.Clear();
    }
    //指定したカードコンポーネントのIDを返すメソッド
    public int GetIDByCard(CardComponent card)
    {
        //LINQを使用してvalueからキーを取得
        int key = cardComponents.FirstOrDefault(x => x.Value == card).Key;
        Debug.Log(key);
        return key;
    }
}

