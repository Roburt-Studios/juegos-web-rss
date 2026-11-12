using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class CanvasFader : MonoBehaviour
{
    [SerializeField, Min(0.01f)]
    private float defaultDuration = 0.35f;

    [SerializeField]
    private bool fadeInOnStart;

    private CanvasGroup canvasGroup;
    private readonly Queue<FadeRequest> fadeQueue = new Queue<FadeRequest>();
    private Coroutine queueCoroutine;

    private struct FadeRequest
    {
        public float targetAlpha;
        public float duration;
        public Action onComplete;

        public FadeRequest(float targetAlpha, float duration, Action onComplete)
        {
            this.targetAlpha = targetAlpha;
            this.duration = duration;
            this.onComplete = onComplete;
        }
    }

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        if (fadeInOnStart)
        {
            canvasGroup.alpha = 0f;
            FadeIn();
        }
    }

    public void FadeIn()
    {
        FadeTo(1f, defaultDuration);
    }

    public void FadeOut()
    {
        FadeTo(0f, defaultDuration);
    }

    public void FadeIn(float duration)
    {
        FadeTo(1f, duration);
    }

    public void FadeOut(float duration)
    {
        FadeTo(0f, duration);
    }

    public void FadeTo(float targetAlpha, float duration)
    {
        EnqueueFade(targetAlpha, duration, null);
    }

    public IEnumerator FadeToAndWait(float targetAlpha, float duration)
    {
        bool completed = false;
        EnqueueFade(targetAlpha, duration, () => completed = true);

        while (!completed)
            yield return null;
    }

    private void EnqueueFade(float targetAlpha, float duration, Action onComplete)
    {
        fadeQueue.Enqueue(new FadeRequest(Mathf.Clamp01(targetAlpha), Mathf.Max(0f, duration), onComplete));

        if (queueCoroutine == null)
            queueCoroutine = StartCoroutine(ProcessQueueRoutine());
    }

    private IEnumerator ProcessQueueRoutine()
    {
        while (fadeQueue.Count > 0)
        {
            FadeRequest request = fadeQueue.Dequeue();
            yield return FadeRoutine(request.targetAlpha, request.duration);
            request.onComplete?.Invoke();
        }

        queueCoroutine = null;
    }

    private IEnumerator FadeRoutine(float targetAlpha, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = false;

        if (duration <= 0f)
        {
            canvasGroup.alpha = targetAlpha;
        }
        else
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
                yield return null;
            }

            canvasGroup.alpha = targetAlpha;
        }

        bool visible = targetAlpha > 0f;
        canvasGroup.blocksRaycasts = visible;
        canvasGroup.interactable = visible;
    }
}
