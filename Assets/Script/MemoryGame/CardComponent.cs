using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CardComponent : MonoBehaviour
{
    public int id;
    public Card card;
    SpriteRenderer spriteRenderer;

    bool isFace = false;

    //カードの一回転アニメーション
    public IEnumerator ReverseCard(MemoryGameManager.CardActionDelegate callBack)
    {

        float theta = 0;
        //一度裏返したか
        bool isReverse = false;
        Vector3 originalScale = gameObject.transform.localScale;
        while (true)
        {
            theta += Time.deltaTime * 4f;//4倍速
            float sinTheta = Mathf.Sin(theta);
            //sinの値を絶対値に修正する
            float absSinTheta = Mathf.Abs(sinTheta - 1);
            //スケールが小さくなったら画像を入れ替える
            if (absSinTheta < 0.01 && isReverse == false)
            {
                //面の入れ替えを行う
                spriteRenderer.sprite = isFace ? card.back : card.face;
                //トリガーを切替え
                isFace = !isFace;
                isReverse = true;
            }
            Vector3 nowScale = gameObject.transform.localScale;
            this.gameObject.transform.localScale = new Vector3(absSinTheta, nowScale.y, nowScale.z);

            //一回転した場合
            if (absSinTheta > 0.98 && isReverse)
            {
                this.gameObject.transform.localScale = new Vector3(1, nowScale.y, nowScale.z);
                break;
            }

            yield return null;
        }
        yield return new WaitForSeconds(0.3f);
        callBack();
    }
    //カードの直線移動アニメーション
    public IEnumerator LinerMove(Vector3 targetPos)
    {
        float theta = 0;
        Vector3 originPos = gameObject.transform.position;
        while (true)
        {
            theta += Time.deltaTime * 4;
            float sinTheta = Mathf.Sin(theta);

            gameObject.transform.position = Vector3.Lerp(originPos, targetPos, sinTheta);

            //ある程度移動が完了したとき
            if (sinTheta > 0.98)
            {
                gameObject.transform.position = targetPos;
                break;
            }
            yield return null;
        }
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
        //最初は必ず裏面に設定
        if (isFace)
        {
            isFace = false;
            spriteRenderer.sprite = card.back;
        }
        else
        {
            spriteRenderer.sprite = card.back;
        }
    }
}
