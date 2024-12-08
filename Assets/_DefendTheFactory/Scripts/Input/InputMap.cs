//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.11.2
//     from Assets/_DefendTheFactory/Input/InputMap.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @InputMap: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputMap()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputMap"",
    ""maps"": [
        {
            ""name"": ""MenuInput"",
            ""id"": ""d4469d69-1bff-454a-960e-5fabf12c3b10"",
            ""actions"": [
                {
                    ""name"": ""New action"",
                    ""type"": ""Button"",
                    ""id"": ""a91786a7-399f-4408-aab7-ae335e26064a"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""1469250c-42ac-4b0c-a896-77baab839bcb"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""New action"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""GameInput"",
            ""id"": ""de2a862b-8eb6-4a30-bce5-04f2f7ae26bb"",
            ""actions"": [
                {
                    ""name"": ""CameraMovement"",
                    ""type"": ""Value"",
                    ""id"": ""3c676878-5157-4a2b-bab7-c16ddaa73baf"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""CameraRotation"",
                    ""type"": ""Value"",
                    ""id"": ""5cc3a6b8-35fd-4dbc-a32d-6f943dbfe638"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""CameraZoom"",
                    ""type"": ""Value"",
                    ""id"": ""7140d0f4-37cd-4a42-8b20-0213fbb3373d"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": ""Normalize(min=-1,max=1)"",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""LeftClick"",
                    ""type"": ""Button"",
                    ""id"": ""8925807f-1a19-4e26-b7b6-528a7ab360f5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""RightClick"",
                    ""type"": ""Button"",
                    ""id"": ""3ecd7876-89c1-4935-8ee2-4598c78befbb"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ScrollClick"",
                    ""type"": ""Button"",
                    ""id"": ""10562648-e9cf-445c-be32-56b15ba1ab8b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""PointerPosition"",
                    ""type"": ""Value"",
                    ""id"": ""29ce2b52-47d0-40ff-b58e-7305375086e5"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Pouse"",
                    ""type"": ""Button"",
                    ""id"": ""d2f0b8fb-f961-4d42-941d-4a8f78224ab6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""TimeNormal"",
                    ""type"": ""Button"",
                    ""id"": ""2d998036-fce3-4ba9-9839-867ba4b35f2d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""TimeFast"",
                    ""type"": ""Button"",
                    ""id"": ""7ed7c962-5b81-46e3-bb71-81c81e0a2979"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""TimeExtraFast"",
                    ""type"": ""Button"",
                    ""id"": ""7c33c70c-05dc-4dae-8849-aa25b6adad53"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""BuildingMenu"",
                    ""type"": ""Button"",
                    ""id"": ""063b217b-9449-408f-9e0e-b5d0f452caa6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""BuildingRotation"",
                    ""type"": ""Button"",
                    ""id"": ""e5f4af99-4657-4f38-8387-9a54f1021317"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Back"",
                    ""type"": ""Button"",
                    ""id"": ""ddad131a-f331-4b42-a0cc-00460005bea2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""b127389d-f19d-4e1e-8082-1420ec179a76"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""WSAD"",
                    ""id"": ""43d1ee1f-4280-416b-8bc3-66c9d7c61199"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraMovement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""93ea7240-37b7-471b-a2f4-39d2e0c66c1e"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""2f95fe85-9df4-4fe2-a2e9-0da769853198"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""45785dc3-f1e8-455b-bf8b-81fe9b41af9a"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""b49e9ea2-956f-48d7-8f8a-224f2180af83"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Arrows"",
                    ""id"": ""ac33bc86-84c1-4032-99c7-c2d3b941987b"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraMovement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""528fc9ed-08e9-4639-9cfe-4562219ba091"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""17d348e4-8867-43a2-be08-0016a47ed681"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""2edcba33-078f-40ec-ac04-cfee57e23de2"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""454921f6-a3ab-457e-8871-59e138e71f06"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""5e669d20-7586-4a52-83f1-c5914d00d48e"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PointerPosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""536c38db-c5a3-4cc3-ab2b-d01328a3ad7b"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pouse"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""949c6c69-d607-45b2-8ce3-999c0e164156"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TimeNormal"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""RotationValue"",
                    ""id"": ""9715260e-c35a-4c8b-94e8-107dbfa0cd2a"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraRotation"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""0c2c3b7b-1b5b-414d-aff8-c75239ed4b50"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraRotation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""820863fe-2632-45b9-970e-fdd2f728debc"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraRotation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""20a5330c-abaa-4eee-b8d4-172eaee51e15"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TimeFast"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a98f2591-1960-4681-9a7f-2a5705a89132"",
                    ""path"": ""<Keyboard>/3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TimeExtraFast"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a0f20ee1-30b2-4cbe-a763-6d261a94dc30"",
                    ""path"": ""<Keyboard>/b"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""BuildingMenu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""be987415-da22-4c0d-b4b5-b1b68dce4e0a"",
                    ""path"": ""<Mouse>/scroll/y"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=90)"",
                    ""groups"": """",
                    ""action"": ""CameraZoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""ButtonZoom"",
                    ""id"": ""8449a03e-638b-4602-9ed6-41f803c5aad9"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraZoom"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""201bda85-d4c5-4f32-bb7a-194b6b648911"",
                    ""path"": ""<Keyboard>/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraZoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""96fd7e6e-99ed-449d-89de-f01a9bf73b4b"",
                    ""path"": ""<Keyboard>/z"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraZoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""0ac9080a-8e4f-4c7e-b3e2-b5de416a4c42"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RightClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f526fe97-617c-4b93-bc0f-38390baa1065"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""BuildingRotation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""605cf42b-253a-4b71-88f4-743ee3097ac5"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ScrollClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""df0b14cd-30c7-47d9-950c-ed0fe0cb9a72"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Back"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // MenuInput
        m_MenuInput = asset.FindActionMap("MenuInput", throwIfNotFound: true);
        m_MenuInput_Newaction = m_MenuInput.FindAction("New action", throwIfNotFound: true);
        // GameInput
        m_GameInput = asset.FindActionMap("GameInput", throwIfNotFound: true);
        m_GameInput_CameraMovement = m_GameInput.FindAction("CameraMovement", throwIfNotFound: true);
        m_GameInput_CameraRotation = m_GameInput.FindAction("CameraRotation", throwIfNotFound: true);
        m_GameInput_CameraZoom = m_GameInput.FindAction("CameraZoom", throwIfNotFound: true);
        m_GameInput_LeftClick = m_GameInput.FindAction("LeftClick", throwIfNotFound: true);
        m_GameInput_RightClick = m_GameInput.FindAction("RightClick", throwIfNotFound: true);
        m_GameInput_ScrollClick = m_GameInput.FindAction("ScrollClick", throwIfNotFound: true);
        m_GameInput_PointerPosition = m_GameInput.FindAction("PointerPosition", throwIfNotFound: true);
        m_GameInput_Pouse = m_GameInput.FindAction("Pouse", throwIfNotFound: true);
        m_GameInput_TimeNormal = m_GameInput.FindAction("TimeNormal", throwIfNotFound: true);
        m_GameInput_TimeFast = m_GameInput.FindAction("TimeFast", throwIfNotFound: true);
        m_GameInput_TimeExtraFast = m_GameInput.FindAction("TimeExtraFast", throwIfNotFound: true);
        m_GameInput_BuildingMenu = m_GameInput.FindAction("BuildingMenu", throwIfNotFound: true);
        m_GameInput_BuildingRotation = m_GameInput.FindAction("BuildingRotation", throwIfNotFound: true);
        m_GameInput_Back = m_GameInput.FindAction("Back", throwIfNotFound: true);
    }

    ~@InputMap()
    {
        UnityEngine.Debug.Assert(!m_MenuInput.enabled, "This will cause a leak and performance issues, InputMap.MenuInput.Disable() has not been called.");
        UnityEngine.Debug.Assert(!m_GameInput.enabled, "This will cause a leak and performance issues, InputMap.GameInput.Disable() has not been called.");
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

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // MenuInput
    private readonly InputActionMap m_MenuInput;
    private List<IMenuInputActions> m_MenuInputActionsCallbackInterfaces = new List<IMenuInputActions>();
    private readonly InputAction m_MenuInput_Newaction;
    public struct MenuInputActions
    {
        private @InputMap m_Wrapper;
        public MenuInputActions(@InputMap wrapper) { m_Wrapper = wrapper; }
        public InputAction @Newaction => m_Wrapper.m_MenuInput_Newaction;
        public InputActionMap Get() { return m_Wrapper.m_MenuInput; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MenuInputActions set) { return set.Get(); }
        public void AddCallbacks(IMenuInputActions instance)
        {
            if (instance == null || m_Wrapper.m_MenuInputActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_MenuInputActionsCallbackInterfaces.Add(instance);
            @Newaction.started += instance.OnNewaction;
            @Newaction.performed += instance.OnNewaction;
            @Newaction.canceled += instance.OnNewaction;
        }

        private void UnregisterCallbacks(IMenuInputActions instance)
        {
            @Newaction.started -= instance.OnNewaction;
            @Newaction.performed -= instance.OnNewaction;
            @Newaction.canceled -= instance.OnNewaction;
        }

        public void RemoveCallbacks(IMenuInputActions instance)
        {
            if (m_Wrapper.m_MenuInputActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IMenuInputActions instance)
        {
            foreach (var item in m_Wrapper.m_MenuInputActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_MenuInputActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public MenuInputActions @MenuInput => new MenuInputActions(this);

    // GameInput
    private readonly InputActionMap m_GameInput;
    private List<IGameInputActions> m_GameInputActionsCallbackInterfaces = new List<IGameInputActions>();
    private readonly InputAction m_GameInput_CameraMovement;
    private readonly InputAction m_GameInput_CameraRotation;
    private readonly InputAction m_GameInput_CameraZoom;
    private readonly InputAction m_GameInput_LeftClick;
    private readonly InputAction m_GameInput_RightClick;
    private readonly InputAction m_GameInput_ScrollClick;
    private readonly InputAction m_GameInput_PointerPosition;
    private readonly InputAction m_GameInput_Pouse;
    private readonly InputAction m_GameInput_TimeNormal;
    private readonly InputAction m_GameInput_TimeFast;
    private readonly InputAction m_GameInput_TimeExtraFast;
    private readonly InputAction m_GameInput_BuildingMenu;
    private readonly InputAction m_GameInput_BuildingRotation;
    private readonly InputAction m_GameInput_Back;
    public struct GameInputActions
    {
        private @InputMap m_Wrapper;
        public GameInputActions(@InputMap wrapper) { m_Wrapper = wrapper; }
        public InputAction @CameraMovement => m_Wrapper.m_GameInput_CameraMovement;
        public InputAction @CameraRotation => m_Wrapper.m_GameInput_CameraRotation;
        public InputAction @CameraZoom => m_Wrapper.m_GameInput_CameraZoom;
        public InputAction @LeftClick => m_Wrapper.m_GameInput_LeftClick;
        public InputAction @RightClick => m_Wrapper.m_GameInput_RightClick;
        public InputAction @ScrollClick => m_Wrapper.m_GameInput_ScrollClick;
        public InputAction @PointerPosition => m_Wrapper.m_GameInput_PointerPosition;
        public InputAction @Pouse => m_Wrapper.m_GameInput_Pouse;
        public InputAction @TimeNormal => m_Wrapper.m_GameInput_TimeNormal;
        public InputAction @TimeFast => m_Wrapper.m_GameInput_TimeFast;
        public InputAction @TimeExtraFast => m_Wrapper.m_GameInput_TimeExtraFast;
        public InputAction @BuildingMenu => m_Wrapper.m_GameInput_BuildingMenu;
        public InputAction @BuildingRotation => m_Wrapper.m_GameInput_BuildingRotation;
        public InputAction @Back => m_Wrapper.m_GameInput_Back;
        public InputActionMap Get() { return m_Wrapper.m_GameInput; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GameInputActions set) { return set.Get(); }
        public void AddCallbacks(IGameInputActions instance)
        {
            if (instance == null || m_Wrapper.m_GameInputActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_GameInputActionsCallbackInterfaces.Add(instance);
            @CameraMovement.started += instance.OnCameraMovement;
            @CameraMovement.performed += instance.OnCameraMovement;
            @CameraMovement.canceled += instance.OnCameraMovement;
            @CameraRotation.started += instance.OnCameraRotation;
            @CameraRotation.performed += instance.OnCameraRotation;
            @CameraRotation.canceled += instance.OnCameraRotation;
            @CameraZoom.started += instance.OnCameraZoom;
            @CameraZoom.performed += instance.OnCameraZoom;
            @CameraZoom.canceled += instance.OnCameraZoom;
            @LeftClick.started += instance.OnLeftClick;
            @LeftClick.performed += instance.OnLeftClick;
            @LeftClick.canceled += instance.OnLeftClick;
            @RightClick.started += instance.OnRightClick;
            @RightClick.performed += instance.OnRightClick;
            @RightClick.canceled += instance.OnRightClick;
            @ScrollClick.started += instance.OnScrollClick;
            @ScrollClick.performed += instance.OnScrollClick;
            @ScrollClick.canceled += instance.OnScrollClick;
            @PointerPosition.started += instance.OnPointerPosition;
            @PointerPosition.performed += instance.OnPointerPosition;
            @PointerPosition.canceled += instance.OnPointerPosition;
            @Pouse.started += instance.OnPouse;
            @Pouse.performed += instance.OnPouse;
            @Pouse.canceled += instance.OnPouse;
            @TimeNormal.started += instance.OnTimeNormal;
            @TimeNormal.performed += instance.OnTimeNormal;
            @TimeNormal.canceled += instance.OnTimeNormal;
            @TimeFast.started += instance.OnTimeFast;
            @TimeFast.performed += instance.OnTimeFast;
            @TimeFast.canceled += instance.OnTimeFast;
            @TimeExtraFast.started += instance.OnTimeExtraFast;
            @TimeExtraFast.performed += instance.OnTimeExtraFast;
            @TimeExtraFast.canceled += instance.OnTimeExtraFast;
            @BuildingMenu.started += instance.OnBuildingMenu;
            @BuildingMenu.performed += instance.OnBuildingMenu;
            @BuildingMenu.canceled += instance.OnBuildingMenu;
            @BuildingRotation.started += instance.OnBuildingRotation;
            @BuildingRotation.performed += instance.OnBuildingRotation;
            @BuildingRotation.canceled += instance.OnBuildingRotation;
            @Back.started += instance.OnBack;
            @Back.performed += instance.OnBack;
            @Back.canceled += instance.OnBack;
        }

        private void UnregisterCallbacks(IGameInputActions instance)
        {
            @CameraMovement.started -= instance.OnCameraMovement;
            @CameraMovement.performed -= instance.OnCameraMovement;
            @CameraMovement.canceled -= instance.OnCameraMovement;
            @CameraRotation.started -= instance.OnCameraRotation;
            @CameraRotation.performed -= instance.OnCameraRotation;
            @CameraRotation.canceled -= instance.OnCameraRotation;
            @CameraZoom.started -= instance.OnCameraZoom;
            @CameraZoom.performed -= instance.OnCameraZoom;
            @CameraZoom.canceled -= instance.OnCameraZoom;
            @LeftClick.started -= instance.OnLeftClick;
            @LeftClick.performed -= instance.OnLeftClick;
            @LeftClick.canceled -= instance.OnLeftClick;
            @RightClick.started -= instance.OnRightClick;
            @RightClick.performed -= instance.OnRightClick;
            @RightClick.canceled -= instance.OnRightClick;
            @ScrollClick.started -= instance.OnScrollClick;
            @ScrollClick.performed -= instance.OnScrollClick;
            @ScrollClick.canceled -= instance.OnScrollClick;
            @PointerPosition.started -= instance.OnPointerPosition;
            @PointerPosition.performed -= instance.OnPointerPosition;
            @PointerPosition.canceled -= instance.OnPointerPosition;
            @Pouse.started -= instance.OnPouse;
            @Pouse.performed -= instance.OnPouse;
            @Pouse.canceled -= instance.OnPouse;
            @TimeNormal.started -= instance.OnTimeNormal;
            @TimeNormal.performed -= instance.OnTimeNormal;
            @TimeNormal.canceled -= instance.OnTimeNormal;
            @TimeFast.started -= instance.OnTimeFast;
            @TimeFast.performed -= instance.OnTimeFast;
            @TimeFast.canceled -= instance.OnTimeFast;
            @TimeExtraFast.started -= instance.OnTimeExtraFast;
            @TimeExtraFast.performed -= instance.OnTimeExtraFast;
            @TimeExtraFast.canceled -= instance.OnTimeExtraFast;
            @BuildingMenu.started -= instance.OnBuildingMenu;
            @BuildingMenu.performed -= instance.OnBuildingMenu;
            @BuildingMenu.canceled -= instance.OnBuildingMenu;
            @BuildingRotation.started -= instance.OnBuildingRotation;
            @BuildingRotation.performed -= instance.OnBuildingRotation;
            @BuildingRotation.canceled -= instance.OnBuildingRotation;
            @Back.started -= instance.OnBack;
            @Back.performed -= instance.OnBack;
            @Back.canceled -= instance.OnBack;
        }

        public void RemoveCallbacks(IGameInputActions instance)
        {
            if (m_Wrapper.m_GameInputActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IGameInputActions instance)
        {
            foreach (var item in m_Wrapper.m_GameInputActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_GameInputActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public GameInputActions @GameInput => new GameInputActions(this);
    public interface IMenuInputActions
    {
        void OnNewaction(InputAction.CallbackContext context);
    }
    public interface IGameInputActions
    {
        void OnCameraMovement(InputAction.CallbackContext context);
        void OnCameraRotation(InputAction.CallbackContext context);
        void OnCameraZoom(InputAction.CallbackContext context);
        void OnLeftClick(InputAction.CallbackContext context);
        void OnRightClick(InputAction.CallbackContext context);
        void OnScrollClick(InputAction.CallbackContext context);
        void OnPointerPosition(InputAction.CallbackContext context);
        void OnPouse(InputAction.CallbackContext context);
        void OnTimeNormal(InputAction.CallbackContext context);
        void OnTimeFast(InputAction.CallbackContext context);
        void OnTimeExtraFast(InputAction.CallbackContext context);
        void OnBuildingMenu(InputAction.CallbackContext context);
        void OnBuildingRotation(InputAction.CallbackContext context);
        void OnBack(InputAction.CallbackContext context);
    }
}
