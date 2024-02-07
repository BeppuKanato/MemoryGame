using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MemoryGameDataStruct 
{
    public PlayerEnum player_enum;
    public int card_id;

    public MemoryGameDataStruct(PlayerEnum player, int id)
    {
        this.player_enum = player;
        this.card_id = id;
    }
}