using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NumberButtonController : MonoBehaviour
{
    [Header("Information")]
    public string interactType;
    public int input;

    [Header("References")]
    public MathsGameManager gm;

    private void Start()
    {
        gm = GameObject.Find("MathsGameManager").GetComponent<MathsGameManager>();
    }

    public void Interact()
    {
        switch (interactType)
        {
            default:
                Debug.Log("BUTTON NOT SET ERROR: " + gameObject.name);
                break;
            case "number":
                gm.AddNumberToSolution(input);
                break;
            case "negative":
                gm.MakeSolutionNegative();
                break;
            case "delete":
                gm.RemoveNumberFromSolution();
                break;
            case "enter":
                gm.EnterSolution();
                break;
        }
    }
}
