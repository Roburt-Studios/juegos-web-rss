
using UnityEngine;

public class BattleUnit : MonoBehaviour
{
    [Header("Datos del personaje")]
    public string unitName = "Combatiente";
    public int maxHP = 100;
    public int currentHP = 100;
    public int attackPower = 15;
    public int skillPower = 25;

    public bool IsAlive => currentHP > 0;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
    }

    public void TakeDamage(int damage)
    {
        if (!IsAlive) return;

        currentHP = Mathf.Max(0, currentHP - damage);
        UpdateAppearance();
    }

    public void Heal(int amount)
    {
        if (!IsAlive) return;

        currentHP = Mathf.Min(maxHP, currentHP + amount);
        UpdateAppearance();
    }

    public void SetSelected(bool selected)
    {
        if (spriteRenderer == null) return;

        if (!IsAlive)
            spriteRenderer.color = Color.gray;
        else
            spriteRenderer.color = selected
                ? Color.yellow
                : Color.white;
    }

    private void UpdateAppearance()
    {
        if (spriteRenderer != null && !IsAlive)
            spriteRenderer.color = Color.gray;
    }
}
