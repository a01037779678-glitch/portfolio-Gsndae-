using UnityEngine;
using UnityEngine.Events;

public class SlidingDoor : MonoBehaviour, IInteractable
{
    [Header("이동 설정")]
    public Vector3 openOffset = new Vector3(2f, 0f, 0f);
    public float speed = 3f;

    [Header("잠금 설정")]
    public bool isLocked = true;
    public string requiredKeyId = "AKey";

    [Header("문 사운드")]
    public AudioSource audioSource;
    public AudioClip openSound;
    public AudioClip closeSound;
    public AudioClip lockedSound;

    [Header("문 열림 이벤트")]
    public UnityEvent onDoorOpened;

    private Vector3 closedPosition;
    private Vector3 openPosition;

    private bool isOpen = false;
    private bool isMoving = false;
    private bool openEventPlayed = false;

    private void Start()
    {
        closedPosition = transform.position;
        openPosition = closedPosition + transform.TransformVector(openOffset);

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!isMoving)
            return;

        Vector3 target = isOpen ? openPosition : closedPosition;

        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, target) < 0.01f)
        {
            transform.position = target;
            isMoving = false;
        }
    }

    public void Interact(PlayerController player)
    {
        if (isMoving)
            return;

        if (isLocked)
        {
            if (player == null || !player.HasKey(requiredKeyId))
            {
                PlayDoorSound(lockedSound);
                Debug.Log("NEED KEY");
                return;
            }

            isLocked = false;
            Debug.Log("DOOR OPENS (UNLOCKING)");
        }

        isOpen = !isOpen;

        if (isOpen)
        {
            PlayDoorSound(openSound);

            if (!openEventPlayed)
            {
                openEventPlayed = true;
                onDoorOpened?.Invoke();

                Debug.Log("SlidingDoor: 문 열림 이벤트 실행");
            }
        }
        else
        {
            PlayDoorSound(closeSound);
        }

        isMoving = true;
    }

    public void OpenDoor()
    {
        if (isMoving)
            return;

        if (isOpen)
            return;

        isLocked = false;
        isOpen = true;

        PlayDoorSound(openSound);

        if (!openEventPlayed)
        {
            openEventPlayed = true;
            onDoorOpened?.Invoke();

            Debug.Log("SlidingDoor: 문 열림 이벤트 실행");
        }

        isMoving = true;

        Debug.Log("SlidingDoor: 이벤트로 문 열림");
    }

    public void CloseDoor()
    {
        if (isMoving)
            return;

        if (!isOpen)
            return;

        isOpen = false;

        PlayDoorSound(closeSound);

        isMoving = true;

        Debug.Log("SlidingDoor: 이벤트로 문 닫힘");
    }
    public void LockDoor()
    {
        isLocked = true;
        Debug.Log("SlidingDoor: 문 잠김");
    }

    public void UnlockDoor()
    {
        isLocked = false;
        Debug.Log("SlidingDoor: 문 잠금 해제");
    }
    private void PlayDoorSound(AudioClip clip)
    {
        if (audioSource == null)
            return;

        if (clip == null)
            return;

        audioSource.PlayOneShot(clip);
    }

    public string GetInteractText()
    {
        if (isMoving)
            return "...";

        if (isLocked)
            return "<E> LOCKED";

        return isOpen ? "<E> CLOSE" : "<E> OPEN";
    }
}