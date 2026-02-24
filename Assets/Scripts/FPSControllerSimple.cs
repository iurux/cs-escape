using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(CharacterController))]
public class FPSControllerSimple : MonoBehaviour
{
    public Transform cam;

    [Header("Movement")]
    public float moveSpeed = 4.5f;
    public float lookSensitivity = 0.12f;
    public float gravity = -20f;

    [Header("Interaction")]
    public float interactDistance = 3f;
    public Image interactPrompt;
    public TMP_Text keyText;

    [Header("Inventory UI")]
    public GameObject inventoryPanel;

    [Header("Book Reading")]
    public GameObject bookCanvas;     // 你那个整体暗屏 + 白纸的 Canvas
    public TMP_Text bookText;         // 白纸上的文字

    [Header("Systems")]
    public InventorySimple inventory;
    public DialogueUI dialogueUI;
    public CircuitPuzzleManager circuitPuzzle;

    float pitch;
    Vector3 vel;
    CharacterController cc;
    bool inventoryOpen;
    bool uiLock;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        interactPrompt.gameObject.SetActive(false);
        inventoryPanel.SetActive(false);

        if (inventory == null)
            inventory = GetComponent<InventorySimple>();

        if (circuitPuzzle == null)
        {
            circuitPuzzle = FindObjectOfType<CircuitPuzzleManager>();
        }

        if (circuitPuzzle != null && circuitPuzzle.puzzleCanvasPanel != null)
        {
            circuitPuzzle.puzzleCanvasPanel.SetActive(false);
        }
    }

    void Update()
    {
        HandleInventoryToggle();

        if (inventoryOpen || uiLock || isReadingBook) return;

        // Check if the Puzzle is active 
        if (circuitPuzzle != null && circuitPuzzle.puzzleCanvasPanel.activeSelf) 
        {
            return; 
        }

        HandleLook();
        HandleMove();
        HandleGravity();
        HandleInteractionRay();
    }

    // ================== Camera ==================
    void HandleLook()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        float mx = mouseDelta.x * lookSensitivity;
        float my = mouseDelta.y * lookSensitivity;

        transform.Rotate(0, mx, 0);
        pitch = Mathf.Clamp(pitch - my, -80f, 80f);
        cam.localEulerAngles = new Vector3(pitch, 0, 0);
    }

    // ================== Movement ==================
    void HandleMove()
    {
        Vector2 move2 = Vector2.zero;
        if (Keyboard.current.wKey.isPressed) move2.y += 1;
        if (Keyboard.current.sKey.isPressed) move2.y -= 1;
        if (Keyboard.current.dKey.isPressed) move2.x += 1;
        if (Keyboard.current.aKey.isPressed) move2.x -= 1;

        move2 = Vector2.ClampMagnitude(move2, 1f);
        Vector3 move = transform.right * move2.x + transform.forward * move2.y;
        cc.Move(move * moveSpeed * Time.deltaTime);
    }

    void HandleGravity()
    {
        if (cc.isGrounded && vel.y < 0) vel.y = -1f;
        vel.y += gravity * Time.deltaTime;
        cc.Move(vel * Time.deltaTime);
    }

    // ================== Interaction ==================
    void HandleInteractionRay()
    {
        interactPrompt.gameObject.SetActive(false);

        Vector3 rayOrigin = cam.position + cam.forward * 0.2f;
        Ray ray = new Ray(rayOrigin, cam.forward);

        Debug.DrawRay(rayOrigin, cam.forward * interactDistance, Color.red);

        if (!Physics.Raycast(ray, out RaycastHit hit, interactDistance))
            return;
        Debug.Log("Ray hit: " + hit.collider.name + 
          " | Tag: " + hit.collider.tag);

        // ---------- Pickup ----------
        if (hit.collider.CompareTag("Pickup"))
        {
            ShowPrompt("F");

            if (Keyboard.current.fKey.wasPressedThisFrame)
            {
                // flashlight logic
                FlashlightPickupTrigger uvPickup = hit.collider.GetComponent<FlashlightPickupTrigger>() ?? 
                                         hit.collider.GetComponentInParent<FlashlightPickupTrigger>();
                if (uvPickup != null) {
                    uvPickup.Interact();
                    return; 
                }

                NormalFlashlightPickup normalPickup = hit.collider.GetComponent<NormalFlashlightPickup>() ?? 
                                             hit.collider.GetComponentInParent<NormalFlashlightPickup>();
                if (normalPickup != null) { 
                    normalPickup.Interact(); 
                    return; 
                }

                EmptyCupPickup cupPickup = hit.collider.GetComponent<EmptyCupPickup>() ?? 
                                           hit.collider.GetComponentInParent<EmptyCupPickup>();
                if (cupPickup != null) 
                { 
                    cupPickup.Interact(); 
                    return; 
                }

                PickupItem p = hit.collider.GetComponentInParent<PickupItem>();

                string itemId = (p != null && !string.IsNullOrEmpty(p.itemId))
                    ? p.itemId
                    : hit.collider.name;

                Sprite icon = p != null ? p.icon : null;

                if (inventory != null)
                    inventory.Add(itemId, icon);

                Debug.Log("Picked up: " + itemId);

                // Student Card pickup
                if (itemId == "Student Card" && dialogueUI != null)
                {
                    dialogueUI.StartDialogue(new string[]
                    {
                        "Here is my Student Card..."
                    });
                }

                // Headphone pickup
                if (itemId == "Headphone" && dialogueUI != null)
                {
                    dialogueUI.StartDialogue(new string[]
                    {
                        "Here are my headphones..."
                    });
                }

                // Phone pickup
                if (itemId == "Phone" && dialogueUI != null)
                {
                    dialogueUI.StartDialogue(new string[]
                    {
                        "Here is my phone..."
                    });
                }

                // Pen pickup
                if (itemId == "Pen" && dialogueUI != null)
                {
                    dialogueUI.StartDialogue(new string[]
                    {
                        "Here is my pen..."
                    });
                }

                // Laptop pickup
                if (itemId == "Laptop" && dialogueUI != null)
                {
                    dialogueUI.StartDialogue(new string[]
                    {
                        "Here is my laptop...",
                        "Why is it even here..."
                    });
                }

                Destroy(hit.collider.gameObject);
            }
        }
        // ---------- Interact ----------
        else if (hit.collider.CompareTag("Interact"))
        {
            if (hit.collider.name.Contains("ElectricityBox") && circuitPuzzle != null && circuitPuzzle.IsSolved)
            {
                return; 
            }

            ShowPrompt("E");

            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                Debug.Log("Interacted with: " + hit.collider.name);

                // ① Card Reader
                CardReaderInteractable reader =
                    hit.collider.GetComponent<CardReaderInteractable>() ??
                    hit.collider.GetComponentInParent<CardReaderInteractable>();

                if (reader != null)
                {
                    reader.TryInteract(dialogueUI);
                    return;
                }

                // ① Door
                DoorInteractable door =
                    hit.collider.GetComponentInParent<DoorInteractable>();

                if (door != null)
                {
                    Debug.Log("Door found: " + hit.collider.name);
                    door.TryInteract(inventory, dialogueUI);
                    return;
                }

                // ② Computer (Maze)
                ComputerInteract computer =
                    hit.collider.GetComponent<ComputerInteract>() ??
                    hit.collider.GetComponentInParent<ComputerInteract>();

                if (computer != null)
                {
                    computer.Interact();
                    return;
                }

                // ③ Circuit Puzzle (Pipe)
                if (hit.collider.name.Contains("ElectricityBox"))
                {
                    if (circuitPuzzle != null)
                    {
                        // 이미 퍼즐이 풀렸다면 여기서 즉시 중단
                        if (circuitPuzzle.IsSolved) 
                        {
                            // 상호작용 프롬프트(E)도 아예 안 보이게
                            interactPrompt.gameObject.SetActive(false); 
                            return; 
                        }

                        // 퍼즐이 아직 안 풀렸을 때만 실행되는 로직
                        circuitPuzzle.OpenPuzzle();
                        
                        interactPrompt.gameObject.SetActive(false); // E 글씨 숨기기
                        this.enabled = false; // 퍼즐 중에는 플레이어 컨트롤러(회전/이동)를 잠시 끕니다.

                        // show mouse cursor to play puzzle
                        Cursor.lockState = CursorLockMode.None;
                        Cursor.visible = true;
                        return;
                    }
                }

                // ③ Water dispenser interaction (snackoverflow puzzle)
                WaterDispenserInteract dispenser = hit.collider.GetComponent<WaterDispenserInteract>() ??
                                                   hit.collider.GetComponentInParent<WaterDispenserInteract>();
                
                if (dispenser != null)
                {
                    dispenser.Interact(); // 정수기의 Interact() 함수를 호출하여 물을 채움
                    return;
                }

                // ④ Elevator Button
                ElevatorButtonInteract elevator =
                    hit.collider.GetComponent<ElevatorButtonInteract>() ??
                    hit.collider.GetComponentInParent<ElevatorButtonInteract>();

                if (elevator != null)
                {
                    elevator.Interact();
                    return;
                }

                // === Terminal Computer (Area A) ===
                TerminalInteractable terminalComputer =
                    hit.collider.GetComponent<TerminalInteractable>() ??
                    hit.collider.GetComponentInParent<TerminalInteractable>();

                if (terminalComputer != null)
                {
                    terminalComputer.Interact(this);
                    return;
                }

                // ================== Book  ==================
                if (TryOpenBook(hit))
                    return;

                // ③ Fallback
                Debug.Log("No interactable script found on object.");
            }
        }
    }

    void ShowPrompt(string key)
    {
        interactPrompt.gameObject.SetActive(true);
        keyText.text = key;
    }

    // ================== Inventory ==================
    void HandleInventoryToggle()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            inventoryOpen = !inventoryOpen;
            inventoryPanel.SetActive(inventoryOpen);

            Time.timeScale = inventoryOpen ? 0f : 1f;
            Cursor.lockState = inventoryOpen
                ? CursorLockMode.None
                : CursorLockMode.Locked;
            Cursor.visible = inventoryOpen;

            // 🔥 只在“打开”时记录
            if (inventoryOpen)
            {
                LogInventoryOpened();
            }
        }
    }

    void LogInventoryOpened()
    {
        float timeSinceGameStart =
            Time.time - AnalyticsManager.gameStartTime;

        AnalyticsManager.LogEvent("inventory_open", new InventoryData
        {
            time_since_game_start = timeSinceGameStart
        });

        Debug.Log("[Analytics] inventory_open logged");
    }

    [System.Serializable]
    public class InventoryData
    {
        public float time_since_game_start;
    }

    // Method to close the puzzle UI and resume player control
    public void ClosePuzzleAndResumeGame()
    {
        // 1. Re-enable this script to restore player movement and look
        this.enabled = true; 
        
        // 2. Relock the mouse cursor for FPS gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // 3. Deactivate the puzzle UI Panel (NOT the script object)
        if (circuitPuzzle != null && circuitPuzzle.puzzleCanvasPanel != null)
        {
            circuitPuzzle.puzzleCanvasPanel.SetActive(false);
        }
            
        Debug.Log("Circuit(pipe) game Resumed & Lights should be ON");
    }

    // ============================================================
    // ================== Book Reading System =====================
    // ============================================================

    [Header("Book UI (Reading)")]
    public GameObject bookCanvasPanel;     // 你的 Book Canvas/Panel（初始关闭）
    public TMP_Text bookContentText;       // 白纸上的文字 TMP_Text
    public string closeKeyHint = "Press E to close";
    int bookOpenedFrame = -1;

    bool isReadingBook = false;
    BookReadable currentBook;
    float savedTimeScale = 1f;

    bool TryOpenBook(RaycastHit hit)
    {
        BookReadable book =
            hit.collider.GetComponent<BookReadable>() ??
            hit.collider.GetComponentInParent<BookReadable>();

        if (book == null) return false;

        currentBook = book;  // 记录是哪本书
        OpenBookUI(book.content);
        return true;
    }


    void OpenBookUI(string content)
    {
        Debug.Log("OpenBookUI called");

        if (bookCanvasPanel == null || bookContentText == null)
        {
            Debug.LogWarning("Book UI references missing: bookCanvasPanel / bookContentText");
            return;
        }

        isReadingBook = true;

        // Pause world（也能兼容你之后别的暂停，比如 inventory）
        savedTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        // UI 显示
        bookCanvasPanel.SetActive(true);

        // 不需要 title：直接显示内容即可
        bookContentText.text = content;

        // 鼠标放开，方便玩家阅读
        //Cursor.lockState = CursorLockMode.None;
        //Cursor.visible = true;

        // 隐藏交互提示
        interactPrompt.gameObject.SetActive(false);

        bookOpenedFrame = Time.frameCount;
    }

    public void CloseBookUI()
    {
        if (!isReadingBook) return;

        isReadingBook = false;

        if (bookCanvasPanel != null)
            bookCanvasPanel.SetActive(false);

        Time.timeScale = savedTimeScale;

        // 只对特定书触发
        if (currentBook != null &&
            currentBook.triggerDialogueAfterReading &&
            dialogueUI != null)
        {
            dialogueUI.StartDialogue(currentBook.afterReadingLines);
        }

        currentBook = null;
    }

    public void SetUILock(bool locked)
    {
        uiLock = locked;
    }

    void LateUpdate()
    {
        if (!isReadingBook) return;

        // 防止同一帧按E打开又立刻触发关闭
        if (Time.frameCount == bookOpenedFrame) return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CloseBookUI();
        }
    }

}