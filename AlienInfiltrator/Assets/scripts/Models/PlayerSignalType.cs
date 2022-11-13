using System;
using UnityEngine;

namespace Models
{
    public enum PlayerSignalType 
    {
        MoveAction, InteractAction, TaskStatusChange
    }

    [Serializable]
    public struct PlayerSignal
    {
        private Vector2 _pos;
        private Vector2 _vel;
        
    }
}
