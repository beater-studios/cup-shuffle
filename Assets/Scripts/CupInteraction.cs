using UnityEngine;
using System.Collections;

public class CupInteraction : MonoBehaviour
{
    #region Fields

    [Header("Cup Settings")]
    [Tooltip("Height the cup rises when revealed.")]
    [SerializeField] private const float revealHeight = 1f;

    [Tooltip("Speed of revealing.")]
    [SerializeField] private const float revealSpeed = 5f;

    private Vector3 currentPosition;
    private bool isRevealed = false;
    private Outline currentOutline;
    private CupManager cupManager;

    #endregion

    #region Unity Methods

    void Start()
    {
        currentPosition = transform.position;
        currentOutline = GetComponent<Outline>();
        cupManager = FindObjectOfType<CupManager>();
    }

    void OnMouseDown()
    {
        if (this.CanInteract())
        {
            StartCoroutine(RevealCup());
            int cupIndex = transform.GetSiblingIndex();
            cupManager.CheckCup(cupIndex);
        }
    }

    void OnMouseEnter()
    {
        SetOutlineMode(Outline.Mode.OutlineAndSilhouette);
    }

    void OnMouseExit()
    {
        SetOutlineMode(Outline.Mode.SilhouetteOnly);
    }

    #endregion

    #region Interaction Methods

    /// <summary>
    /// Checks if the cup can be interacted with.
    /// </summary>
    /// <returns>Returns true if the cup can be interacted with, otherwise false.</returns>
    bool CanInteract()
    {
        return !isRevealed && !cupManager.IsShuffling() && cupManager.GetSelectedCupIndex() == -1;
    }

    /// <summary>
    /// Sets the outline mode of the cup.
    /// </summary>
    /// <param name="mode">Outline mode.</param>
    void SetOutlineMode(Outline.Mode mode)
    {
        if (currentOutline != null && currentOutline.OutlineMode != mode && this.CanInteract())
        {
            currentOutline.OutlineMode = mode;
        }
    }

    /// <summary>
    /// Moves the cup to the target position.
    /// </summary>
    /// <param name="targetPosition">Target position.</param>
    /// <returns>IEnumerator for coroutine.</returns>
    IEnumerator MoveCup(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, revealSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPosition; // Garante que o copo esteja exatamente na posição final
    }

    /// <summary>
    /// Reveals the cup by moving it upwards.
    /// </summary>
    /// <returns>IEnumerator for coroutine.</returns>
    IEnumerator RevealCup()
    {
        isRevealed = true;
        currentPosition = transform.position;
        Vector3 targetPosition = currentPosition + Vector3.up * revealHeight;
        yield return MoveCup(targetPosition);
    }

    /// <summary>
    /// Lowers the cup back to its original position.
    /// </summary>
    /// <returns>IEnumerator for coroutine.</returns>
    public IEnumerator LowerCup()
    {
        yield return MoveCup(currentPosition);
        isRevealed = false;
        cupManager.SetSelectedCupIndex(-1);
        SetOutlineMode(Outline.Mode.SilhouetteOnly);
    }

    /// <summary>
    /// Checks if the cup is revealed.
    /// </summary>
    /// <returns>Returns true if the cup is revealed, otherwise false.</returns>
    public bool IsRevealed()
    {
        return isRevealed;
    }

    #endregion

}