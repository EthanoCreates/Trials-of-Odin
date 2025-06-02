using TrialsOfOdin.Stats;
using UnityEngine;

namespace TrialsOfOdin
{
    public class DamageHandler : MonoBehaviour
    {
        [SerializeField] private CharacterStats stats;
        //DamageResistance
        //DamageVunerability

        public CharacterProfile GetCharacter => stats.Character; 
        public void ApplyDamage(float damage)
        {
            stats.Health.TakeDamage(damage);
        }
    }
}
