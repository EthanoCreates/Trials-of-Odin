using UnityEngine;

namespace TrialsOfOdin
{
    public enum CharacterType
    {
        Player,
        NPC,
        Enemy,
        Boss
    }
    [System.Serializable]
    public class CharacterProfile
    {
        public string name;
        public int id;
        public CharacterType type;
    }
}
