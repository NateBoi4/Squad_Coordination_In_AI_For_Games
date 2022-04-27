using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdjustValues : MonoBehaviour
{
    public Text squads;
    public int squadCount = 4;

    public Text units;
    public int unitCount = 4;

    public Text squadOne;
    public Text squadTwo;
    public Text squadThree;
    public Text squadFour;
    public Text squadFive;
    public Text squadSix;

    public UnitManager manager;


    private void Start()
    {
        squads.text = "Squads: " + squadCount; 
        units.text = "Units: " + unitCount;
        checkSquads();
    }

    public void IncreaseSqauds()
    {
        if(squadCount < 6)
        {
            squadCount++;
            squads.text = "Squads: " + squadCount;
            checkSquads();
        }
    }

    public void DecreaseSquads()
    {
        if (squadCount > 2)
        {
            squadCount--;
            squads.text = "Squads: " + squadCount;
            checkSquads();
        }
    }

    public void IncreaseUnits()
    {
        if (unitCount < 4)
        {
            unitCount++;
            units.text = "Units: " + unitCount;
        }
    }

    public void DecreaseUnits()
    {
        if (unitCount > 1)
        {
            unitCount--;
            units.text = "Units: " + unitCount;
        }
    }

    public void HidePanel()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        manager.numSquads = squadCount;
        manager.numUnits = unitCount;
    }

    private void checkSquads()
    {
        switch (squadCount)
        {
            case 2:
                squadOne.gameObject.SetActive(true);
                squadTwo.gameObject.SetActive(true);
                squadThree.gameObject.SetActive(false);
                squadFour.gameObject.SetActive(false);
                squadFive.gameObject.SetActive(false);
                squadSix.gameObject.SetActive(false);
                break;
            case 3:
                squadOne.gameObject.SetActive(true);
                squadTwo.gameObject.SetActive(true);
                squadThree.gameObject.SetActive(true);
                squadFour.gameObject.SetActive(false);
                squadFive.gameObject.SetActive(false);
                squadSix.gameObject.SetActive(false);
                break;
            case 4:
                squadOne.gameObject.SetActive(true);
                squadTwo.gameObject.SetActive(true);
                squadThree.gameObject.SetActive(true);
                squadFour.gameObject.SetActive(true);
                squadFive.gameObject.SetActive(false);
                squadSix.gameObject.SetActive(false);
                break;
            case 5:
                squadOne.gameObject.SetActive(true);
                squadTwo.gameObject.SetActive(true);
                squadThree.gameObject.SetActive(true);
                squadFour.gameObject.SetActive(true);
                squadFive.gameObject.SetActive(true);
                squadSix.gameObject.SetActive(false);
                break;
            case 6:
                squadOne.gameObject.SetActive(true);
                squadTwo.gameObject.SetActive(true);
                squadThree.gameObject.SetActive(true);
                squadFour.gameObject.SetActive(true);
                squadFive.gameObject.SetActive(true);
                squadSix.gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }
}
