using UnityEngine;
using UnityEngine.Events;

public class KeyPickup : MonoBehaviour, IInteractable
{
    public string keyId = "AKey";
    public string interactText = "<E> GET KEY";

    [Header("È¹µæ ¼º°ø ½Ã ÀÌº¥Æ®")]
    public UnityEvent onPickedUp;

    [Header("È¹µæ ½Ã È°¼ºÈ­ÇÒ ¿ÀºêÁ§Æ®")]
    public GameObject activateObject;

    public void Interact(PlayerController player)
    {
        if (player == null)
        {
            Debug.LogWarning($"{name}: PlayerController°¡ ¾ø½À´Ï´Ù.");
            return;
        }

        player.AddKey(keyId);
        onPickedUp?.Invoke();
        if (activateObject != null)
            activateObject.SetActive(true);
        

        Destroy(gameObject);

        Debug.Log($"Å° È¹µæ: {keyId}");
    }

    public string GetInteractText()
    {
        return interactText;
    }
}