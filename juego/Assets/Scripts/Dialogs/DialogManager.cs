using System;
using System.Collections.Generic;
using Patterns.ServiceLocator;
using Patterns.ServiceLocator.Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DialogManager : MonoBehaviour, IService
{
    [Serializable]
    private struct CharacterSpriteEntry
    {
        public string characterId;
        public Sprite sprite;
    }

    [SerializeField] private List<CharacterSpriteEntry> characterSprites = new();
    private readonly Dictionary<string, Sprite> charactersSpriteMap = new();
    private Dialog currentDialog;
    private Dialog.Node currentNode;
    private Dialog.Option pendingRollOption;
    private string pendingRollNextId;
    private DiceManager diceManager;
    private bool inDialog;
    private float lastAdvanceTime;
    private int lastCompletionFrame = -1;
    private const float AdvanceCooldown = 0.5f;

    public DialogUIController uiController;
    public DialogDB dialogDB;
    public EventHandler<string> OnDialogEnd;
    public InputReader input;

    private void Awake() => RebuildCharacterSpriteMap();
    private void OnValidate() => RebuildCharacterSpriteMap();

    public void Initialize()
    {
        dialogDB = new DialogDB();
        dialogDB.Initialize();

        if (uiController == null)
            uiController = FindFirstObjectByType<DialogUIController>();

        if (uiController == null)
            Debug.LogError("DialogManager: falta DialogUIController.");
        else
            uiController.Initialize(this);
    }

    public bool TryGetCharacterSprite(string characterId, out Sprite sprite) =>
        charactersSpriteMap.TryGetValue(characterId, out sprite);

    private void RebuildCharacterSpriteMap()
    {
        charactersSpriteMap.Clear();
        foreach (CharacterSpriteEntry entry in characterSprites)
        {
            if (!string.IsNullOrWhiteSpace(entry.characterId) && entry.sprite != null)
                charactersSpriteMap[entry.characterId] = entry.sprite;
        }
    }

    private void Update()
    {
        if (!inDialog || Keyboard.current == null)
            return;

        if (Keyboard.current.tabKey.wasPressedThisFrame ||
            (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame))
            AdvanceDialog();
    }

    public void StartDialog(string dialogId)
    {
        if (inDialog || uiController == null || dialogDB == null)
            return;

        currentDialog = dialogDB.GetDialog(dialogId);
        if (currentDialog == null || !currentDialog.TryGetNode(currentDialog.startNodeId, out currentNode))
            return;

        CancelInvoke(nameof(RestoreGameplayControl));
        CancelInvoke(nameof(UnlockPlayer));
        input.DisableAllInput();
        inDialog = true;
        pendingRollOption = null;
        pendingRollNextId = null;
        lastCompletionFrame = -1;
        lastAdvanceTime = Time.time;
        ShowCurrentNode();
    }

    public void AdvanceDialog()
    {
        if (!inDialog || currentNode == null || Time.frameCount == lastCompletionFrame)
            return;

        if (uiController.CompleteCurrentText())
        {
            lastCompletionFrame = Time.frameCount;
            lastAdvanceTime = Time.time - AdvanceCooldown;
            return;
        }

        if (Time.time - lastAdvanceTime < AdvanceCooldown)
            return;

        lastAdvanceTime = Time.time;
        if (pendingRollNextId != null)
        {
            string nextId = pendingRollNextId;
            pendingRollNextId = null;
            GoToNode(nextId);
            return;
        }

        if (currentNode.isChoice)
            return;

        if (string.Equals(currentNode.tag, "combate", StringComparison.OrdinalIgnoreCase))
        {
            FinishDialog(false);
            SceneManager.LoadScene("CombatScene");
            return;
        }

        GoToNode(currentNode.nextId);
    }

    public void SelectChoice(int index)
    {
        if (!inDialog || currentNode == null || !currentNode.isChoice ||
            index < 0 || index >= currentNode.options.Count ||
            Time.frameCount == lastCompletionFrame)
            return;

        if (uiController.CompleteCurrentText())
        {
            lastCompletionFrame = Time.frameCount;
            return;
        }

        Dialog.Option option = currentNode.options[index];
        lastAdvanceTime = Time.time;
        if (option.difficulty > 0)
        {
            pendingRollOption = option;
            uiController.ShowRollPrompt(option.difficulty);
        }
        else
        {
            GoToNode(option.nextId);
        }
    }

    public void RollForChoice()
    {
        if (!inDialog || pendingRollOption == null)
            return;

        if (diceManager == null)
            diceManager = GetComponent<DiceManager>();
        if (diceManager == null)
            diceManager = gameObject.AddComponent<DiceManager>();

        DiceRoll roll = diceManager.Roll();
        int difficulty = pendingRollOption.difficulty;
        bool success = roll.Total >= difficulty;
        pendingRollNextId = success ? pendingRollOption.nextId : pendingRollOption.failureNextId;
        pendingRollOption = null;
        lastCompletionFrame = Time.frameCount;
        lastAdvanceTime = Time.time;
        uiController.ShowRollResult(roll, difficulty, success);
    }

    private void GoToNode(string nodeId)
    {
        if (string.IsNullOrEmpty(nodeId))
        {
            EndDialog();
            return;
        }

        if (!currentDialog.TryGetNode(nodeId, out currentNode))
        {
            Debug.LogError($"Diálogo {currentDialog.id}: nodo {nodeId} no encontrado.");
            EndDialog();
            return;
        }

        ShowCurrentNode();
    }

    private void ShowCurrentNode()
    {
        charactersSpriteMap.TryGetValue(currentNode.characterId, out Sprite sprite);
        uiController.ShowNode(currentNode, sprite);
    }

    public void EndDialog()
    {
        FinishDialog(true);
    }

    private void FinishDialog(bool restoreInput)
    {
        if (!inDialog)
            return;

        string finishedDialogId = currentDialog.id;
        inDialog = false;
        currentNode = null;
        currentDialog = null;
        pendingRollOption = null;
        pendingRollNextId = null;
        uiController.HideUI();
        if (restoreInput)
            Invoke(nameof(RestoreGameplayControl), 0.1f);
        OnDialogEnd?.Invoke(this, finishedDialogId);
    }

    private void RestoreGameplayControl() => Invoke(nameof(UnlockPlayer), 0.5f);
    private void UnlockPlayer() => input.EnableGameplayInput();
    public bool IsInDialog() => inDialog;
}
