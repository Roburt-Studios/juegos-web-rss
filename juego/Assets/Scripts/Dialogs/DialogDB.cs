using System.Collections.Generic;
using UnityEngine;

public class DialogDB
{
    private readonly Dictionary<string, Dialog> dialogs = new();

    public void Initialize()
    {
        TextAsset csvFile = Resources.Load<TextAsset>("Dialogs");
        if (csvFile == null)
        {
            Debug.LogError("No se encontró Resources/Dialogs.csv");
            return;
        }

        string[] rows = csvFile.text.Split('\n');
        for (int i = 1; i < rows.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(rows[i]))
                continue;

            string[] columns = rows[i].TrimEnd('\r').Split(';');
            if (columns.Length < 6)
            {
                Debug.LogWarning($"Diálogo: fila {i + 1} incompleta.");
                continue;
            }

            string dialogId = columns[0].Trim();
            string nodeId = columns[1].Trim();
            string type = columns[2].Trim().ToLowerInvariant();
            string characterId = columns[3].Trim();
            string text = columns[4].Trim();
            string nextId = columns[5].Trim();
            string failureNextId = columns.Length > 6 ? columns[6].Trim() : "";
            int difficulty = 0;
            if (columns.Length > 7 && !string.IsNullOrWhiteSpace(columns[7]) &&
                (!int.TryParse(columns[7].Trim(), out difficulty) || difficulty < 2 || difficulty > 12))
            {
                Debug.LogWarning($"Diálogo: dificultad inválida en fila {i + 1}; debe estar entre 2 y 12.");
                continue;
            }
            string tag = columns.Length > 8 ? columns[8].Trim() : "";

            if (!dialogs.TryGetValue(dialogId, out Dialog dialog))
            {
                dialog = new Dialog(dialogId);
                dialogs.Add(dialogId, dialog);
            }

            if (type == "option")
            {
                if (dialog.TryGetNode(nodeId, out Dialog.Node choice) && choice.isChoice)
                {
                    if (difficulty > 0 && string.IsNullOrEmpty(failureNextId))
                        Debug.LogWarning($"Diálogo: opción con tirada sin ruta de fallo en fila {i + 1}.");
                    else
                        choice.options.Add(new Dialog.Option(text, nextId, failureNextId, difficulty));
                }
                else
                    Debug.LogWarning($"Diálogo: opción sin nodo choice en fila {i + 1}.");
            }
            else if (type == "line" || type == "choice")
            {
                if (!dialog.AddNode(new Dialog.Node(nodeId, characterId, text, nextId, type == "choice", tag)))
                    Debug.LogWarning($"Diálogo: nodo duplicado {dialogId}/{nodeId}.");
            }
            else
            {
                Debug.LogWarning($"Diálogo: tipo desconocido '{type}' en fila {i + 1}.");
            }
        }

        Debug.Log($"[Database] Cargados {dialogs.Count} diálogos.");
    }

    public Dialog GetDialog(string id)
    {
        if (dialogs.TryGetValue(id, out Dialog dialog))
            return dialog;

        Debug.LogWarning($"Dialog ID {id} no encontrado.");
        return null;
    }
}
