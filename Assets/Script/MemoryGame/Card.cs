using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : ScriptableObject
{
    public GameObject gameObject;
    //�J�[�h�̃}�[�N
    public CardMark mark;
    //�J�[�h�̐���
    public int number;
    //�I�u�W�F�N�g��spriteRenderer
    [SerializeField]
    SpriteRenderer spriteRenderer;
    //�\�ʂ̉摜
    public Sprite face;
    //���ʂ̉摜
    public Sprite back;
    //�J�[�h��sprite�ݒ�
    public void SetSprite(Sprite face, Sprite back)
    {
        this.face = face;
        this.back = back;
    }
}
