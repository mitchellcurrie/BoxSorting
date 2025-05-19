using FSM.Data;
using UnityEngine;

namespace UI
{
    public class HUD : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI _currentState;
        [SerializeField] private TMPro.TextMeshProUGUI _previousState;

        private void Awake()
        {
            if (!_currentState || !_previousState)
            {
                Debug.LogError("State text is not set on the HUD");
            }
        }
        public void UpdateStateText(StateName newState)
        {
            _previousState.text = _currentState.text;
            _currentState.text = newState.ToString();
        }
    }
}
