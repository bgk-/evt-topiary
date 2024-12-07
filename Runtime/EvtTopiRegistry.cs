using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PeartreeGames.Evt.Variables;
using PeartreeGames.Topiary.Unity;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace PeartreeGames.Evt.Topiary
{
    
    public class EvtTopiRegistry
    {
        private AsyncOperationHandle<IList<EvtVariable>> _aoList;
        private readonly Dictionary<string, EvtVariable> _variables;
        private readonly Dictionary<string, Delegate> _callbacks;
        private readonly Dialogue _dialogue;

        public EvtTopiRegistry(Dialogue dialogue)
        {
            _dialogue = dialogue;
            _variables = new Dictionary<string, EvtVariable>();
            _callbacks = new Dictionary<string, Delegate>();
        }

        public void OnValueChanged(string name, TopiValue value)
        {
            if (_variables.TryGetValue(name, out var variable))
            {
                switch (value.tag)
                {
                    case TopiValue.Tag.Bool when variable is EvtTopiBool b:
                        b.Value = value.Bool;
                        break;
                    case TopiValue.Tag.Number when variable is EvtTopiInt i:
                        i.Value = value.Int;
                        break;
                    case TopiValue.Tag.Number when variable is EvtTopiFloat f:
                        f.Value = value.Float;
                        break;
                    case TopiValue.Tag.String when variable is EvtTopiString s:
                        s.Value = value.String;
                        break;
                    case TopiValue.Tag.Enum when variable is EvtTopiEnum e:
                        Debug.Assert(e.Enum.Name == value.Enum.Name,
                            $"{name} is not instance of {e.Enum.Name}");
                        var enumValue = value.Enum.Value;
                        Debug.Assert(Array.Exists(e.Enum.Values, v => v == enumValue),
                            $"{e.Enum.Name} does not contain {enumValue}");
                        e.Value = value.Enum.Value;
                        break;
                }
            }

        }

        public async Task LoadTopiValues()
        {
            _aoList = Addressables.LoadAssetsAsync<EvtVariable>(
                new List<string> { "Topiary", "Evt" }, evt =>
                {
                    string topiName = null;
                    switch (evt)
                    {
                        case EvtTopiBool b:
                            topiName = b.Name;
                            _dialogue.Set(topiName, new TopiValue(b.Value));
                            _callbacks[topiName] =
                                new Action<bool>(v => _dialogue.Set(topiName, new TopiValue(v)));
                            b.OnEvt += (Action<bool>)_callbacks[topiName];
                            break;
                        case EvtTopiFloat f:
                            topiName = f.Name;
                            _dialogue.Set(topiName, new TopiValue(f.Value));
                            _callbacks[topiName] =
                                new Action<float>(v => _dialogue.Set(topiName, new TopiValue(v)));
                            f.OnEvt += (Action<float>)_callbacks[topiName];
                            break;
                        case EvtTopiInt i:
                            topiName = i.Name;
                            _dialogue.Set(topiName, new TopiValue(i.Value));
                            _callbacks[topiName] =
                                new Action<int>(v => _dialogue.Set(topiName, new TopiValue(v)));
                            i.OnEvt += (Action<int>)_callbacks[topiName];
                            break;
                        case EvtTopiString s:
                            topiName = s.Name;
                            _dialogue.Set(topiName, new TopiValue(s.Value));
                            _callbacks[topiName] =
                                new Action<string>(v => _dialogue.Set(topiName, new TopiValue(v)));
                            s.OnEvt += (Action<string>)_callbacks[topiName];
                            break;
                        case EvtTopiEnum e:
                            topiName = e.Name;
                            _dialogue.Set(topiName, new TopiValue(e.Enum.Name, e.Value));
                            _callbacks[topiName] =
                                new Action<string>(
                                    v => _dialogue.Set(topiName, new TopiValue(e.Enum.Name, v)));
                            e.OnEvt += (Action<string>)_callbacks[topiName];
                            break;
                    }

                    if (topiName == null || !_dialogue.Data.Externs.Contains(topiName))
                        return;
                    if (!_dialogue.Subscribe(topiName))
                    {
                        Dialogue.Log($"Could not Subscribe to {topiName}",
                            Library.Severity.Warn);
                        UnsubscribeEvt(topiName, evt);
                        return;
                    }

                    _variables[topiName] = evt;
                },
                Addressables.MergeMode.Intersection);
            await _aoList.Task;
        }

        public void UnloadTopiValues()
        {
            foreach (var kvp in _variables)
            {
                _dialogue.Unsubscribe(kvp.Key);
                UnsubscribeEvt(kvp.Key, kvp.Value);
            }

            if (_aoList.IsValid()) Addressables.Release(_aoList);
        }

        private void UnsubscribeEvt(string topiName, EvtVariable evt)
        {
            if (!_callbacks.TryGetValue(topiName, out var del)) return;
            switch (evt)
            {
                case EvtTopiBool b:
                    b.OnEvt -= (Action<bool>)del;
                    break;
                case EvtTopiFloat f:
                    f.OnEvt -= (Action<float>)del;
                    break;
                case EvtTopiInt i:
                    i.OnEvt -= (Action<int>)del;
                    break;
                case EvtTopiString s:
                    s.OnEvt -= (Action<string>)del;
                    break;
                case EvtTopiEnum e:
                    e.OnEvt -= (Action<string>)del;
                    break;
            }
        }
    }
}