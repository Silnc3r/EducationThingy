using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using JetBrains.Annotations;
using System.IO;

public class MathsGameManager : MonoBehaviour
{
    [Header("Maths Information")]
    public List<string> problems = new();
    public string currentProblem;
    public string correctSolution;
    public string currentSolution;

    public Sprite[] negativeButtonSprites;

    [Header("Blocks Information")]
    public GameObject blockPrefab;
    public Color[] blockColour;

    public GameObject blockSpawnPosition;

    public float blockFallSpeed;
    public float blockMoveSpeed;

    public GameObject currentBlock;
    public bool isDroppingBlock;

    public float highestPoint;

    public float score;
    public float scoreForBlockSuccess;
    public float scoreForBlockFailure;
    public float scoreMultiplier;
    public float multiplierBreakTime;

    public float timerSeconds;
    public float timerMinutes;
    public float timerStarttime; // total length of timer in seconds

    [Header("References")]
    public GameObject viewHolder;

    public GameObject problemHolder;
    public GameObject solutionHolder;

    public GameObject[] numberButtons;
    public GameObject numberButtonsHolder;

    public TMP_Text problemText;
    public TMP_Text solutionText;
    public TMP_Text timerText;
    public TMP_Text scoreText;

    private void Start()
    {
        GetProblems();
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
        if (!isDroppingBlock)
        {
            //Debug.Log("Entered Solution: " + currentSolution);

            if (CheckSolution())
            {
                Debug.Log("Correct");

                //ChooseNewProblem();
                CreateBlock();
            }
            else
            {
                Debug.Log("Incorrect");
            }
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
            numberButtons[9].GetComponent<SpriteRenderer>().sprite = negativeButtonSprites[0];

            if (currentSolution == "")
            {
                currentSolution = "0";
            }

            Debug.Log("Solution Made Positive");
        }
        else
        {
            if (currentSolution == "0")
            {
                currentSolution = "";
            }

            currentSolution = "-" + currentSolution;
            numberButtons[9].GetComponent<SpriteRenderer>().sprite = negativeButtonSprites[1];

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
        numberButtons[9].GetComponent<SpriteRenderer>().sprite = negativeButtonSprites[0];

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
        string path = "Assets/Resources/maths_problems.txt";

        if (!File.Exists(path))
        {
            Debug.Log("File not found at: " + path);
            return;
        }

        StreamReader reader = new StreamReader(path);
        string line;

        while ((line = reader.ReadLine()) != null)
        {
            if (IsValidProblem(line))
            {
                problems.Add(line.ToLower());
            }
        }

        reader.Close();
    }

    public bool IsValidProblem(string inputProblem)
    {
        if (inputProblem.Split('=').Length - 1 == 1)
        {
            Debug.Log(inputProblem + " is a valid problem");
            return true;
        }
        else
        {
            Debug.Log(inputProblem + " is an invalid problem");
            return false;
        }
    }

    public void ChooseNewProblem()
    {
        ClearSolution();

        int i = Random.Range(0, problems.Count);
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

    public IEnumerator Timer()
    {
        yield return new WaitForSeconds(1);
    }

    public IEnumerator BlockLoop()
    {
        while (isDroppingBlock)
        {
            ControlBlock();

            yield return new WaitForSeconds(0.1f);
        }

        AdjustPosition();
        CalculateHighestPoint();

        Debug.Log("Block Finished");
        ChooseNewProblem();
    }
    
    public void CreateBlock()
    {
        isDroppingBlock = true;

        currentBlock = Instantiate(blockPrefab, blockSpawnPosition.transform.position, Quaternion.identity);

        Debug.Log("Created Block");

        StartCoroutine(BlockLoop());
    }

    public void ControlBlock()
    {

    }

    public void BlockLanded()
    {
        isDroppingBlock = false;

        Debug.Log("Block Landed");
    }

    public void AdjustPosition()
    {
        //change the move to be not instant
        viewHolder.transform.position += new Vector3(0, blockPrefab.GetComponent<SpriteRenderer>().bounds.size.y, 0);

        Debug.Log("Adjusted Position");
    }

    public void CalculateHighestPoint()
    {
        Debug.Log("Highest Point Calculated");
    }

    public void AddScore(float scoreToAdd)
    {

    }

    public void CalculateFinalScore()
    {

    }
}
