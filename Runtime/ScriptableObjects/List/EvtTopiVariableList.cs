using PeartreeGames.Evt.Variables.Lists;
using UnityEngine;

namespace PeartreeGames.Evt.Topiary
{
    public abstract class EvtTopiVariableList<T> : EvtVariableList<T>
    {
        [SerializeField] private string valueName;
        public string Name => valueName;
        
        public new void Add(T item) => Value.Add(item);
        public new void Remove(T item) => Value.Remove(item);
        public new void Clear() => Value.Clear();
        public new bool Contains(T item) => Value.Contains(item);
    }
}