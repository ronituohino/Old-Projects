using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Handle back-end of the controlpanel, hiding windows, window settings and whatnot
public class ControlPanelManager : Singleton<ControlPanelManager>
{
    [HideInInspector]
    public List<Tab> tabs = new List<Tab>();

    [SerializeField] GameObject tabObject;
    [SerializeField] Transform tabsParent;

    [Space]

    public List<Page> controlPanelPages = new List<Page>();
    public int currentPage;

    //Create a tab for a window
    public Tab AddTabSingle(Window window)
    {
        GameObject g = Instantiate(tabObject, tabsParent);

        int count = tabs.Count;
        RectTransform rc = g.GetComponent<RectTransform>();
        rc.anchoredPosition = new Vector2(0, count * -50);

        Image i = g.GetComponent<Image>();
        i.sprite = window.tabSprite;

        Tab t = g.GetComponent<Tab>();
        t.windowToOpen = window;
        t.windowSettings = GenerateWindowDefaultSettings(controlPanelPages.Count);

        ApplySettings(t, FetchSettings(t));

        tabs.Add(t);
        return t;
    }

    //Create a tab that contains subtabs for windows
    public Tab AddTabMultiple(List<Window> windows, Sprite parentTabSprite)
    {
        GameObject g = Instantiate(tabObject, tabsParent);
        Image i = g.GetComponent<Image>();
        i.sprite = parentTabSprite;

        Tab t = g.GetComponent<Tab>();
        t.hasSubTabs = true;
        t.subTabs = new List<Tab>();
        t.windowSettings = GenerateWindowDefaultSettings(controlPanelPages.Count);

        foreach (Window p in windows)
        {
            GameObject g2 = Instantiate(tabObject);
            Image i2 = g2.GetComponent<Image>();
            i2.sprite = p.tabSprite;

            Tab subTab = g2.AddComponent<Tab>();
            subTab.windowToOpen = p;
            subTab.windowSettings = GenerateWindowDefaultSettings(controlPanelPages.Count);

            ApplySettings(subTab, FetchSettings(subTab));

            t.subTabs.Add(subTab);
        }

        tabs.Add(t);
        return t;
    }

    public void RemoveTab()
    {

    }


    //Reads settings from memory
    public Tab.WindowSettings FetchSettings(Tab t)
    {
        return t.windowSettings[currentPage];
    }

    //Updates new settings to memory
    public void UpdateSettings(Tab t, Tab.WindowSettings settings)
    {
        t.windowSettings[currentPage] = settings;
    }

    //Applies settings to the window object
    public void ApplySettings(Tab t, Tab.WindowSettings settings)
    {
        t.windowToOpen.gameObject.SetActive(settings.enabled);
        t.windowToOpen.windowTransform.anchoredPosition = settings.location;
        t.windowToOpen.windowTransform.sizeDelta = new Vector2(t.windowToOpen.defaultSize.x * settings.scale.x, t.windowToOpen.defaultSize.y * settings.scale.y);
    }

    //Switches a window on/off
    public void ToggleWindow(Tab t)
    {
        Tab.WindowSettings settings = FetchSettings(t);
        settings.enabled = !settings.enabled;
        t.windowToOpen.MakeWindowOnTop();

        UpdateSettings(t, settings);

        if(!t.hasSubTabs)
        {
            ApplySettings(t, settings);
        }
        else
        {

        }
    }

    
    public void SwitchPage(Page p)
    {
        int count = controlPanelPages.Count;
        bool validPage = false;

        for (int i = 0; i < count; i++)
        {
            if (controlPanelPages[i] == p)
            {
                currentPage = i;
                validPage = true;
                break;
            }
        }

        if(validPage)
        {
            //Close and open windows
            foreach(Tab tab in tabs)
            {
                Tab.WindowSettings settings = FetchSettings(tab);
                if(!tab.hasSubTabs)
                {
                    ApplySettings(tab, settings);
                }
            }
        }
        else
        {
            Debug.Log("Error switching page!");
            return;
        }
    }



    public List<Tab.WindowSettings> GenerateWindowDefaultSettings(int amountOfCopies)
    {
        Tab.WindowSettings settings = new Tab.WindowSettings();
        settings.enabled = false;
        settings.location = Vector2.zero;
        settings.scale = new Vector2(1, 1);

        List<Tab.WindowSettings> settingsList = new List<Tab.WindowSettings>();
        for (int i = 0; i < amountOfCopies; i++)
        {
            settingsList.Add(settings);
        }

        return settingsList;
    }
}