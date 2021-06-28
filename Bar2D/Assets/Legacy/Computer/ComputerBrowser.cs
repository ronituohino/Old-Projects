using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ComputerBrowser : Singleton<ComputerBrowser>
{
    [SerializeField] RectTransform computerScreen;

    public List<Website> registeredSites = new List<Website>();

    Website startPage;
    int siteCount;

    public List<TabInfo> tabs { get; set; } = new List<TabInfo>();
    public TabInfo currentTab { get; private set; }

    [Space]

    [SerializeField] Transform tabParent;
    [SerializeField] GameObject tabObject;

    [SerializeField] int firstTabOffset;
    [SerializeField] int tabWidth;
    [SerializeField] int spaceBetweenTabs;

    [SerializeField] float newTabOffset;
    [SerializeField] float tabYPosition;

    [SerializeField] RectTransform newTabRect;

    [Space]

    [SerializeField] TextMeshProUGUI urlText;
    [SerializeField] RectTransform urlRect;

    [SerializeField] RectTransform blinker;
    [SerializeField] float writeCharacterBlinkTime;
    float blinkTimer = 0f;
    bool showingBlink = false;

    [SerializeField] ScrollBarHandle scrollBarHandle;

    public enum BrowseType
    {
        Regular,
        Return,
        UndoReturn,
        SwitchTab
    }

    [SerializeField] float spawnHeightAdd;

    void Start()
    {
        //Hide all other pages and start on the home page
        siteCount = registeredSites.Count;
        for (int i = 1; i < siteCount; i++)
        {
            registeredSites[i].siteObject.SetActive(false);
        }

        startPage = registeredSites[0];
        AddTab();
    }

    public void BrowseWithURL(string url)
    {
        bool found = false;
        foreach (Website w in registeredSites)
        {
            if (w.url == url)
            {
                Browse(w, BrowseType.Regular);
                found = true;
                break;
            }
        }

        if(!found)
        {
            //Page not found website
            Debug.Log("Page not found!");
        }
    }

    #region BROWSING
    //Called when the user clicks a link
    public void Browse(Website w, BrowseType browseType)
    {
        //Manage browsing history
        if (currentTab.siteHistoryCount > 0 && browseType != BrowseType.SwitchTab)
        {
            currentTab.currentWebsite.siteObject.SetActive(false);
        }

        switch (browseType)
        {
            case BrowseType.Regular:
                //We browsed after we returned to a previous site, cut some of the sites off
                if (currentTab.historyPointer < currentTab.siteHistoryCount)
                {
                    int amountOfSitesToRemove = currentTab.siteHistoryCount - currentTab.historyPointer;
                    for (int i = 0; i < amountOfSitesToRemove; i++)
                    {
                        currentTab.siteHistory.RemoveAt(currentTab.historyPointer);
                    }
                }

                currentTab.siteHistory.Add(w);
                currentTab.siteHistoryCount = currentTab.siteHistory.Count;
                currentTab.historyPointer = currentTab.siteHistoryCount;
                break;

            case BrowseType.Return:
                currentTab.historyPointer--;
                break;

            case BrowseType.UndoReturn:
                currentTab.historyPointer++;
                break;
            default:
                break;
        }

        //Show new site
        w.siteObject.SetActive(true);
        urlText.text = w.url;

        currentTab.headerText.text = w.headerText;
        currentTab.currentWebsite = w;

        if(browseType != BrowseType.SwitchTab)
        {
            scrollBarHandle.SetPageAndHandle(0f);
        } 
        else
        {
            scrollBarHandle.SetPageAndHandle(currentTab.percentagePageScrolled);
        }
    }

    //Return to previous page
    public void Return()
    {
        Website previous = currentTab.siteHistory[currentTab.historyPointer - 2];
        Browse(previous, BrowseType.Return);
    }

    //Go back to page that we returned from
    public void UndoReturn()
    {
        Website undoPrevious = currentTab.siteHistory[currentTab.historyPointer];
        Browse(undoPrevious, BrowseType.UndoReturn);
    }

    public void ReturnToHomePage()
    {
        Browse(registeredSites[0], BrowseType.Regular);
    }

    public void EnterURLWriteMode()
    {
        //InputManager.Instance.inKeyboardReadMode = true;

        showingBlink = true;
        blinkTimer = 0f;
        blinker.gameObject.SetActive(showingBlink);

        //InputManager.Instance.mouseClickedEvent += ExitURLWriteMode;
    }

    //Called from mouseClickedEvent in InputManager.cs
    void ExitURLWriteMode()
    {
        //InputManager.Instance.inKeyboardReadMode = false;

        showingBlink = false;
        blinkTimer = 0f;
        blinker.gameObject.SetActive(showingBlink);

        //InputManager.Instance.mouseClickedEvent -= ExitURLWriteMode;
    }

    public void AddTab()
    {
        GameObject obj = Instantiate(tabObject, tabParent);
        TabInfo newTab = obj.GetComponent<TabInfo>();
        RectTransform rect = newTab.rect;

        Vector2 tabPos = new Vector2(firstTabOffset + tabs.Count * (spaceBetweenTabs + tabWidth), 3f);
        rect.anchoredPosition = tabPos;
        newTabRect.anchoredPosition = new Vector2(tabPos.x + newTabOffset, tabPos.y);

        newTab.currentWebsite = startPage;
        newTab.siteHistory.Add(startPage);
        newTab.siteHistoryCount = 1;
        newTab.historyPointer = 1;
        tabs.Add(newTab);

        SwitchTab(newTab);
    }

    //Called when removing a tab with the close button
    public void CloseTab(TabInfo tab)
    {
        int index = tabs.FindIndex((x) => x == tab);

        //If the closed tab was currently open, switch to a new one
        if (currentTab == tab)
        {
            int newIndex = 0;

            if (index == tabs.Count - 1)
            {
                newIndex = index - 1;
            }
            else
            {
                newIndex = index + 1;
            }

            SwitchTab(tabs[newIndex]);
        }

        tabs.Remove(tab);
        Destroy(tab.gameObject);

        //Move every tab that comes after to it's new place
        for (int i = index; i < tabs.Count; i++)
        {
            tabs[i].rect.anchoredPosition = new Vector2(firstTabOffset + i * (spaceBetweenTabs + tabWidth), tabYPosition);
        }

        newTabRect.anchoredPosition = new Vector2(firstTabOffset + (tabs.Count - 1) * (spaceBetweenTabs + tabWidth) + newTabOffset, tabYPosition);
    }

    //Called when switching to another tab
    public void SwitchTab(TabInfo tab)
    {
        if (currentTab != null && currentTab != tab)
        {
            //Deactivate old tab
            currentTab.active = false;
            currentTab.currentWebsite.siteObject.SetActive(false);
        }

        //Switch to new tab
        tab.active = true;
        tab.transform.SetAsLastSibling();

        currentTab = tab;
        Browse(tab.currentWebsite, BrowseType.SwitchTab);
    }

    #endregion

    private void Update()
    {
        //URL writing and browsing
        if (true /*InputManager.Instance.inKeyboardReadMode*/)
        {
            //Writing blinker
            blinkTimer += Time.deltaTime;
            if (blinkTimer > writeCharacterBlinkTime)
            {
                showingBlink = !showingBlink;
                blinkTimer = 0f;

                blinker.gameObject.SetActive(showingBlink);
            }

            Vector2 blinkerLocalPos = new Vector2(-urlText.rectTransform.rect.width / 2f, 0f);
            Vector3 blinkerScreenPosLeft = computerScreen.InverseTransformPoint(urlText.rectTransform.TransformPoint(blinkerLocalPos));
            Vector3 blinkerScreenPosPlusText = new Vector3(blinkerScreenPosLeft.x + urlText.textBounds.size.x, blinkerScreenPosLeft.y, blinkerScreenPosLeft.z);
            blinker.anchoredPosition = blinkerScreenPosPlusText;

            //print(urlText.textBounds.size);
            //print(urlText.renderedWidth);

            string inputString = "";
            foreach (char c in inputString)
            {
                //If we pressed enter, search with the input text
                if (c == 13)
                {
                    BrowseWithURL(urlText.text);
                    ExitURLWriteMode();
                    break;
                }
                //If we pressed backspace, remove a character
                else if (c == 8)
                {
                    if(urlText.text.Length > 0)
                    {
                        urlText.text = urlText.text.Substring(0, urlText.text.Length - 1);
                    }
                }
                else if (c >= 97 && c <= 122 || c == 46 || c == 47 || c == 58)
                {
                    urlText.text += c;
                }
            }
        }
    }

    public void PurchaseItem(BartendingItem item)
    {
        
    }
}