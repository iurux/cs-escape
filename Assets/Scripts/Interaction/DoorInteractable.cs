using System.Collections;
using UnityEngine;

public class DoorInteractable : MonoBehaviour
{
    [Header("Door Rotation")]
    public Transform doorToRotate;
    public float openAngleY = 90f;
    public float openSpeed = 2f;

    public enum RequirementType
    {
        None,
        RequiresItem,
        RequiresCardReaderAccess,
        RequiresFlashlight     // ✅ 新增
    }

    [Header("Requirement")]
    public RequirementType requirement = RequirementType.None;

    // === Item Requirement ===
    public string requiredItemId = "Student Card";
    public string failDialogue = "I forgot to pick up my student ID card.";

    // === Card Reader Requirement ===
    [Header("Card Reader")]
    public CardReaderInteractable cardReader;
    public string needSwipeDialogue = "I need to use the card reader first.";

    // === Flashlight Requirement ===
    [Header("Flashlight Requirement")]
    public string flashlightItemId = "Flashlight";
    public string tooDarkDialogue =
        "It's too dark inside...\nI can't see anything without some light.";

    bool isOpen = false;
    bool isAnimating = false;

    Quaternion closedRot;
    Quaternion openRot;

    void Awake()
    {
        if (doorToRotate == null)
            doorToRotate = transform;

        closedRot = doorToRotate.localRotation;
        openRot = closedRot * Quaternion.Euler(0f, openAngleY, 0f);
    }

    public void TryInteract(InventorySimple inv, DialogueUI dialogueUI)
    {
        if (isAnimating) return;

        // ===== 条件检查 =====

        // 1️⃣ 需要普通物品
        if (requirement == RequirementType.RequiresItem)
        {
            if (inv == null || !inv.Has(requiredItemId))
            {
                dialogueUI?.StartDialogue(new string[] { failDialogue });
                return;
            }
        }

        // 2️⃣ 需要刷卡
        if (requirement == RequirementType.RequiresCardReaderAccess)
        {
            if (cardReader == null || !cardReader.accessGranted)
            {
                dialogueUI?.StartDialogue(new string[] { needSwipeDialogue });
                return;
            }
        }

        // 3️⃣ 需要手电筒
        if (requirement == RequirementType.RequiresFlashlight)
        {
            if (inv == null || !inv.Has(flashlightItemId))
            {
                dialogueUI?.StartDialogue(new string[]
                {
                    "It's too dark inside...",
                    "I can't see anything without some light."
                });
                return;
            }
        }

        // ===== 条件通过，开门 =====
        Quaternion target = isOpen ? closedRot : openRot;
        StartCoroutine(AnimateDoor(target));
    }

    IEnumerator AnimateDoor(Quaternion target)
    {
        isAnimating = true;

        Quaternion start = doorToRotate.localRotation;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * openSpeed;
            doorToRotate.localRotation =
                Quaternion.Slerp(start, target, t);
            yield return null;
        }

        doorToRotate.localRotation = target;
        isOpen = !isOpen;
        isAnimating = false;
    }
}