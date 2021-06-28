using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BartendingBook : MonoBehaviour
{
    [SerializeField] GameObject book;

    //The first page is a logo, (0)
    //The second page is a catalogue that reaches x pages (1-x)

    [System.Serializable]
    public class Page
    {
        [Tooltip("Enable for an empty logo page")]
        public bool logoPage;
        [Tooltip("0 if is not a catalogue page")]
        internal bool catalogue; 
        [Tooltip("Show this drink on page")]
        public Drink shownDrink;
    }

    public List<Page> pages = new List<Page>();
    int currentPage = 0; //The page number on the left is the current page

    [System.Serializable]
    public class PageControls
    {
        public RectTransform logoRect;
        public RectTransform catalogueRect;
        public RectTransform drinkRect;
    }

    [SerializeField] PageControls leftPage;
    [SerializeField] PageControls rightPage;

    private void Start()
    {
        GlobalReferencesAndSettings.Instance.bartendingBook = this;
    }

    void UpdateBook()
    {
        int pageCount = pages.Count;

        Page left = pages[currentPage];
        Page right = null;

        if (currentPage + 1 < pageCount)
        {
            right = pages[currentPage + 1];
        }

        SetPage(leftPage, left);
        SetPage(rightPage, right);
    }

    void SetPage(PageControls c, Page p)
    {

    }

    public void OpenBook()
    {
        book.SetActive(true);
    }

    public void CloseBook()
    {
        book.SetActive(false);
    }

    public void ReturnToCatalogue()
    {
        print("cat");
    }

    public void SwitchPageLeft()
    {
        print("l");
    }

    public void SwitchPageRight()
    {
        print("r");
    }
}