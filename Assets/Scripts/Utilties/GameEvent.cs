using System;
using UnityEngine;

namespace Malyglut.CubitWorld.Utilties
{
    [CreateAssetMenu(fileName = "Game Event", menuName = "Cubit World/Game Event", order = 0)]
    public class GameEvent : ScriptableObject
    {
        private event Action OnRaiseParameterless;
        private event Action<object> OnRaiseParameter;

        public void Subscribe(Action listener)
        {
            OnRaiseParameterless += listener;
        }

        public void Unsubscribe(Action listener)
        {
            OnRaiseParameterless -= listener;
        }

        public void Raise()
        {
            OnRaiseParameterless?.Invoke();
        }
        
        public void Subscribe(Action<object> listener)
        {
            OnRaiseParameter += listener;
        }

        public void Unsubscribe(Action<object> listener)
        {
            OnRaiseParameter -= listener;
        }

        public void Raise(object parameter)
        {
            OnRaiseParameter?.Invoke(parameter);
        }
    }
}