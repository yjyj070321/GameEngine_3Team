
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class AttackConeIndicator : MonoBehaviour
{
    [Header("공격 범위")]
    public float range = 3f;

    [Range(10f, 180f)]
    public float angle = 90f;

    [Range(3, 60)]
    public int segments = 30;

    [Header("표시 설정")]
    public Material warningMaterial;
    public float groundOffset = 0.05f;

    [Header("애니메이션")]
    [Range(0f, 1f)]
    public float startAlpha = 0.08f;

    Mesh attackMesh;
    MeshRenderer meshRenderer;
    Material indicatorMaterial;

    Color originalColor;
    Vector3 originalScale;
    Coroutine animationCoroutine;

    bool shouldBeVisible;

    void Awake()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();

        attackMesh = new Mesh();
        attackMesh.name = "Attack Warning Cone";
        meshFilter.sharedMesh = attackMesh;

        if (warningMaterial != null)
            meshRenderer.sharedMaterial = warningMaterial;

        indicatorMaterial = meshRenderer.material;
        originalColor = indicatorMaterial.color;

        originalScale = transform.localScale;

        transform.localPosition = new Vector3(
            0f, groundOffset, 0f
        );

        transform.localRotation = Quaternion.identity;

        CreateCone();
        Hide();
    }

    void CreateCone()
    {
        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 3];

        vertices[0] = Vector3.zero;

        float halfAngle = angle * 0.5f;

        for (int i = 0; i <= segments; i++)
        {
            float currentAngle = Mathf.Lerp(
                -halfAngle,
                halfAngle,
                (float)i / segments
            );

            float rad = currentAngle * Mathf.Deg2Rad;

            vertices[i + 1] = new Vector3(
                Mathf.Sin(rad) * range,
                0f,
                Mathf.Cos(rad) * range
            );
        }

        for (int i = 0; i < segments; i++)
        {
            int t = i * 3;
            triangles[t] = 0;
            triangles[t + 1] = i + 1;
            triangles[t + 2] = i + 2;
        }

        attackMesh.vertices = vertices;
        attackMesh.triangles = triangles;
        attackMesh.RecalculateNormals();
        attackMesh.RecalculateBounds();
    }

    // EnemyChaser에서 호출할 수 있는 경고 애니메이션
    public void PlayWarning(float duration)
    {
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        animationCoroutine = StartCoroutine(
            PlayAnimation(duration)
        );
    }

    // 부채꼴이 투명한 상태에서 점점 펼쳐짐
    public IEnumerator PlayAnimation(float duration)
    {
        shouldBeVisible = true;
        meshRenderer.enabled = true;

        transform.localScale = Vector3.zero;

        Color color = originalColor;
        color.a = startAlpha;
        indicatorMaterial.color = color;

        float elapsed = 0f;
        duration = Mathf.Max(0.01f, duration);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float progress = Mathf.Clamp01(elapsed / duration);
            float eased = 1f - Mathf.Pow(1f - progress, 2f);

            transform.localScale = new Vector3(
                originalScale.x * eased,
                originalScale.y,
                originalScale.z * eased
            );

            color.a = Mathf.Lerp(
                startAlpha,
                originalColor.a,
                eased
            );

            indicatorMaterial.color = color;

            yield return null;
        }

        transform.localScale = originalScale;
        indicatorMaterial.color = originalColor;

        animationCoroutine = null;
    }

    // 표시를 숨기고 초기 상태로 복구
    public void Hide()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }

        shouldBeVisible = false;

        if (meshRenderer != null)
            meshRenderer.enabled = false;

        transform.localScale = originalScale;

        if (indicatorMaterial != null)
            indicatorMaterial.color = originalColor;
    }

    // 기존 코드와의 호환용 함수
    public void SetVisible(bool visible)
    {
        if (visible)
        {
            shouldBeVisible = true;
            meshRenderer.enabled = true;
        }
        else
        {
            Hide();
        }
    }

    void LateUpdate()
    {
        if (meshRenderer != null)
            meshRenderer.enabled = shouldBeVisible;
    }

    void OnDisable()
    {
        shouldBeVisible = false;

        if (meshRenderer != null)
            meshRenderer.enabled = false;
    }

    void OnDestroy()
    {
        if (attackMesh != null)
            Destroy(attackMesh);

        if (indicatorMaterial != null)
            Destroy(indicatorMaterial);
    }
}
