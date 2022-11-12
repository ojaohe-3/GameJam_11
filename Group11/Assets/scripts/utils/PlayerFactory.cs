using Models;
using Objects;
using UnityEngine;

namespace utils
{
    public abstract class PlayerFactory
    {
        public abstract Player GeneratePlayer(GameObject obj0, PlayerCharacter ch);
    }
}