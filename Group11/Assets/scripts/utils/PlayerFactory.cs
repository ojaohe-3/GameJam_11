using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerFactory
{
    public abstract Player GeneratePlayer(GameObject obj0, PlayerCharacter ch);
}