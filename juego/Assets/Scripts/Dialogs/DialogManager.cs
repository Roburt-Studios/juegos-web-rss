using System;
using System.Collections.Generic;
using Patterns.ServiceLocator;
using Patterns.ServiceLocator.Interfaces;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogManager : MonoBehaviour, IService
{
    [Serializable]
    private struct CharacterSpriteEntry
    {
        [Tooltip("Identificador del personaje tal y como aparece en los diálogos.")]
        public string characterId;

        public Sprite sprite;
    }

    private Dialog currentDialog;
    private int currentLineIndex = 0;
    private bool inDialog = false;
    private float lastAdvanceTime = 0f;
    private float advanceCooldown = 0.5f; // Segundos de espera entre clicks

    public DialogUIController uiController;

    [SerializeField]
    [Tooltip("Sprites asociados a cada identificador de personaje.")]
    private List<CharacterSpriteEntry> characterSprites = new();

    private readonly Dictionary<string, Sprite> charactersSpriteMap = new();

    public DialogDB dialogDB;

    public EventHandler<String> OnDialogEnd;

    public InputReader input;

    private void Awake()
    {
        RebuildCharacterSpriteMap();
    }

    private void OnValidate()
    {
        RebuildCharacterSpriteMap();
    }

    public void Initialize()
    {
        // Inicializar la base de datos de diálogos
        dialogDB = new DialogDB();
        dialogDB.Initialize();

        uiController = GameObject.FindFirstObjectByType<DialogUIController>();
        uiController.Initialize();

        // Validar que uiController esté asignado
        if (uiController == null)
        {
            Debug.LogError(
                "DialogManager: uiController no está asignado. Asigna el DialogUIController en el Inspector."
            );
        }
    }

    public bool TryGetCharacterSprite(string characterId, out Sprite sprite)
    {
        return charactersSpriteMap.TryGetValue(characterId, out sprite);
    }

    private void RebuildCharacterSpriteMap()
    {
        charactersSpriteMap.Clear();

        foreach (CharacterSpriteEntry entry in characterSprites)
        {
            if (string.IsNullOrWhiteSpace(entry.characterId) || entry.sprite == null)
                continue;

            charactersSpriteMap[entry.characterId] = entry.sprite;
        }
    }

    private void Update()
    {
        if (inDialog && Keyboard.current != null && Mouse.current != null)
        {
            if (
                Keyboard.current.tabKey.wasPressedThisFrame
                || Mouse.current.leftButton.wasPressedThisFrame
            )
            {
                AdvanceDialog();
            }
        }
    }

    public void StartDialog(string dialogId)
    {
        input.DisableAllInput();
        // Validar que uiController esté asignado

        if (uiController == null)
        {
            //Debug.LogError("DialogManager: No se puede iniciar diálogo. uiController no está asignado.");
            return;
        }

        currentDialog = dialogDB.GetDialog(dialogId);

        if (currentDialog == null)
        {
            return;
        }

        inDialog = true;
        currentLineIndex = 0;
        ShowCurrentLine();
    }

    public void AdvanceDialog()
    {
        if (!inDialog || currentDialog == null)
            return;

        // Si ha pasado muy poco tiempo desde el último click, ignoramos este
        if (Time.time - lastAdvanceTime < advanceCooldown)
            return;

        lastAdvanceTime = Time.time;

        currentLineIndex++;

        if (currentLineIndex < currentDialog.GetLineCount())
        {
            ShowCurrentLine();
        }
        else
        {
            EndDialog();
        }
    }

    private void ShowCurrentLine()
    {
        if (currentDialog != null && uiController != null)
        {
            Tuple<string, string> dialogInstance = currentDialog.lines[currentLineIndex];
            uiController.ShowText(
                dialogInstance.Item2,
                charactersSpriteMap[dialogInstance.Item1],
                currentLineIndex,
                currentDialog.GetLineCount()
            );
        }
    }

    public void EndDialog()
    {
        inDialog = false;
        currentLineIndex = 0;

        if (uiController != null)
        {
            uiController.HideUI();
        }

        // Esperamos un momento antes de devolver el control para que el
        // click que cerró el diálogo no haga que el jugador dispare o salte.
        Invoke("RestoreGameplayControl", 0.1f);
        OnDialogEnd?.Invoke(this, currentDialog.id);
        currentDialog = null;
    }

    private void RestoreGameplayControl()
    {
        Invoke("UnlockPlayer", 0.5f);
    }

    // Retorna si actualmente está en un diálogo
    public bool IsInDialog()
    {
        return inDialog;
    }

    private void UnlockPlayer()
    {
        input.EnableGameplayInput();
    }
}
