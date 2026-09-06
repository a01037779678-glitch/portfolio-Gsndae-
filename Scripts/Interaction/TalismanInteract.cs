using UnityEngine;

public class TalismanInteract : MonoBehaviour, IInteractable
{
    [Header("집2 진행 매니저")]
    public House2SequenceManager house2SequenceManager;

    [Header("상호작용 문구")]
    public string interactText = "<E> 부적 떼기";

    private bool removed = false;

    public void Interact(PlayerController player)
    {
        if (removed)
            return;

        removed = true;

        if (house2SequenceManager != null)
        {
            house2SequenceManager.RemoveTalisman();
        }
        else
        {
            Debug.LogWarning($"{name}: House2SequenceManager가 연결되지 않았습니다.");
        }
    }

    public string GetInteractText()
    {
        return interactText;
    }
}