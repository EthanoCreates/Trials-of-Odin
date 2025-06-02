using UnityEngine;

namespace TrialsOfOdin.Generics
{
    [System.Serializable]
    public class Stat<T>
    {
        [SerializeField] private string name;
        [SerializeField] private T baseValue;
        [SerializeField] private T modifiedValue;

        public Stat(string name, T baseValue)
        {
            this.name = name;
            this.baseValue = baseValue;
            modifiedValue = baseValue;
        }

        public void Modify(System.Func<T, T> modifier)
        {
            modifiedValue = modifier(baseValue);
        }

        public void Reset() => modifiedValue = baseValue;
        public T GetValue() => modifiedValue;
        public string GetName() => name;
    }
}
