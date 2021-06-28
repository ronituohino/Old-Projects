using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Hero : Interactable
{
    [Header("Gameplay")]
    // This serves as a base for what the character started with
    public Character characterBase = null; 

    // These are the characters current stats and data
    public Character.CharacterData currentData;

    public Item item = null;

    public List<Status> activeStatuses = new List<Status>();



    // Optimization
    Status deadStatus;


    [Header("UI")]

    public RectTransform rect;
    [SerializeField] int id;

    [SerializeField] Image characterImage;
    [SerializeField] CanvasGroup cg;

    public ItemContainer itemContainer;
    [SerializeField] TextMeshProUGUI hpAmount;

    private void Start()
    {
        deadStatus = StatusManager.Instance.FindStatus("Dead");
        UpdateHero();
    }

    public void UpdateHero()
    {
        if(characterBase == null)
        {
            cg.alpha = 0f;
            item = null;

            itemContainer.canStoreItem = false;
            cg.blocksRaycasts = false;

            if(itemContainer.storedItem != null)
            {
                Destroy(itemContainer.storedItem);
                itemContainer.storedItem = null;
            }
        }
        else
        {
            cg.alpha = 1f;
            itemContainer.canStoreItem = true;

            characterImage.sprite = characterBase.sprite;
            cg.blocksRaycasts = true;

            // Calculate current stats
            int maxHealth = characterBase.data.stats.currentHealth + (item != null ? item.statEffects.currentHealth : 0);
            int currentHealth = maxHealth + activeStatuses.Sum(x => x.statEffects.currentHealth);

            bool isDead = activeStatuses.Contains(deadStatus);
            if (currentHealth <= 0)
            {
                if(!isDead)
                {
                    activeStatuses.Add(deadStatus);
                    isDead = true;
                }
            }

            int attack = characterBase.data.stats.attack + (item != null ? item.statEffects.attack : 0) + activeStatuses.Sum(x => x.statEffects.attack);
            float swiftness = characterBase.data.stats.swiftness + (item != null ? item.statEffects.swiftness : 0) + activeStatuses.Sum(x => x.statEffects.swiftness);

            float attackSpeed = characterBase.data.stats.attackSpeed + (item != null ? item.statEffects.attackSpeed : 0) + activeStatuses.Sum(x => x.statEffects.attackSpeed);
            float attackRange = characterBase.data.stats.attackRange + (item != null ? item.statEffects.attackRange : 0) + activeStatuses.Sum(x => x.statEffects.attackRange);

            Character.CharacterData.Stats newStats = new Character.CharacterData.Stats();
            newStats.maxHealth = maxHealth;
            newStats.currentHealth = (isDead ? 0 : currentHealth);

            newStats.attack = attack;
            newStats.swiftness = swiftness;

            newStats.attackSpeed = attackSpeed;
            newStats.attackRange = attackRange;

            currentData.stats = newStats;

            currentData.characterId = id;
            currentData.projectileVelocity = characterBase.data.projectileVelocity;
            currentData.attackAdvantageTime = characterBase.data.attackAdvantageTime;

            // Update visuals
            hpAmount.text = $"{currentHealth}/{maxHealth}";
        }
    }

    public override void Enter(int fingerId)
    {

    }

    public override void Stay(Vector2 delta)
    {

    }

    public override void Interrupt()
    {
        
    }

    public override void Complete()
    {
        MoreInfo.Instance.ShowMoreInfo(this, null);
    }
}
