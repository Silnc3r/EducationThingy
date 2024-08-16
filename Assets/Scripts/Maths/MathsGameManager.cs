using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MathsGameManager : MonoBehaviour
{
    [Header("Information")]
    public string[] problems;
    public string currentProblem;
    public string correctSolution;
    public string currentSolution;

    public GameObject[] symbolPrefabs;
    public GameObject[] solutionSymbols;

    [Header("References")]
    public GameObject problemHolder;
    public GameObject solutionHolder;
    public GameObject blockHolder;

    public GameObject[] numberButtons;
    public GameObject numberButtonsHolder;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null)
            {
                //Debug.Log("CLICKED " + hit.collider.name);

                foreach (GameObject nb in numberButtons)
                {
                    if (hit.collider.name == nb.name)
                    {
                        nb.GetComponent<NumberButtonController>().Interact();
                    }
                }
            }
        }
    }

    public void EnterSolution()
    {
        Debug.Log("Entered Solution: " + currentSolution);

        if (CheckSolution())
        {
            Debug.Log("Correct");
        }
        else
        {
            Debug.Log("Incorrect");
        }
    }

    public bool CheckSolution()
    {
        if (currentSolution == correctSolution)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void MakeSolutionNegative()
    {
        Debug.Log("Solution Made Negative");
    }

    public void RemoveNumberFromSolution()
    {
        Debug.Log("Remove Number");
    }

    public void AddNumberToSolution(int number)
    {
        Debug.Log("Add: " + number);
    }

    public void GetProblems()
    {
        Debug.Log("Get Problems From File");
    }

    public void ChooseNewProblem()
    {
        Debug.Log("Choose New Problem");
    }

    public void DisplayProblem()
    {
        Debug.Log("Display Problem");
    }
}
