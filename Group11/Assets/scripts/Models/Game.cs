using System;
using System.Collections.Generic;
using Objects;

namespace Models
{
    public interface IGame
    {
        List<Character> Players { get; set; }
        Player Player { get; set; }

        Action GameStateChange { get; set; }
        void MeetingCalled();
        bool GameStatus();
        void OnPlayerTaskUpdate();
        
        
    }
}