using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Character character;
    SpriteRenderer spriteRenderer;
    AudioSource audioSource;
    public bool isConnect = true;
    //自身がCPUかを表す
    public bool cpu;

    //トランプの選択処理を行うメソッド
    public int SelectCard()
    {
        int result = -1;
        //自分がCPUではないとき
        if (!cpu)
        {
            result = PlayerSelectCard();
        }
        else
        {
            result = CPUSelectCard();
        }

        return result;
    }
    //人間操作の時のカード選択処理メソッド
    int PlayerSelectCard()
    {
        int result = -1;
        //マウスが右クリックされたとき
        if (Input.GetMouseButtonDown(0))
        {
            //rayを飛ばす
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //rayがオブジェクトにヒットしたら
            RaycastHit2D hit = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction, 10.0f);
            //オブジェクトのタグを取得
            if (hit.collider != null && hit.collider.tag == "Card")
            {
                int card = hit.collider.gameObject.GetComponent<CardComponent>().id;
                result = card;
            }
        }

        return result;
    }
    //CPU操作の時のカード選択処理メソッド
    int CPUSelectCard()
    {
        int result = UnityEngine.Random.RandomRange(0, 52);
        Debug.Log("CPUの処理です");
        return result;
    }

    //スプライトレンダラーの初期化
    public void InitializeSpriteRenderer()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        //spriteRendererが付いていないとき
        if (spriteRenderer == null)
        {
            gameObject.AddComponent<SpriteRenderer>();
        }
        SetSprite(CharacterExpressionEnum.Normal);
    }
    //AoudioSouceの初期化
    public void InitializeAudioSouce()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            gameObject.AddComponent<AudioSource>();
        }
    }
    //キャラクターのセット
    public void SetCharacter(Character character)
    {
        this.character = character;
    }

    //画像のセット
    public void SetSprite(CharacterExpressionEnum express)
    {
        spriteRenderer.sprite = character.GetIllusts()[express];
    }

    //一回跳ねるアニメーション
    public IEnumerator UpDownAnim()
    {
        Vector3 beforePos = this.gameObject.transform.position;
        float theta = 0f;
        while (true)
        {
            theta += Time.deltaTime * 4;
            float sinTheta = Mathf.Sin(theta);

            //跳ね終えたら
            if (sinTheta < 0)
            {
                this.gameObject.transform.position = beforePos;
                SetSprite(CharacterExpressionEnum.Normal);
                yield break;
            }

            this.gameObject.transform.position = new Vector3(beforePos.x, (float)(beforePos.y + (sinTheta * 1)), beforePos.z);

            yield return null;
        }
    }

    //指定された種類の音声をランダムに選択、再生するメソッド
    public void PlayVoice(CharacterVoiceEnum voiceEnum)
    {
        AudioClip voice = SelectVoice(voiceEnum);
        audioSource.PlayOneShot(voice);
    }

    //ボイスを選択するメソッド
    AudioClip SelectVoice(CharacterVoiceEnum voiceEnum)
    {
        int numberOfVoice = character.GetNumberOfVoices(voiceEnum);
        int random = UnityEngine.Random.RandomRange(0, numberOfVoice);

        AudioClip voice = character.GetAudioClip(voiceEnum, random);

        return voice;
    }
}

