using System;

namespace Models
{    
    [Serializable]
    public struct PlayerCharacter
    {
        public bool Impostor{get; set;}
        public string Name { get; set; }
        public string Uid { get; }
        
    }
}
