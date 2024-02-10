using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private UnitManager _gameManager;

    [Header("Unit Properties")]

    [SerializeField] private string _unitName = "Unit";
    [SerializeField] private string _unitDescription = "A unit.";

    [SerializeField] private UnitClass unitClass;
    [SerializeField] private UnitTeam team;
    private PromotesTo promotionClass;

    [Header("Level")]
    private int _minLevel = 1;
    private int _maxLevel = 20;

    [SerializeField]
    [Range(1, 20)]
    private int _level;

    [SerializeField]
    [Range(0, 100)]
    private int experience;

    [Header("Stats")]
    public int health = 0;
    public int maxHealth = 0;

    public int power;
    public int skill;
    public int speed;
    public int constitution;
    public int defense;
    public int resistance;
    public int luck;

    public int movementRange = 0;

    [SerializeField]
    private enum UnitTeam
    {
        Team1,
        Team2
    }

    [SerializeField]
    private enum UnitClass
    {
        Archer,
        Cavalier,
        DragonRider,
        Fighter,
        Mage,
        Priest,
        Warlock,
    }

    private enum PromotesTo
    {
        Sniper,
        Paladin,
        DragonKnight,
        Hero,
        Sage,
        Bishop,
        Demon
    }

    void Start()
    {
        _gameManager = UnitManager.Instance;

        if (!_gameManager.allUnits.Contains(this))
        {
            _gameManager.allUnits.Add(this);

            switch (team)
            {
                case UnitTeam.Team1:
                    _gameManager.playerTeam.Add(this);
                    break;
                case UnitTeam.Team2:
                    _gameManager.enemyTeam.Add(this);
                    break;
            }
        }

        LoadUnitData();
    }

    // Handles experience gain.
    public void GainExperience(int experienceGained)
    {
        if (_level != _maxLevel)
        {
            experience += experienceGained;

            if (experience >= 100)
            {
                experience -= 100;
                _level++;
            }
        }
    }

    // Handles taking damage.
    public void TakeDamage(int damage)
    {
        if (health > 0)
        {
            health -= damage;

            if (health <= 0)
            {
                gameObject.SetActive(false);
            }
        }
    }

    private void LoadUnitData()
    {
        UnitClassData selectedData = GetSelectedClassData();

        if (selectedData != null)
        {
            movementRange = selectedData.defaultMovementRange;

            ApplyGrowthForLevels(selectedData);
            ApplyBaseStats(selectedData);
        }
    }
    private void ApplyBaseStats(UnitClassData selectedData)
    {
        health += selectedData.defaultHealth;
        power += selectedData.defaultPower;
        skill += selectedData.defaultSkill;
        constitution += selectedData.defaultConstitution;
        defense += selectedData.defaultDefense;
        resistance += selectedData.defaultResistance;
        luck += selectedData.defaultLuck;
        maxHealth = health;
    }

    private void ApplyGrowthForLevels(UnitClassData selectedData)
    {
        for (int i = 1; i <= _level; i++)
        {
            health = CalculateStatWithGrowth(health, selectedData.healthGrowth);
            power = CalculateStatWithGrowth(power, selectedData.powerGrowth);
            skill = CalculateStatWithGrowth(skill, selectedData.skillGrowth);
            speed = CalculateStatWithGrowth(speed, selectedData.speedGrowth);
            constitution = CalculateStatWithGrowth(constitution, selectedData.constitutionGrowth);
            defense = CalculateStatWithGrowth(defense, selectedData.defenseGrowth);
            resistance = CalculateStatWithGrowth(resistance, selectedData.resistanceGrowth);
            luck = CalculateStatWithGrowth(luck, selectedData.luckGrowth);
        }
    }


    private UnitClassData GetSelectedClassData()
    {
        promotionClass = GetPromotionClass(unitClass);
        return GetUnitClassData(unitClass);
    }

    // Handles class promotions. Based on the base class chosen.
    private PromotesTo GetPromotionClass(UnitClass unitClass)
    {
        switch (unitClass)
        {
            case UnitClass.Archer: return PromotesTo.Sniper;
            case UnitClass.Cavalier: return PromotesTo.Paladin;
            case UnitClass.DragonRider: return PromotesTo.DragonKnight;
            case UnitClass.Fighter: return PromotesTo.Hero;
            case UnitClass.Mage: return PromotesTo.Sage;
            case UnitClass.Priest: return PromotesTo.Bishop;
            case UnitClass.Warlock: return PromotesTo.Demon;
            default: return PromotesTo.Paladin;
        }
    }

    // Gets the corresponding class data to the selected enum
    private UnitClassData GetUnitClassData(UnitClass unitClass)
    {
        switch (unitClass)
        {
            case UnitClass.Archer: return _gameManager.archerData;
            case UnitClass.Cavalier: return _gameManager.cavalierData;
            case UnitClass.DragonRider: return _gameManager.dragonRiderData;
            case UnitClass.Fighter: return _gameManager.fighterData;
            case UnitClass.Mage: return _gameManager.mageData;
            case UnitClass.Priest: return _gameManager.priestData;
            case UnitClass.Warlock: return _gameManager.warlockData;
            default: return _gameManager.cavalierData;
        }
    }

    // Rolls a number between 0.1 - 1.0. If the rolled number is less or equal to the growth rate, the unit receives a point in that respective stat. Else, nothing happens.
    private int CalculateStatWithGrowth(int baseStat, float growthRate)
    {
        float randomValue = Random.value;

        if (randomValue <= growthRate)
        {
            return baseStat + 1;
        }

        return baseStat;
    }
}
