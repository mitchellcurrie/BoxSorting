using UnityEngine;

namespace Core
{
    public class Application : MonoBehaviour
    {
        private void Start()
        {
            UnityEngine.Application.runInBackground = true;
        }
    }
}
