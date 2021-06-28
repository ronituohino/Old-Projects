using System.Collections;
using UnityEngine;

public class MoreInfo : Singleton<MoreInfo>
{
    [SerializeField] MoreInfoPanel heroPanel;
    [SerializeField] MoreInfoPanel itemPanel;

    [SerializeField] RectTransform moreInfoParent;

    [Space]

    [SerializeField] float moreInfoTransitionTime;

    public RectTransform focusedObjectRect;

    public Transform originalParent;
    Vector2 rectOriginalScreenPosition;

    public void ShowMoreInfo(Hero hero, ItemObject item)
    {
        if (!heroPanel.opened && !itemPanel.opened)
        {
            if (hero != null)
            {
                originalParent = hero.rect.parent;
                rectOriginalScreenPosition = RectTransformUtility.WorldToScreenPoint
                                                (
                                                    InputManager.Instance.sceneCamera,
                                                    hero.rect.position
                                                );

                heroPanel.opened = true;
                heroPanel.gameObject.SetActive(true);

                focusedObjectRect = hero.rect;

                StartCoroutine(Open(heroPanel));
            }
            else if (item != null)
            {
                originalParent = item.rect.parent;
                rectOriginalScreenPosition = RectTransformUtility.WorldToScreenPoint
                                                (
                                                    InputManager.Instance.sceneCamera,
                                                    item.rect.position
                                                );

                itemPanel.opened = true;
                itemPanel.gameObject.SetActive(true);

                focusedObjectRect = item.rect;

                StartCoroutine(Open(itemPanel));
            }
        }
    }

    public void HideMoreInfo(MoreInfoPanel panel)
    {
        StartCoroutine(Close(panel));
    }

    IEnumerator Open(MoreInfoPanel panel)
    {
        int steps = Mathf.RoundToInt(moreInfoTransitionTime / Time.fixedDeltaTime);
        focusedObjectRect.SetParent(InputManager.Instance.sceneCanvas);

        Vector2 startPoint = RectTransformUtility.WorldToScreenPoint(InputManager.Instance.sceneCamera, focusedObjectRect.position);
        Vector2 targetPoint = RectTransformUtility.WorldToScreenPoint(InputManager.Instance.sceneCamera, panel.contentAnchor.position);

        for (int i = 1; i <= steps; i++)
        {
            Extensions.TranslateRect(focusedObjectRect, startPoint, targetPoint, ((float)i / (float)steps));
            yield return GameManager.Instance.wfu;
        }
    }

    IEnumerator Close(MoreInfoPanel panel)
    {
        int steps = Mathf.RoundToInt(moreInfoTransitionTime / Time.fixedDeltaTime);

        Vector2 startPoint = RectTransformUtility.WorldToScreenPoint(InputManager.Instance.sceneCamera, focusedObjectRect.position);
        Vector2 targetPoint = rectOriginalScreenPosition;

        for (int i = 1; i <= steps; i++)
        {
            Extensions.TranslateRect(focusedObjectRect, startPoint, targetPoint, ((float)i / (float)steps));
            yield return GameManager.Instance.wfu;
        }

        focusedObjectRect.SetParent(originalParent);

        panel.opened = false;
        panel.gameObject.SetActive(false);

        ResetParams();
    }

    public void ResetParams()
    {
        originalParent = null;
        focusedObjectRect = null;
        rectOriginalScreenPosition = Vector2.zero;
    }
}