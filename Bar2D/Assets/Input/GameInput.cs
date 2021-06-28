// GENERATED AUTOMATICALLY FROM 'Assets/Input/GameInput.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @GameInput : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @GameInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""GameInput"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""3882a78a-abc4-441a-98b8-389ece89b389"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""f48e98c7-2e6c-4afd-958a-9d13b84ac82e"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""DropLeft"",
                    ""type"": ""Button"",
                    ""id"": ""da8870f1-bc1c-48dd-b57b-ea9eb7f32047"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""DropRight"",
                    ""type"": ""Button"",
                    ""id"": ""4bd2c539-6580-438f-a3b7-a50fbfcfb0c3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ChangeMode"",
                    ""type"": ""Button"",
                    ""id"": ""e3737cf9-65ee-40a9-9953-a4aff5664032"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""OpenBartendingBook"",
                    ""type"": ""Button"",
                    ""id"": ""0dd7c257-6831-4735-b8ea-da445c7d5f82"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Scroll"",
                    ""type"": ""Value"",
                    ""id"": ""424da225-33ee-4a06-a1cd-fc17e96f8518"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""e162215a-c621-4de7-9ebb-dcf8cd864384"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""22f715d8-0ad9-4e6f-902f-c5ce36f6d1e7"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""65b089d0-4dc2-4953-bf47-c0af24805d5c"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""0b8b9991-73cb-4928-96cc-c264eb1b46b9"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""f1f39bb8-d864-4c9b-98b0-96e0b2384293"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Arrows"",
                    ""id"": ""ecc9d1a3-03aa-4139-82a6-51810adf6b8b"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""13916c8f-9872-42c5-92ae-75b2f2382733"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""b2cdc84f-4695-4e7e-8e17-d490cbf0d92a"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""02a51131-1408-4051-bc21-41063b8a7c3c"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""679a6eeb-8b58-4025-b7d8-ca3bce62b350"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""d41725ee-c64f-4260-b560-cd6da447043b"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DropLeft"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bcef2070-0a2c-4558-8a2f-67e8b003ee9b"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DropRight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bc4c95b8-06e1-4769-9424-87dc426d9ea2"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ChangeMode"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a56c3140-b69d-46cd-83a0-413a67a72064"",
                    ""path"": ""<Keyboard>/tab"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""OpenBartendingBook"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c49870cc-aef3-4663-a64d-747b877484e9"",
                    ""path"": ""<Mouse>/scroll/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Scroll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard"",
            ""bindingGroup"": ""Keyboard"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Movement = m_Player.FindAction("Movement", throwIfNotFound: true);
        m_Player_DropLeft = m_Player.FindAction("DropLeft", throwIfNotFound: true);
        m_Player_DropRight = m_Player.FindAction("DropRight", throwIfNotFound: true);
        m_Player_ChangeMode = m_Player.FindAction("ChangeMode", throwIfNotFound: true);
        m_Player_OpenBartendingBook = m_Player.FindAction("OpenBartendingBook", throwIfNotFound: true);
        m_Player_Scroll = m_Player.FindAction("Scroll", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_Movement;
    private readonly InputAction m_Player_DropLeft;
    private readonly InputAction m_Player_DropRight;
    private readonly InputAction m_Player_ChangeMode;
    private readonly InputAction m_Player_OpenBartendingBook;
    private readonly InputAction m_Player_Scroll;
    public struct PlayerActions
    {
        private @GameInput m_Wrapper;
        public PlayerActions(@GameInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_Player_Movement;
        public InputAction @DropLeft => m_Wrapper.m_Player_DropLeft;
        public InputAction @DropRight => m_Wrapper.m_Player_DropRight;
        public InputAction @ChangeMode => m_Wrapper.m_Player_ChangeMode;
        public InputAction @OpenBartendingBook => m_Wrapper.m_Player_OpenBartendingBook;
        public InputAction @Scroll => m_Wrapper.m_Player_Scroll;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                @DropLeft.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDropLeft;
                @DropLeft.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDropLeft;
                @DropLeft.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDropLeft;
                @DropRight.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDropRight;
                @DropRight.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDropRight;
                @DropRight.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDropRight;
                @ChangeMode.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnChangeMode;
                @ChangeMode.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnChangeMode;
                @ChangeMode.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnChangeMode;
                @OpenBartendingBook.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnOpenBartendingBook;
                @OpenBartendingBook.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnOpenBartendingBook;
                @OpenBartendingBook.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnOpenBartendingBook;
                @Scroll.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnScroll;
                @Scroll.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnScroll;
                @Scroll.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnScroll;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @DropLeft.started += instance.OnDropLeft;
                @DropLeft.performed += instance.OnDropLeft;
                @DropLeft.canceled += instance.OnDropLeft;
                @DropRight.started += instance.OnDropRight;
                @DropRight.performed += instance.OnDropRight;
                @DropRight.canceled += instance.OnDropRight;
                @ChangeMode.started += instance.OnChangeMode;
                @ChangeMode.performed += instance.OnChangeMode;
                @ChangeMode.canceled += instance.OnChangeMode;
                @OpenBartendingBook.started += instance.OnOpenBartendingBook;
                @OpenBartendingBook.performed += instance.OnOpenBartendingBook;
                @OpenBartendingBook.canceled += instance.OnOpenBartendingBook;
                @Scroll.started += instance.OnScroll;
                @Scroll.performed += instance.OnScroll;
                @Scroll.canceled += instance.OnScroll;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);
    private int m_KeyboardSchemeIndex = -1;
    public InputControlScheme KeyboardScheme
    {
        get
        {
            if (m_KeyboardSchemeIndex == -1) m_KeyboardSchemeIndex = asset.FindControlSchemeIndex("Keyboard");
            return asset.controlSchemes[m_KeyboardSchemeIndex];
        }
    }
    public interface IPlayerActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnDropLeft(InputAction.CallbackContext context);
        void OnDropRight(InputAction.CallbackContext context);
        void OnChangeMode(InputAction.CallbackContext context);
        void OnOpenBartendingBook(InputAction.CallbackContext context);
        void OnScroll(InputAction.CallbackContext context);
    }
}
