using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : ScriptableObject
{
    public GameObject gameObject;
    //カードのマーク
    public CardMark mark;
    //カードの数字
    public int number;
    //オブジェクトのspriteRenderer
    [SerializeField]
    SpriteRenderer spriteRenderer;
    //表面の画像
    public Sprite face;
    //裏面の画像
    public Sprite back;
    //カードのsprite設定
    public void SetSprite(Sprite face, Sprite back)
    {
        this.face = face;
        this.back = back;
    }
}
