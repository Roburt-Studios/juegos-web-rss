
using UnityEngine;
using System.Collections;

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
    private Vector3 originalPosition;

    private Coroutine damageCoroutine;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalPosition = transform.position;
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
        if (spriteRenderer == null)
            return;

        if (!IsAlive)
        {
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }

            transform.position = originalPosition;
            spriteRenderer.color = Color.gray;
            return;
        }

        if (damageCoroutine != null)
            StopCoroutine(damageCoroutine);

        damageCoroutine = StartCoroutine(DamageEffect());
    }

    private IEnumerator DamageEffect()
    {
        spriteRenderer.color = Color.red;

        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Mathf.Sin(elapsed * 60f) * 0.08f;

            transform.position =
                originalPosition + Vector3.right * x;

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.position = originalPosition;
        spriteRenderer.color = Color.white;

        damageCoroutine = null;
    }


}
