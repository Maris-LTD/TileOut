namespace GameModules.CameraSystem.Core
{
    using UnityEngine;

    public static class CameraInputUtility
    {
#if ENABLE_INPUT_SYSTEM
        private static UnityEngine.InputSystem.Mouse Mouse => UnityEngine.InputSystem.Mouse.current;
#endif

        public static Vector2 GetMouseDelta()
        {
#if ENABLE_INPUT_SYSTEM
            return Mouse != null ? Mouse.delta.ReadValue() : Vector2.zero;
#else
            return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
#endif
        }

        public static float GetScrollDelta()
        {
#if ENABLE_INPUT_SYSTEM
            return Mouse != null ? Mouse.scroll.ReadValue().y : 0f;
#else
            return Input.mouseScrollDelta.y;
#endif
        }

        public static bool GetMouseButton(int buttonIndex)
        {
#if ENABLE_INPUT_SYSTEM
            if (Mouse == null)
            {
                return false;
            }

            return buttonIndex switch
            {
                0 => Mouse.leftButton.isPressed,
                1 => Mouse.rightButton.isPressed,
                2 => Mouse.middleButton.isPressed,
                _ => false
            };
#else
            return Input.GetMouseButton(buttonIndex);
#endif
        }
    }
}

