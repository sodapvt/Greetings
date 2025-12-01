using System.Collections;
using UnityEngine;
using TMPro;

public class WaveTypeWriter : MonoBehaviour
{
    private TMP_Text textComponent;
    public float characterDelay = 0.05f,exitCharacterDelay=0.02f;
    public float floatDistance = 10f;
    public float floatDuration = 0.3f;
[Header("Scale Animation Settings")]
public float appearStartScale = 1.2f;
public float appearEndScale = 1f;
    private string fullText;

    void Start()
    {
        textComponent = GetComponent<TMP_Text>();
        fullText = textComponent.text;
        textComponent.text = fullText;
        textComponent.ForceMeshUpdate();
        HideAllCharacters();  
    }
    public void ResetText()
    {
        if (textComponent == null)
            return;
        textComponent.text = fullText;
        textComponent.ForceMeshUpdate();
        HideAllCharacters();
    }

    void HideAllCharacters()
    {
        textComponent.ForceMeshUpdate();
        TMP_TextInfo textInfo = textComponent.textInfo;

        for (int i = 0; i < fullText.Length; i++)
        {
            if (!textInfo.characterInfo[i].isVisible)
                continue;

            int meshIndex = textInfo.characterInfo[i].materialReferenceIndex;
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;

            Color32[] vertexColors = textInfo.meshInfo[meshIndex].colors32;

            for (int j = 0; j < 4; j++)
            {
                vertexColors[vertexIndex + j].a = 0;
            }
        }

        textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }
    public void PlayTextAnimation()
    {
        StartCoroutine(TypeText());
    }
    public IEnumerator TypeText()
    {
        FlowHandler.textWaveFinished = false;
        TMP_TextInfo textInfo = textComponent.textInfo;

        for (int i = 0; i < fullText.Length; i++)
        {
            if (!textInfo.characterInfo[i].isVisible)
            {
                yield return new WaitForSeconds(characterDelay);
                continue;
            }

            StartCoroutine(FadeAndFloatCharacter(i));
            yield return new WaitForSeconds(characterDelay);
        }
        FlowHandler.textWaveFinished = true;
    }

    IEnumerator FadeAndFloatCharacter(int charIndex)
    {
        TMP_TextInfo textInfo = textComponent.textInfo;
        int materialIndex = textInfo.characterInfo[charIndex].materialReferenceIndex;
        int vertexIndex = textInfo.characterInfo[charIndex].vertexIndex;

        Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;
        Color32[] colors = textInfo.meshInfo[materialIndex].colors32;

        // Cache the original vertex positions
        Vector3[] originalVertices = new Vector3[4];
        for (int i = 0; i < 4; i++)
        {
            originalVertices[i] = vertices[vertexIndex + i];
        }

        float elapsed = 0f;

        while (elapsed < floatDuration)
        {
            float t = elapsed / floatDuration;
            float easedT = Mathf.SmoothStep(0, 1, t);

            Vector3 offset = Vector3.up * floatDistance * easedT;
            byte alpha = (byte)(255 * easedT);

            for (int i = 0; i < 4; i++)
            {
                vertices[vertexIndex + i] = originalVertices[i] + offset;
                colors[vertexIndex + i].a = alpha;
            }

            textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices | TMP_VertexDataUpdateFlags.Colors32);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Set final state
        for (int i = 0; i < 4; i++)
        {
            vertices[vertexIndex + i] = originalVertices[i] + Vector3.up * floatDistance;
            colors[vertexIndex + i].a = 255;
        }

        textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices | TMP_VertexDataUpdateFlags.Colors32);
    }
    
    // ------------------- EXIT LOGIC ------------------------

    public void PlayExitAnimation()
    {
        StartCoroutine(ExitText());
    }

    public IEnumerator ExitText()
    {
        TMP_TextInfo textInfo = textComponent.textInfo;

        for (int i = 0; i < fullText.Length; i++)
        {
            if (!textInfo.characterInfo[i].isVisible)
            {
                yield return new WaitForSeconds(exitCharacterDelay);
                continue;
            }

            StartCoroutine(FadeAndSinkCharacter(i));
            yield return new WaitForSeconds(exitCharacterDelay);
        }

    }

    IEnumerator FadeAndSinkCharacter(int charIndex)
    {
        TMP_TextInfo textInfo = textComponent.textInfo;
        int materialIndex = textInfo.characterInfo[charIndex].materialReferenceIndex;
        int vertexIndex = textInfo.characterInfo[charIndex].vertexIndex;

        Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;
        Color32[] colors = textInfo.meshInfo[materialIndex].colors32;

        Vector3[] originalVertices = new Vector3[4];
        for (int i = 0; i < 4; i++)
        {
            originalVertices[i] = vertices[vertexIndex + i];
        }

        float elapsed = 0f;

        while (elapsed < floatDuration)
        {
            float t = elapsed / floatDuration;
            float easedT = Mathf.SmoothStep(0, 1, t);

            Vector3 offset = Vector3.down * floatDistance * easedT;
            byte alpha = (byte)(255 * (1 - easedT)); // Fade out

            for (int i = 0; i < 4; i++)
            {
                vertices[vertexIndex + i] = originalVertices[i] + offset;
                colors[vertexIndex + i].a = alpha;
            }

            textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices | TMP_VertexDataUpdateFlags.Colors32);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Final invisible state
        for (int i = 0; i < 4; i++)
        {
            vertices[vertexIndex + i] = originalVertices[i] + Vector3.down * floatDistance;
            colors[vertexIndex + i].a = 0;
        }

        textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices | TMP_VertexDataUpdateFlags.Colors32);
    }

     // ------------------- NEW: SCALE ANIMATION ------------------------

    public void PlayScaleAppearAnimation()
    {
        StartCoroutine(ScaleAppearText());
    }

    IEnumerator ScaleAppearText()
    {
        FlowHandler.textWaveFinished = false;
        TMP_TextInfo textInfo = textComponent.textInfo;

        for (int i = 0; i < fullText.Length; i++)
        {
            if (!textInfo.characterInfo[i].isVisible)
            {
                yield return new WaitForSeconds(characterDelay);
                continue;
            }

            StartCoroutine(ScaleInCharacter(i));
            yield return new WaitForSeconds(characterDelay);
        }

        FlowHandler.textWaveFinished = true;
    }

    IEnumerator ScaleInCharacter(int charIndex)
    {
        TMP_TextInfo textInfo = textComponent.textInfo;
        int materialIndex = textInfo.characterInfo[charIndex].materialReferenceIndex;
        int vertexIndex = textInfo.characterInfo[charIndex].vertexIndex;

        Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;
        Color32[] colors = textInfo.meshInfo[materialIndex].colors32;

        Vector3[] originalVertices = new Vector3[4];
        for (int i = 0; i < 4; i++)
            originalVertices[i] = vertices[vertexIndex + i];

        Vector3 center = (originalVertices[0] + originalVertices[2]) / 2f;

        float elapsed = 0f;
        while (elapsed < floatDuration)
        {
            float t = elapsed / floatDuration;
            float easedT = Mathf.SmoothStep(0, 1, t);
            float scale = Mathf.Lerp(0f, 1f, easedT);
            byte alpha = (byte)(255 * easedT);

            for (int i = 0; i < 4; i++)
            {
                vertices[vertexIndex + i] = center + (originalVertices[i] - center) * scale;
                colors[vertexIndex + i].a = alpha;
            }

            textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices | TMP_VertexDataUpdateFlags.Colors32);
            elapsed += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < 4; i++)
        {
            vertices[vertexIndex + i] = originalVertices[i];
            colors[vertexIndex + i].a = 255;
        }

        textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices | TMP_VertexDataUpdateFlags.Colors32);
    }

    public void PlayScaleExitAnimation()
    {
        StartCoroutine(ScaleExitText());
    }

    IEnumerator ScaleExitText()
    {
        TMP_TextInfo textInfo = textComponent.textInfo;

        for (int i = 0; i < fullText.Length; i++)
        {
            if (!textInfo.characterInfo[i].isVisible)
            {
                yield return new WaitForSeconds(exitCharacterDelay);
                continue;
            }

            StartCoroutine(ScaleOutCharacter(i));
            yield return new WaitForSeconds(exitCharacterDelay);
        }
    }

    IEnumerator ScaleOutCharacter(int charIndex)
    {
        TMP_TextInfo textInfo = textComponent.textInfo;
        int materialIndex = textInfo.characterInfo[charIndex].materialReferenceIndex;
        int vertexIndex = textInfo.characterInfo[charIndex].vertexIndex;

        Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;
        Color32[] colors = textInfo.meshInfo[materialIndex].colors32;

        Vector3[] originalVertices = new Vector3[4];
        for (int i = 0; i < 4; i++)
            originalVertices[i] = vertices[vertexIndex + i];

        Vector3 center = (originalVertices[0] + originalVertices[2]) / 2f;

        float elapsed = 0f;
        while (elapsed < floatDuration)
        {
            float t = elapsed / floatDuration;
            float easedT = Mathf.SmoothStep(0, 1, t);
            float scale = Mathf.Lerp(1f, 0f, easedT);
            byte alpha = (byte)(255 * (1 - easedT));

            for (int i = 0; i < 4; i++)
            {
                vertices[vertexIndex + i] = center + (originalVertices[i] - center) * scale;
                colors[vertexIndex + i].a = alpha;
            }

            textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices | TMP_VertexDataUpdateFlags.Colors32);
            elapsed += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < 4; i++)
        {
            vertices[vertexIndex + i] = center;
            colors[vertexIndex + i].a = 0;
        }

        textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices | TMP_VertexDataUpdateFlags.Colors32);
    }
    public void PlayPopScaleAnimation()
{
    StartCoroutine(PopScaleAppearText());
}

IEnumerator PopScaleAppearText()
{
    FlowHandler.textWaveFinished = false;
    TMP_TextInfo textInfo = textComponent.textInfo;

    for (int i = 0; i < fullText.Length; i++)
    {
        if (!textInfo.characterInfo[i].isVisible)
        {
            yield return new WaitForSeconds(characterDelay);
            continue;
        }

        StartCoroutine(PopScaleInCharacter(i));
        yield return new WaitForSeconds(characterDelay);
    }

    FlowHandler.textWaveFinished = true;
}

IEnumerator PopScaleInCharacter(int charIndex)
{
    TMP_TextInfo textInfo = textComponent.textInfo;
    int materialIndex = textInfo.characterInfo[charIndex].materialReferenceIndex;
    int vertexIndex = textInfo.characterInfo[charIndex].vertexIndex;

    Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;
    Color32[] colors = textInfo.meshInfo[materialIndex].colors32;

    Vector3[] originalVertices = new Vector3[4];
    for (int i = 0; i < 4; i++)
        originalVertices[i] = vertices[vertexIndex + i];

    Vector3 center = (originalVertices[0] + originalVertices[2]) / 2f;

    float elapsed = 0f;
    while (elapsed < floatDuration)
    {
        float t = elapsed / floatDuration;
        float easedT = Mathf.SmoothStep(0, 1, t);

        // Scale from appearStartScale → appearEndScale (e.g. 1.2 → 1)
        float scale = Mathf.Lerp(appearStartScale, appearEndScale, easedT);
        byte alpha = (byte)(255 * easedT);

        for (int i = 0; i < 4; i++)
        {
            vertices[vertexIndex + i] = center + (originalVertices[i] - center) * scale;
            colors[vertexIndex + i].a = alpha;
        }

        textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices | TMP_VertexDataUpdateFlags.Colors32);
        elapsed += Time.deltaTime;
        yield return null;
    }

    // Final state (normal scale & visible)
    for (int i = 0; i < 4; i++)
    {
        vertices[vertexIndex + i] = originalVertices[i];
        colors[vertexIndex + i].a = 255;
    }

    textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices | TMP_VertexDataUpdateFlags.Colors32);
}
}