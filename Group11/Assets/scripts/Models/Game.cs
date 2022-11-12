using System.Collections.Generic;
using Objects;

namespace Models
{
    public interface IGame
    {
        List<Character> _players { get; set; }
        Player _player { get; set; }
        void MeetingCalled();
        bool GameStatus();
        void OnPlayerTaskUpdate();
    }
}