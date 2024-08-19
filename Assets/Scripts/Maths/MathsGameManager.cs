using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using JetBrains.Annotations;

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

    public TMP_Text problemText;
    public TMP_Text solutionText;

    private void Start()
    {
        ChooseNewProblem();
    }

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
        //Debug.Log("Entered Solution: " + currentSolution);

        if (CheckSolution())
        {
            Debug.Log("Correct");

            ChooseNewProblem();
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
        if (currentSolution.Contains("-"))
        {
            currentSolution = currentSolution.Replace("-", "");
            Debug.Log("Solution Made Positive");
        }
        else
        {
            currentSolution = "-" + currentSolution;
            Debug.Log("Solution Made Negative");
        }

        DisplaySolution();
    }

    public void RemoveNumberFromSolution()
    {
        if (currentSolution.Length == 1)
        {
            currentSolution = "0";
        }
        else
        {
            currentSolution = currentSolution.Substring(0, currentSolution.Length - 1);
        }

        DisplaySolution();

        Debug.Log("Removed Last Number");
    }

    public void ClearSolution()
    {
        currentSolution = "0";

        DisplaySolution();

        Debug.Log("Solution Cleared");
    }

    public void AddNumberToSolution(int number)
    {
        if (currentSolution != "" && currentSolution != "0")
        {
            currentSolution += number.ToString();
        }
        else
        {
            currentSolution = number.ToString();
        }

        DisplaySolution();

        Debug.Log("Added: " + number);
        Debug.Log("New Answer: " + currentSolution);
    }

    public void GetProblems()
    {
        Debug.Log("Get Problems From File");
    }

    public void ChooseNewProblem()
    {
        ClearSolution();

        int i = Random.Range(0, problems.Length);
        string tempString = problems[i];

        currentProblem = tempString.Split('=')[0];
        correctSolution = tempString.Split('=')[1];

        problemText.text = currentProblem;

        Debug.Log("Chose New Problem: " + currentProblem + " The Solution Is: " + correctSolution);
    }

    public void DisplaySolution()
    {
        solutionText.text = currentSolution;

        Debug.Log("Displayed Problem");
    }
}
