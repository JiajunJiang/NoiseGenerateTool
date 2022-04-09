using System;
using System.Collections;
using System.Collections.Generic;
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
                RandomOffset = seed.Next(0, 100);
                currentTexture = GenerateTexture();
            }
        }
        if (GUILayout.Button("Save"))
        {
            
        }
        GUILayout.EndArea();
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
        }

        return Color.black;
    }

    private Color WhiteNoise()
    {
        return seed.Next(0, 2) == 0 ? Color.black : Color.white;
    }

    private Color PerlinNoise(int x, int y, int size)   
    {
        float sample = Mathf.PerlinNoise((float) x / size * Scale + RandomOffset, (float) y / size * Scale + RandomOffset);
        return new Color(sample, sample, sample);
    }
}