using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BattleManagement
{
    class MouseCursorManagement : MonoBehaviour
    {
        public static MouseCursorManagement Instance;
        public Texture2D cursorTexture;

        public CursorMode cursorMode = CursorMode.ForceSoftware;
        public Vector2 hotSpot = Vector2.zero;

        public MouseCursorManagement()
        {
            Instance = Instance == null ? this : null;
        }

        public void SetMeelAtackIcon()
        {
            Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
        }

        public void DefaultCursor()
        {
            Cursor.SetCursor(null, Vector2.zero, cursorMode);
        }
    }
}
