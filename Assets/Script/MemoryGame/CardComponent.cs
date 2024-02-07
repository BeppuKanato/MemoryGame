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

    //�J�[�h�̈��]�A�j���[�V����
    public IEnumerator ReverseCard(MemoryGameManager.CardActionDelegate callBack)
    {

        float theta = 0;
        //��x���Ԃ�����
        bool isReverse = false;
        Vector3 originalScale = gameObject.transform.localScale;
        while (true)
        {
            theta += Time.deltaTime * 4f;//4�{��
            float sinTheta = Mathf.Sin(theta);
            //sin�̒l���Βl�ɏC������
            float absSinTheta = Mathf.Abs(sinTheta - 1);
            //�X�P�[�����������Ȃ�����摜�����ւ���
            if (absSinTheta < 0.01 && isReverse == false)
            {
                //�ʂ̓���ւ����s��
                spriteRenderer.sprite = isFace ? card.back : card.face;
                //�g���K�[��ؑւ�
                isFace = !isFace;
                isReverse = true;
            }
            Vector3 nowScale = gameObject.transform.localScale;
            this.gameObject.transform.localScale = new Vector3(absSinTheta, nowScale.y, nowScale.z);

            //���]�����ꍇ
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
    //�J�[�h�̒����ړ��A�j���[�V����
    public IEnumerator LinerMove(Vector3 targetPos)
    {
        float theta = 0;
        Vector3 originPos = gameObject.transform.position;
        while (true)
        {
            theta += Time.deltaTime * 4;
            float sinTheta = Mathf.Sin(theta);

            gameObject.transform.position = Vector3.Lerp(originPos, targetPos, sinTheta);

            //������x�ړ������������Ƃ�
            if (sinTheta > 0.98)
            {
                gameObject.transform.position = targetPos;
                break;
            }
            yield return null;
        }
    }
    //�X�v���C�g�����_���[�̏�����
    public void InitializeSpriteRenderer()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        //spriteRenderer���t���Ă��Ȃ��Ƃ�
        if (spriteRenderer == null)
        {
            gameObject.AddComponent<SpriteRenderer>();
        }
        //�ŏ��͕K�����ʂɐݒ�
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
