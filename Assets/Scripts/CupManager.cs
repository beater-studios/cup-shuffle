using UnityEngine;
using System.Collections;

public class CupManager : MonoBehaviour
{
    #region Fields

    [Header("Cup Settings")]
    [Tooltip("Prefab of the cup.")]
    [SerializeField] private GameObject cupPrefab;

    [Tooltip("Number of shuffles to perform.")]
    [SerializeField] private int numberOfShuffles = 5;

    [Tooltip("Duration of each shuffle.")]
    [SerializeField] private float shuffleDuration = 0.5f;

    [Tooltip("Speed of the shuffling.")]
    [SerializeField] private float shuffleSpeed = 3f;

    [Tooltip("Height of the arc for the parabolic trajectory.")]
    [SerializeField] private float arcHeight = 1f;

    [Tooltip("Time to wait before shuffling again.")]
    [SerializeField] private float waitBeforeShuffle = 2f;

    private GameObject[] cups;
    private Vector3[] positions;
    private bool isShuffling = false;
    [SerializeField] private int correctCupIndex;
    [SerializeField] private int selectedCupIndex = -1;

    #endregion

    #region Properties

    public int GetSelectedCupIndex()
    {
        return selectedCupIndex;
    }

    public void SetSelectedCupIndex(int index)
    {
        selectedCupIndex = index;
    }

    public bool IsShuffling()
    {
        return isShuffling;
    }

    #endregion

    #region Unity Methods

    void Start()
    {
        InitializePositions();
        InitializeCups();
        PositionCamera();
        SelectRandomCup();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S) && !isShuffling)
        {
            StartCoroutine(ShuffleCups(numberOfShuffles));
        }
    }

    #endregion

    #region Initialization Methods

    /// <summary>
    /// Initializes the positions of the cups.
    /// </summary>
    void InitializePositions()
    {
        positions = new Vector3[]
        {
            new Vector3(-2f, 0, 0),
            new Vector3(0, 0, 0),
            new Vector3(2f, 0, 0)
        };
    }

    /// <summary>
    /// Initializes the cups.
    /// </summary>
    void InitializeCups()
    {
        GameObject cupParent = new GameObject("CupParent");
        cups = new GameObject[positions.Length];
        Quaternion cupRotation = Quaternion.Euler(90, 0, 0);

        for (int i = 0; i < positions.Length; i++)
        {
            cups[i] = Instantiate(cupPrefab, positions[i], cupRotation, cupParent.transform);
        }
    }

    #endregion

    #region Game Logic Methods

    /// <summary>
    /// Selects a random cup as the correct cup.
    /// </summary>
    void SelectRandomCup()
    {
        correctCupIndex = Random.Range(0, cups.Length);
    }

    /// <summary>
    /// Checks if the selected cup is the correct one.
    /// </summary>
    /// <param name="cupIndex">The index of the selected cup.</param>
    public void CheckCup(int cupIndex)
    {
        selectedCupIndex = cupIndex;
        if (cupIndex == correctCupIndex)
        {
            ScoreManager.instance.AddScore(10);
            Debug.Log("Correct! You earned 10 points.");
        }
        else
        {
            Debug.Log("Wrong! Try again.");
        }

        StartCoroutine(WaitAndShuffle());
    }

    /// <summary>
    /// Waits for a specified time before shuffling the cups again.
    /// </summary>
    IEnumerator WaitAndShuffle()
    {
        yield return new WaitForSeconds(waitBeforeShuffle);
        StartCoroutine(LowerCups());
    }

    /// <summary>
    /// Lowers all the revealed cups.
    /// </summary>
    IEnumerator LowerCups()
    {
        foreach (GameObject cup in cups)
        {
            CupInteraction cupInteraction = cup.GetComponent<CupInteraction>();
            if (cupInteraction != null && cupInteraction.IsRevealed())
            {
                yield return StartCoroutine(cupInteraction.LowerCup());
                break;
            }
        }

        SelectRandomCup();
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(ShuffleCups(numberOfShuffles));
    }

    /// <summary>
    /// Shuffles the cups for a specified number of times.
    /// </summary>
    /// <param name="shuffles">The number of shuffles to perform.</param>
    IEnumerator ShuffleCups(int shuffles)
    {
        if (isShuffling) yield break;
        isShuffling = true;

        for (int i = 0; i < shuffles; i++)
        {
            int firstIndex = Random.Range(0, cups.Length);
            int secondIndex = (firstIndex + 1 + Random.Range(0, cups.Length - 1)) % cups.Length;

            Vector3 firstPosition = cups[firstIndex].transform.position;
            Vector3 secondPosition = cups[secondIndex].transform.position;

            float elapsedTime = 0;

            while (elapsedTime < shuffleDuration)
            {
                float t = elapsedTime / shuffleDuration;
                float height = Mathf.Sin(t * Mathf.PI) * arcHeight;

                Vector3 newPos1 = Vector3.Lerp(firstPosition, secondPosition, t);
                Vector3 newPos2 = Vector3.Lerp(secondPosition, firstPosition, t);

                newPos1.y = Mathf.Lerp(firstPosition.y, secondPosition.y, t);
                newPos1.z += height;

                newPos2.y = Mathf.Lerp(secondPosition.y, firstPosition.y, t);
                newPos2.z -= height;

                cups[firstIndex].transform.position = newPos1;
                cups[secondIndex].transform.position = newPos2;

                elapsedTime += Time.deltaTime * shuffleSpeed;
                yield return null;
            }

            cups[firstIndex].transform.position = secondPosition;
            cups[secondIndex].transform.position = firstPosition;
        }

        isShuffling = false;
    }

    /// <summary>
    /// Positions the camera to focus on the cups.
    /// </summary>
    void PositionCamera()
    {
        Camera.main.transform.position = new Vector3(0, 5, -5);
        Camera.main.transform.rotation = Quaternion.Euler(45, 0, 0);
        Camera.main.transform.LookAt(new Vector3(0, 0, 0));
    }

    #endregion

}