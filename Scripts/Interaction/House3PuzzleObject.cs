using UnityEngine;

public class House3PuzzleObject : MonoBehaviour, IInteractable
{
    [Header("퍼즐 번호")]
    public int puzzleNumber = 1;

    [Header("퍼즐 매니저")]
    public House3OrderPuzzle orderPuzzle;

    [Header("상호작용 문구")]
    public string interactText = "<E> CHECK";

    [Header("숨길 대상")]
    public GameObject visualObject;

    private bool hidden = false;

    private void Awake()
    {
        if (visualObject == null)
            visualObject = gameObject;
    }

    public void Interact(PlayerController player)
    {
        if (hidden)
            return;

        if (orderPuzzle == null)
        {
            Debug.LogWarning($"{name}: House3OrderPuzzle이 연결되지 않았습니다.");
            return;
        }

        orderPuzzle.TryInput(puzzleNumber, this);
    }

    public string GetInteractText()
    {
        if (hidden)
            return "";

        return interactText;
    }

    public void HideObject()
    {
        hidden = true;

        if (visualObject != null)
            visualObject.SetActive(false);
    }

    public void ShowObject()
    {
        hidden = false;

        if (visualObject != null)
            visualObject.SetActive(true);
    }
}