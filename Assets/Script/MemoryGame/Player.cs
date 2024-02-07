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
    //���g��CPU����\��
    public bool cpu;

    //�g�����v�̑I���������s�����\�b�h
    public int SelectCard()
    {
        int result = -1;
        //������CPU�ł͂Ȃ��Ƃ�
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
    //�l�ԑ���̎��̃J�[�h�I���������\�b�h
    int PlayerSelectCard()
    {
        int result = -1;
        //�}�E�X���E�N���b�N���ꂽ�Ƃ�
        if (Input.GetMouseButtonDown(0))
        {
            //ray���΂�
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //ray���I�u�W�F�N�g�Ƀq�b�g������
            RaycastHit2D hit = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction, 10.0f);
            //�I�u�W�F�N�g�̃^�O���擾
            if (hit.collider != null && hit.collider.tag == "Card")
            {
                int card = hit.collider.gameObject.GetComponent<CardComponent>().id;
                result = card;
            }
        }

        return result;
    }
    //CPU����̎��̃J�[�h�I���������\�b�h
    int CPUSelectCard()
    {
        int result = UnityEngine.Random.RandomRange(0, 52);
        Debug.Log("CPU�̏����ł�");
        return result;
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
        SetSprite(CharacterExpressionEnum.Normal);
    }
    //AoudioSouce�̏�����
    public void InitializeAudioSouce()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            gameObject.AddComponent<AudioSource>();
        }
    }
    //�L�����N�^�[�̃Z�b�g
    public void SetCharacter(Character character)
    {
        this.character = character;
    }

    //�摜�̃Z�b�g
    public void SetSprite(CharacterExpressionEnum express)
    {
        spriteRenderer.sprite = character.GetIllusts()[express];
    }

    //��񒵂˂�A�j���[�V����
    public IEnumerator UpDownAnim()
    {
        Vector3 beforePos = this.gameObject.transform.position;
        float theta = 0f;
        while (true)
        {
            theta += Time.deltaTime * 4;
            float sinTheta = Mathf.Sin(theta);

            //���ˏI������
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

    //�w�肳�ꂽ��ނ̉����������_���ɑI���A�Đ����郁�\�b�h
    public void PlayVoice(CharacterVoiceEnum voiceEnum)
    {
        AudioClip voice = SelectVoice(voiceEnum);
        audioSource.PlayOneShot(voice);
    }

    //�{�C�X��I�����郁�\�b�h
    AudioClip SelectVoice(CharacterVoiceEnum voiceEnum)
    {
        int numberOfVoice = character.GetNumberOfVoices(voiceEnum);
        int random = UnityEngine.Random.RandomRange(0, numberOfVoice);

        AudioClip voice = character.GetAudioClip(voiceEnum, random);

        return voice;
    }
}

