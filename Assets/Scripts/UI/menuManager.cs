using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class menuManager : MonoBehaviour
{
    private bool menu_on = false;
    private UIDocument m_uiDocument;
    private VisualElement menu_base;
    public static menuManager Instance;

    [SerializeField]
    private InputActionAsset asset;
    [SerializeField]
    private VisualTreeAsset list_template;

    public event Action<string> onActionMapSwich;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        m_uiDocument = gameObject.GetComponent<UIDocument>();
       
        menu_base = m_uiDocument.rootVisualElement.Q<VisualElement>("window"); 
        pop = m_uiDocument.rootVisualElement.Q<VisualElement>("Popup");
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        LoadOverrides();
    }
    public void MenuButton()
    {
        if(menu_on)
        {
            close_menu();
        }
        else 
        {
            open_menu();
        }
        menu_on=!menu_on;   
    }

    private void open_menu()
    {
        Time.timeScale= 0f; // Pause the game
        menu_base.RemoveFromClassList("hidden");
        DisplayControl();
    }

    private void Bindig_menu(InputActionMap map)
    {
        ScrollView scrollView= menu_base.Q<ScrollView>("controlArea");
        // Clear the scroll view content container explicitly to avoid leftover
        // elements when reopening the menu.
        scrollView.contentContainer.Clear();
        foreach (var action in map.actions)
        {
            list_template.CloneTree(scrollView.contentContainer);
            VisualElement element = scrollView.ElementAt(scrollView.contentContainer.childCount-1);
            element.dataSource = action;
            Button RemapButton = element.Q<Button>("remap");
            RemapButton.clicked +=()=>{ OnRemapButtonPress(action); };
        }

        onActionMapSwich?.Invoke(map.name);
    }
    private InputActionRebindingExtensions.RebindingOperation rebind_opera_kebord;
    private InputActionRebindingExtensions.RebindingOperation rebind_opera_gamepad;
    private VisualElement pop;
    private void OnRemapButtonPress(InputAction action)
    {
       
        pop.RemoveFromClassList("hidden");
        action.Disable();
        //rebind keybord
        rebind_opera_kebord = action.PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse")
            .WithTargetBinding(0)
            .OnMatchWaitForAnother(0.1f);
        rebind_opera_kebord.Start();
        rebind_opera_kebord.OnComplete(OnRebindComplete);
        
        //rebind gamepad
        rebind_opera_gamepad= action.PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse")
            .WithTargetBinding(1)
            .OnMatchWaitForAnother(0.1f);
        rebind_opera_gamepad.Start();
        rebind_opera_gamepad.OnComplete(OnRebindComplete);


    }

    private void OnRebindComplete(InputActionRebindingExtensions.RebindingOperation operation)
    {   pop.AddToClassList("hidden");
        DisplayControl();
        
        operation.action.Enable();
        rebind_opera_gamepad.Cancel();
        rebind_opera_kebord.Cancel();
        SaveBinding();
    }

    void DisplayControl()
    {
        ToggleButtonGroup toggleGroup = menu_base.Q<ToggleButtonGroup>("controlPreset");
        IEnumerable<InputActionMap> maps= asset.actionMaps.ToArray().Where(x=>x.name.Contains("Gameplay"));

        // Remove previously added buttons so we don't duplicate them on each
        // call (DisplayControl can be invoked multiple times e.g. after rebinding).
        toggleGroup.Clear();

        foreach(InputActionMap map in maps)
        {
            Button button= new Button(()=> { Bindig_menu(map);  });
            button.text = map.name;
            button.AddToClassList("controlPresetButton");
            toggleGroup.Add(button);
        }

        // Ensure the tabIndex is within range before selecting the initial map.
        int mapsCount = maps.Count();
        int index = Mathf.Clamp(toggleGroup.tabIndex, 0, Math.Max(0, mapsCount - 1));
        if (mapsCount > 0)
            Bindig_menu(maps.ElementAt(index));
    }

    private void close_menu()
    {
        Time.timeScale = 1f; // unPause the game
        menu_base.AddToClassList("hidden");
    }
    private void LoadOverrides()
    {
        string path = string.Concat(Application.persistentDataPath, "/bindings.json/controls.json");
        if (File.Exists(path))
        {
            string overide= File.ReadAllText(path);
            asset.LoadBindingOverridesFromJson(overide);
        }
     }

    private void SaveBinding()
    {
        string overrides = asset.SaveBindingOverridesAsJson();
        string path= string.Concat(Application.persistentDataPath,"/bindings.json");

        if(!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        File.WriteAllText(string.Concat(path, "/controls.json" ),overrides);
    }



}
