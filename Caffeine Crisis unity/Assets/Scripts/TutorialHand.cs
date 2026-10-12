using System.Collections;
using UnityEngine;

public class TutorialHand : MonoBehaviour
{
    private RectTransform handRect;
    private Coroutine currentAnimation;

    void Awake()
    {
        handRect = GetComponent<RectTransform>();
    }

    // Eli hedef objenin üzerine konumlandırır ve zıplama efekti başlatır
    public void PointTo(RectTransform target)
    {
        if (target == null) return;

        StopCurrentAnimation();
        gameObject.SetActive(true);
        
        // Eli hedefin pozisyonuna getir
        handRect.position = target.position;

        // Sağ-Alt (+30, -30) yerine Sol-Yukarı (-30, +30) olarak güncellendi:
        handRect.anchoredPosition += new Vector2(-30f, 50f);

        currentAnimation = StartCoroutine(BounceRoutine());
    }

    // Eli başlangıç noktasından hedef noktaya doğru sürükleme hareketi yaptırır
    public void DragTo(RectTransform startTarget, RectTransform endTarget)
    {
        if (startTarget == null || endTarget == null) return;

        StopCurrentAnimation();
        gameObject.SetActive(true);

        currentAnimation = StartCoroutine(DragRoutine(startTarget, endTarget));
    }

    public void Hide()
    {
        StopCurrentAnimation();
        gameObject.SetActive(false);
    }

    void StopCurrentAnimation()
    {
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
            currentAnimation = null;
        }
    }

    // Tıklama dürtüsü veren zıplama animasyonu
    IEnumerator BounceRoutine()
    {
        Vector2 basePosition = handRect.anchoredPosition;
        while (true)
        {
            float bounce = Mathf.Sin(Time.time * 6f) * 15f;
            handRect.anchoredPosition = basePosition + new Vector2(bounce, -bounce);
            yield return null;
        }
    }

    // Sürükleme animasyonu (Bardağı Müşteriye Çekme)
    IEnumerator DragRoutine(RectTransform start, RectTransform end)
    {
        while (true)
        {
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * 1.5f; // Sürükleme hızı
                handRect.position = Vector3.Lerp(start.position, end.position, t);
                yield return null;
            }
            yield return new WaitForSeconds(0.3f); // Sona ulaşınca kısa bekleme
        }
    }
}