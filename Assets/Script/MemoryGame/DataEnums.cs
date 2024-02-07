using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�Q�[���̐i�s�Ǘ��񋓑�
public enum GameState
{
    None,
    Ready,
    ReadySelect,
    Select,
    CheckPair,
    ReadyNextTurn,
    Result,
    EndGame,
}
//�v���C���[�̎�ނ̗񋓑�
public enum PlayerEnum
{
    Player_1,
    Player_2,
}
//�L�����N�^�[�̎�ނ̗񋓑�
public enum CharacterEnum
{
    UnityChan,
}
//�L�����N�^�[�̗����G�̎�ނ̗񋓑�
public enum CharacterExpressionEnum
{
    Normal,
    Happy,
    Regret,
}
//�^�[����\���񋓑�
public enum TurnEnum
{
    None,
    MyTurn,
    OpponentTurn,
}
//�g�����v�̃}�[�N��\���񋓑�
public enum CardMark
{
    Clover,
    Spade,
    Diamond,
    Heart,
}
//�A�j���[�V�����̎�ނ�\���񋓑�
public enum CharacterAnimEnum
{
    UpDown,
    Happy,
    Regret,
}

//�����̎�ނ�\���񋓑�
public enum CharacterVoiceEnum 
{
    Select,
    IsPair,
    Miss,
    Waiting,
}
