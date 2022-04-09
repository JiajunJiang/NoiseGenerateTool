using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Editor;
using UnityEditor;
using UnityEngine;
using Random = System.Random;


public class NoiseGenerateTool : EditorWindow
{
    [MenuItem("Tools/NoiseGenerateTool")]
    public static void OpenWindow()
    {
        //EditorWindow.GetWindow<FightPanelEditor>();
        var windows = EditorWindow.GetWindow(typeof(NoiseGenerateTool), false, "Generate", true);
        windows.Show();
    }

    private NoiseType CurrentNoiseType { get; set; }

    private TextureSize TextureSize { get; set; } = TextureSize.Size256;

    private Random seed = new Random();

    private Texture2D currentTexture { get; set; }

    private float Scale { get; set; } = 1.0f;

    private int RandomOffset { get; set; }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        CurrentNoiseType = (NoiseType) EditorGUILayout.EnumPopup("Noise Choose", CurrentNoiseType);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        TextureSize = (TextureSize) EditorGUILayout.EnumPopup("Texture Size", TextureSize);
        GUILayout.EndHorizontal();

        if (GUILayout.Button("GenerateTexture"))
        {
            currentTexture = GenerateTexture();
        }

        int offset = 30;
        int previewSize = 256;

        if (currentTexture != null)
        {
            GUILayout.BeginArea(new Rect(0, 65, previewSize, previewSize + offset));
            // GUILayout.BeginHorizontal();
            GUILayout.Label("Preview:");
            EditorGUI.DrawPreviewTexture(new Rect(0, offset, previewSize, previewSize), currentTexture);
            // GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        GUILayout.BeginArea(new Rect(0, 80 + previewSize + offset, 200, 200));
        if (CurrentNoiseType == NoiseType.PerlinNoise)
        {
            var lastScale = Scale;
            GUILayout.BeginHorizontal();
            GUILayout.Label("Scale:");
            Scale = EditorGUILayout.Slider(Scale, 0.0f, 100.0f);
            GUILayout.EndHorizontal();
            if (lastScale != Scale)
            {
                currentTexture = GenerateTexture();
            }

            if (GUILayout.Button("Random"))
            {
                RandomOffset = seed.Next(0, (int)TextureSize * (int)TextureSize);
                currentTexture = GenerateTexture();
            }
        }
        else if (CurrentNoiseType == NoiseType.ValueNoise)
        {
            if (GUILayout.Button("Random"))
            {
                RandomOffset = seed.Next(0, (int)TextureSize * (int)TextureSize);
                currentTexture = GenerateTexture();
            }
        }

        if (GUILayout.Button("Save"))
        {
            Save();
        }

        GUILayout.EndArea();
    }

    private void Save()
    {
        string fileName = $"{CurrentNoiseType.ToString()}_{DateTime.Now.Ticks}";
        string path = $"{Application.dataPath}/{fileName}.png";
        byte[] bytes = currentTexture.EncodeToPNG();
        File.WriteAllBytes(path, bytes);//写入文件里面
        Debug.Log($"Save {path} Success!");
        AssetDatabase.Refresh();
    }

    private Texture2D GenerateTexture()
    {
        seed = new Random(DateTime.Now.Millisecond * (int) DateTime.Now.Ticks);
        int size = (int) TextureSize;
        Texture2D texture = new Texture2D(size, size);

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                Color color = CalculateColor(x, y, size);
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();

        return texture;
    }

    private Color CalculateColor(int x, int y, int size)
    {
        switch (CurrentNoiseType)
        {
            case NoiseType.WhiteNoise:
                return WhiteNoise();
            case NoiseType.PerlinNoise:
                return PerlinNoise(x, y, size);
            case NoiseType.ValueNoise:
                return ValueNoise(x, y);
        }

        return Color.black;
    }

    private Color WhiteNoise()
    {
        return seed.Next(0, 2) == 0 ? Color.black : Color.white;
    }

    private Color PerlinNoise(int x, int y, int size)
    {
        float sample = Mathf.PerlinNoise((float) x / size * Scale + RandomOffset,
            (float) y / size * Scale + RandomOffset);
        return new Color(sample, sample, sample);
    }

    #region ValueNoise

    private Vector2 X1Y0 = new Vector2(1, 0);
    private Vector2 X0Y1 = new Vector2(0, 1);

    private Color ValueNoise(int x, int y)
    {
        Vector2 i = floor(new Vector2(x + RandomOffset, y + RandomOffset));
        Vector2 f = fract(new Vector2(x + RandomOffset, y + RandomOffset));

        float a = random(i);
        float b = random(i + X1Y0);
        float c = random(i + X0Y1);
        float d = random(i + Vector2.one);

        Vector2 u = f * f * (new Vector2(3.0f, 3.0f) - 2.0f * f);

        float sample = mix(mix(a, b, u.x), mix(b, c, u.x), u.y);
        return new Color(sample, sample, sample);
    }

    #endregion


    #region Math

    private Vector2 floor(Vector2 v2)
    {
        return new Vector2(Mathf.Floor(v2.x), Mathf.Floor(v2.y));
    }

    private float fract(float value)
    {
        return value - Mathf.Floor(value);
    }

    private Vector2 fract(Vector2 v2)
    {
        return new Vector2(fract(v2.x), fract(v2.y));
    }

    private float random(Vector2 v2)
    {
        return fract(Mathf.Sin(Vector2.Dot(v2, new Vector2(13.0909f, 783.342f))) * 423234.323f);
    }

    private float mix(float x, float y, float level)
    {
        return x * (1 - level) + y * level;
    }

    #endregion
}