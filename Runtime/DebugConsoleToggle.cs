using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItsPeetah.DebugConsole.Extra
{
    public class DebugConsoleToggle : MonoBehaviour
    {
        [SerializeField] private KeyCode toggleKey = KeyCode.Escape;

        private void Update()
        {
            if (Input.GetKeyDown(toggleKey))
            {
                DebugConsole.Main.Active = !DebugConsole.Main.Active;
            }
        }
    }
}