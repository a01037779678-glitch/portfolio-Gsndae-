using UnityEngine;

public class MissingPosterBoard : MonoBehaviour, IInteractable
{
    [Header("상호작용 문구")]
    public string interactText = "<E> 전단지 읽기";

    [Header("대사")]
    [TextArea(2, 5)]
    public string[] dialogues;

    [Header("한 번 읽은 후 문구")]
    public bool rememberReadState = true;
    public string afterReadInteractText = "<E> 다시 읽기";

    private bool hasRead = false;
    private int dialogueIndex = 0;

    public void Interact(PlayerController player)
    {
        if (dialogues == null || dialogues.Length == 0)
        {
            Debug.LogWarning($"{name}: 출력할 대사가 없습니다.");
            return;
        }

        if (StoryDialogueUI.Instance == null)
        {
            Debug.LogWarning("StoryDialogueUI가 씬에 없습니다.");
            return;
        }

        StoryDialogueUI.Instance.ShowDialogue(dialogues[dialogueIndex]);

        dialogueIndex++;

        if (dialogueIndex >= dialogues.Length)
            dialogueIndex = dialogues.Length - 1;

        hasRead = true;
    }

    public string GetInteractText()
    {
        if (rememberReadState && hasRead)
            return afterReadInteractText;

        return interactText;
    }
}