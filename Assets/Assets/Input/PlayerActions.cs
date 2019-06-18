// GENERATED AUTOMATICALLY FROM 'Assets/Input/PlayerActions.inputactions'

using System;
using UnityEngine;
using UnityEngine.Experimental.Input;


[Serializable]
public class PlayerActions : InputActionAssetReference
{
    public PlayerActions()
    {
    }
    public PlayerActions(InputActionAsset asset)
        : base(asset)
    {
    }
    [NonSerialized] private bool m_Initialized;
    private void Initialize()
    {
        // Character Action
        m_CharacterAction = asset.GetActionMap("Character Action");
        m_CharacterAction_Move = m_CharacterAction.GetAction("Move");
        m_CharacterAction_Action = m_CharacterAction.GetAction("Action");
        m_Initialized = true;
    }
    private void Uninitialize()
    {
        m_CharacterAction = null;
        m_CharacterAction_Move = null;
        m_CharacterAction_Action = null;
        m_Initialized = false;
    }
    public void SetAsset(InputActionAsset newAsset)
    {
        if (newAsset == asset) return;
        if (m_Initialized) Uninitialize();
        asset = newAsset;
    }
    public override void MakePrivateCopyOfActions()
    {
        SetAsset(ScriptableObject.Instantiate(asset));
    }
    // Character Action
    private InputActionMap m_CharacterAction;
    private InputAction m_CharacterAction_Move;
    private InputAction m_CharacterAction_Action;
    public struct CharacterActionActions
    {
        private PlayerActions m_Wrapper;
        public CharacterActionActions(PlayerActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move { get { return m_Wrapper.m_CharacterAction_Move; } }
        public InputAction @Action { get { return m_Wrapper.m_CharacterAction_Action; } }
        public InputActionMap Get() { return m_Wrapper.m_CharacterAction; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled { get { return Get().enabled; } }
        public InputActionMap Clone() { return Get().Clone(); }
        public static implicit operator InputActionMap(CharacterActionActions set) { return set.Get(); }
    }
    public CharacterActionActions @CharacterAction
    {
        get
        {
            if (!m_Initialized) Initialize();
            return new CharacterActionActions(this);
        }
    }
    private int m_GamepadSchemeIndex = -1;
    public InputControlScheme GamepadScheme
    {
        get

        {
            if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.GetControlSchemeIndex("Gamepad");
            return asset.controlSchemes[m_GamepadSchemeIndex];
        }
    }
}
