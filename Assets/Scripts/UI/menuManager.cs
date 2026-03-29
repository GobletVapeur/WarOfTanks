using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class menuManager : MonoBehaviour
{
    private bool menu_on = false;
    private UIDocument m_uiDocument;
    private VisualElement menu_base;
    public static menuManager instance;

    [SerializeField]
    private InputActionAsset asset;
    [SerializeField]
    private VisualTreeAsset list_template;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        m_uiDocument = gameObject.GetComponent<UIDocument>();
        menu_base= m_uiDocument.rootVisualElement.Q<VisualElement>("window");
        instance = this;
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
        Bindig_menu();
    }

    private void Bindig_menu()
    {
    ScrollView scrollView= menu_base.Q<ScrollView>("controlArea");
    scrollView.Clear();
        InputActionMap map = asset.FindActionMap("Gameplay");

        foreach (var action in map.actions)
        {
            list_template.CloneTree(scrollView.contentContainer);
            VisualElement element = scrollView.ElementAt(scrollView.contentContainer.childCount-1);
            element.dataSource = action;
        }
    }

    private void close_menu()
    {
        Time.timeScale = 1f; // unPause the game
        menu_base.AddToClassList("hidden");
    }



}
