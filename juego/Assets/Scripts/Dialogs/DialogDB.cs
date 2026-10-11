using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DialogDB
{
    private Dictionary<string, Dialog> dialogs = new Dictionary<string, Dialog>();

    public void Initialize()
    {
        LoadCSV();
    }

    void LoadCSV()
    {
        TextAsset csvFile = Resources.Load<TextAsset>("Dialogs");

        if (csvFile == null)
        {
            Debug.LogError("No se encontró el archivo de diálogos en Resources!");
            return;
        }
        // Aqí cambiamos el idioma de los dialogos:

        // Separar por saltos de línea
        string[] rows = csvFile.text.Split('\n');

        for (int i = 1; i < rows.Length; i++) // Saltamos cabecera
        {
            if (string.IsNullOrWhiteSpace(rows[i]))
                continue;

            // Separar por pipe
            string[] columns = rows[i].Split(';');

            if (columns.Length < 3)
                continue;

            string id = columns[0].Trim();
            string character = columns[1].Trim();
            string text = columns[2].Trim();

            Debug.Log("id: " + id + " character: " + character + " text: " + text);

            // Si el ID no existe en el diccionario, creamos un nuevo Dialog
            if (!dialogs.ContainsKey(id))
            {
                dialogs.Add(id, new Dialog(id));
            }

            // Agregamos la línea al Dialog correspondiente
            dialogs[id].AddLine(character, text);
        }
        Debug.Log($"[Database] Cargados {dialogs.Count} diálogos grupales.");
    }

    // Método para obtener un diálogo completo por su ID
    public Dialog GetDialog(string id)
    {
        if (dialogs.TryGetValue(id, out Dialog d))
        {
            return d;
        }
        Debug.LogWarning($"Dialog ID {id} no encontrado.");
        return null;
    }
}
