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

    public Vector2 furthestMove; // furthest position in x, x is positive, y is negative
    public bool moveDir; // false is left

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

        if (!isDroppingBlock)
        {
            foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyCode))
                {
                    if (NumberCheck(keyCode.ToString()))
                    {
                        string pressedNumberString = keyCode.ToString();
                        //Debug.Log(pressedNumberString);
                        if (pressedNumberString.Contains("Alpha"))
                        {
                            pressedNumberString = pressedNumberString.Replace("Alpha", "");
                        }
                        //Debug.Log(pressedNumberString);

                        int pressedNumber = int.Parse(pressedNumberString);
                        AddNumberToSolution(pressedNumber);
                    }
                    else if (keyCode.ToString() == "Minus")
                    {
                        MakeSolutionNegative();
                    }
                    else if (keyCode.ToString() == "Return")
                    {
                        EnterSolution();
                    }
                }
            }
        }
        else
        {
            if (Input.GetKeyDown("space"))
            {
                ReleaseBlock();
            }
        }
    }

    public bool NumberCheck(string possibleNumber)
    {
        if (possibleNumber.Contains("Alpha"))
        {
            possibleNumber = possibleNumber.Replace("Alpha", "");
        }

        if (possibleNumber == "0" ||
            possibleNumber == "1" ||
            possibleNumber == "2" ||
            possibleNumber == "3" ||
            possibleNumber == "4" ||
            possibleNumber == "5" ||
            possibleNumber == "6" ||
            possibleNumber == "7" ||
            possibleNumber == "8" ||
            possibleNumber == "9")
        {
            return true;
        }
        else return false;
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

            //Debug.Log("Solution Made Positive");
        }
        else
        {
            if (currentSolution == "0")
            {
                currentSolution = "";
            }

            currentSolution = "-" + currentSolution;
            numberButtons[9].GetComponent<SpriteRenderer>().sprite = negativeButtonSprites[1];

            //Debug.Log("Solution Made Negative");
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

        //Debug.Log("Removed Last Number");
    }

    public void ClearSolution()
    {
        currentSolution = "0";
        numberButtons[9].GetComponent<SpriteRenderer>().sprite = negativeButtonSprites[0];

        DisplaySolution();

        //Debug.Log("Solution Cleared");
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
        //Debug.Log("New Answer: " + currentSolution);
    }

    public void GetProblems()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "maths_problems.txt");

        if (!File.Exists(path))
        {
            Debug.LogError("File not found at: " + path);
            return;
        }

        string fileContents = "";

        if (path.Contains("://") || path.Contains(":///"))
        {
            // For Android or WebGL, where files may be accessed via URL
            WWW reader = new WWW(path);
            while (!reader.isDone) { }
            fileContents = reader.text;
        }
        else
        {
            // For standalone platforms or Editor
            fileContents = File.ReadAllText(path);
        }

        using (StringReader reader = new StringReader(fileContents))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (IsValidProblem(line))
                {
                    problems.Add(line.ToLower());
                }
            }
        }
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

        //Debug.Log("Chose New Problem: " + currentProblem + " The Solution Is: " + correctSolution);
    }

    public void DisplaySolution()
    {
        solutionText.text = currentSolution;

        //Debug.Log("Displayed Problem");
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

        //Debug.Log("Block Finished");
        ChooseNewProblem();
    }
    
    public void CreateBlock()
    {
        isDroppingBlock = true;

        currentBlock = Instantiate(blockPrefab, blockSpawnPosition.transform.position, Quaternion.identity);
        currentBlock.GetComponent<SpriteRenderer>().color = blockColour[Random.Range(0, blockColour.Length)];

        Debug.Log("Created Block");

        StartCoroutine(BlockLoop());
    }

    public void ControlBlock()
    {
        //moveDir false is left, true is right
        if (currentBlock.transform.position.x > furthestMove.x)
        {
            moveDir = true;
        }else if (currentBlock.transform.position.x < furthestMove.y)
        {
            moveDir = false;
        }

        int moveMultiplier = 1;
        if (moveDir)
        {
            moveMultiplier = -1;
        }

        currentBlock.transform.position += new Vector3(blockMoveSpeed * moveMultiplier * Time.deltaTime, blockFallSpeed * Time.deltaTime, 0);
    }

    public void ReleaseBlock()
    {
        isDroppingBlock = false;
        currentBlock.GetComponent<Rigidbody2D>().gravityScale = 1;
        //maybe increase scale but turn off rotation
        Debug.Log("Release Block");
    }

    public void BlockLanded()
    {
        isDroppingBlock = false;
        CalculateHighestPoint();

        //Debug.Log("Block Landed");
    }

    public void AdjustPosition()
    {
        //change the move to be not instant
        viewHolder.transform.position += new Vector3(0, blockPrefab.GetComponent<SpriteRenderer>().bounds.size.y, 0);

        Debug.Log("Adjusted Position");
    }

    public void CalculateHighestPoint()
    {
        Debug.Log(currentBlock.GetComponent<SpriteRenderer>().bounds.max.y);

        Debug.Log("Highest Point Calculated");
    }

    public void AddScore(float scoreToAdd)
    {

    }

    public void CalculateFinalScore()
    {

    }
}
