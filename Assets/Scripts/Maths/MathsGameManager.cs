using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

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
    public float lasthighestPoint;

    public float score;
    public float scoreForBlockSuccess;
    public float scoreForBlockFailure;
    //public float scoreMultiplier;
    //public float multiplierBreakTime;

    [Header("GamePlay Information")]
    public bool isPlaying;

    public float timerLength;
    public float timerCurrent;
    public float minutes { get; private set; }
    public float seconds { get; private set; }

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

    public TMP_Text countdownText;
    public TMP_Text updateText;

    public TMP_Text finalScore;
    public GameObject gameplayElements;
    public GameObject gameplayUI;
    public GameObject gameendUI;

    private void Start()
    {
        //Application.targetFrameRate = 60;

        GetProblems();
        ChooseNewProblem();

        isPlaying = false;
        StartCoroutine(Countdown());
    }

    public IEnumerator Countdown()
    {
        //Debug.Log("Countdown Start");

        int time = 3;

        countdownText.gameObject.SetActive(true);

        while (time > 0)
        {
            countdownText.text = time.ToString();

            yield return new WaitForSeconds(1);
            time--;
        }

        countdownText.gameObject.SetActive(false);

        updateText.gameObject.SetActive(true);
        updateText.text = "Begin";

        isPlaying = true;
        TimerStart();

        yield return new WaitForSeconds(0.5f);
        updateText.gameObject.SetActive(false);

        //Debug.Log("Countdown End");
    }

    private void Update()
    {
        if (isPlaying)
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
                        /*else if (keyCode.ToString() == "Backspace")
                        {
                            RemoveNumberFromSolution();
                        }*/
                        //Debug.Log(keyCode.ToString());
                    }
                }
                if (Input.GetKeyDown("backspace"))
                {
                    RemoveNumberFromSolution();
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

    public void TimerStart()
    {
        timerCurrent = timerLength;
        StartCoroutine(Timer());
    }

    public void TimerDisplay()
    {
        string secondsString = "";
        string minutesString = "";

        if (seconds < 10)
        {
            secondsString = "0" + seconds.ToString();
        }
        else
        {
            secondsString = seconds.ToString();
        }
        if (minutes < 10)
        {
            minutesString = "0" + minutes.ToString();
        }
        else
        {
            minutesString = minutes.ToString();
        }

        timerText.text = minutesString + " : " + secondsString;
    }

    public IEnumerator Timer()
    {
        while (timerCurrent > 0)
        {
            if (isPlaying)
            {
                timerCurrent--;

                seconds = timerCurrent;
                minutes = 0;
                while (seconds > 60)
                {
                    seconds -= 60;
                    minutes++;
                }

                TimerDisplay();

                yield return new WaitForSeconds(1f);
            }
        }

        GameEnd();
    }

    public void GameEnd()
    {
        isPlaying = false;

        //gameplayElements.SetActive(false);
        /*for (int i = 0; i < gameplayElements.transform.childCount; i++)
        {
            if (gameplayElements.transform.GetChild(i).gameObject != Camera.main.gameObject)
            {
                gameplayElements.transform.GetChild(i).gameObject.SetActive(false);
            }
        }*/
        numberButtonsHolder.SetActive(false);

        gameplayUI.SetActive(false);
        gameendUI.SetActive(true);
        finalScore.text = scoreText.text;
    }

    public IEnumerator BlockLoop()
    {
        while (isDroppingBlock)
        {
            ControlBlock();

            yield return new WaitForSeconds(0.1f);
        }

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

        Vector3 moveAmount = new Vector3(blockMoveSpeed * moveMultiplier * Time.deltaTime, blockFallSpeed * Time.deltaTime, 0);

        currentBlock.transform.position += moveAmount;
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

        AdjustPosition();

        AddScore(scoreForBlockSuccess);

        //Debug.Log("Block Landed");
    }

    public void AdjustPosition()
    {
        float targetY = viewHolder.transform.position.y + (highestPoint - lasthighestPoint);

        StartCoroutine(SmoothAdjustPosition(targetY));
    }

    private IEnumerator SmoothAdjustPosition(float targetY)
    {
        float duration = 1.0f; // Adjust this value as needed
        float elapsedTime = 0.0f;

        Vector3 initialPosition = viewHolder.transform.position;
        Vector3 targetPosition = new Vector3(initialPosition.x, targetY, initialPosition.z);

        while (elapsedTime < duration)
        {
            viewHolder.transform.position = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        viewHolder.transform.position = targetPosition;
    }

    public void CalculateHighestPoint()
    {
        lasthighestPoint = highestPoint;

        float currentBlockHighestY = currentBlock.GetComponent<SpriteRenderer>().bounds.max.y;

        if (currentBlockHighestY > highestPoint)
        {
            highestPoint = currentBlockHighestY;
        }

        //.ToString("F3") rounds to 2 decimals, so only shows 3 decimals in the score
        scoreText.text = "Highest Point: " + highestPoint.ToString("F3");

        Debug.Log("Highest Point Calculated: " + highestPoint);
    }

    public void AddScore(float scoreToAdd)
    {
        score += scoreToAdd;
        //scoreText.text = score.ToString();
    }
}
