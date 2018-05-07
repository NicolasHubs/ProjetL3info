using System;
using System.Collections.Generic;
using UnityEngine;


namespace TheNewWorld.CrossPlatformInput
{
    public abstract class VirtualInput
    {
        public Vector3 virtualMousePosition { get; private set; }


        protected Dictionary<string, CrossPlatformInputManager.VirtualButton> m_VirtualButtons =
            new Dictionary<string, CrossPlatformInputManager.VirtualButton>();
        protected List<string> m_AlwaysUseVirtual = new List<string>();
            // list of the axis and button names that have been flagged to always use a virtual axis or button
        

        public bool ButtonExists(string name)
        {
            return m_VirtualButtons.ContainsKey(name);
        }


        public void RegisterVirtualButton(CrossPlatformInputManager.VirtualButton button)
        {
            // check if already have a buttin with that name and log an error if we do
            if (m_VirtualButtons.ContainsKey(button.name))
            {
                Debug.LogError("There is already a virtual button named " + button.name + " registered.");
            }
            else
            {
                // add any new buttons
                m_VirtualButtons.Add(button.name, button);

                // if we dont want to match to the input manager then always use a virtual axis
                if (!button.matchWithInputManager)
                {
                    m_AlwaysUseVirtual.Add(button.name);
                }
            }
        }


        public void UnRegisterVirtualButton(string name)
        {
            // if we have a button with this name then remove it from our dictionary of registered buttons
            if (m_VirtualButtons.ContainsKey(name))
            {
                m_VirtualButtons.Remove(name);
            }
        }


        public void SetVirtualMousePositionX(float f)
        {
            virtualMousePosition = new Vector3(f, virtualMousePosition.y, virtualMousePosition.z);
        }


        public void SetVirtualMousePositionY(float f)
        {
            virtualMousePosition = new Vector3(virtualMousePosition.x, f, virtualMousePosition.z);
        }


        public void SetVirtualMousePositionZ(float f)
        {
            virtualMousePosition = new Vector3(virtualMousePosition.x, virtualMousePosition.y, f);
        }


        public abstract float GetAxis(string name, bool raw);
        
        public abstract bool GetButton(string name);
        public abstract bool GetButtonDown(string name);
        public abstract bool GetButtonUp(string name);

        public abstract void SetButtonDown(string name);
        public abstract void SetButtonUp(string name);
        public abstract void SetAxisPositive(string name);
        public abstract void SetAxisNegative(string name);
        public abstract void SetAxisZero(string name);
        public abstract void SetAxis(string name, float value);
        public abstract Vector3 MousePosition();
    }
}
