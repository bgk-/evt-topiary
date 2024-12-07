using System.Collections.Generic;
using System.Threading.Tasks;
using PeartreeGames.Topiary.Unity;
using UnityEngine;

namespace PeartreeGames.Evt.Topiary
{
    public class EvtTopiManager : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            var go = new GameObject("EvtTopiManager");
            go.AddComponent<EvtTopiManager>();
            DontDestroyOnLoad(go);
        }
        
        private Dictionary<Dialogue, EvtTopiRegistry> _dict;

        private void Awake()
        {
            _dict = new Dictionary<Dialogue, EvtTopiRegistry>();
        }

        private void OnEnable()
        {
            Dialogue.OnStartBlocking += OnDialogueStart;
            Dialogue.OnEnd += OnDialogueEnd;
            Dialogue.OnValueChanged += OnDialogueValueChanged;
        }

        private void OnDisable()
        {
            Dialogue.OnStartBlocking -= OnDialogueStart;
            Dialogue.OnEnd -= OnDialogueEnd;
            Dialogue.OnValueChanged -= OnDialogueValueChanged;
        }


        private async Task OnDialogueStart(Dialogue dialogue)
        {
            var reg = new EvtTopiRegistry(dialogue);
            await reg.LoadTopiValues();
            _dict.Add(dialogue, reg);
        }
        
        private void OnDialogueEnd(Dialogue dialogue)
        {
            if (_dict.TryGetValue(dialogue, out var reg)) reg.UnloadTopiValues();
            _dict.Remove(dialogue);
        }
        
        private void OnDialogueValueChanged(Dialogue dialogue, string topiName, TopiValue value)
        {
            if (_dict.TryGetValue(dialogue, out var reg)) reg.OnValueChanged(topiName, value);
        }

    }
}