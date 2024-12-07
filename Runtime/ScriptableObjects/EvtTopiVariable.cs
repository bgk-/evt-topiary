using PeartreeGames.Evt.Variables;
using UnityEngine;

namespace PeartreeGames.Evt.Topiary
{
    public abstract class EvtTopiVariable<T> : EvtVariable<T>
    {
        [SerializeField] private string valueName;
        public string Name => valueName;
    }
}