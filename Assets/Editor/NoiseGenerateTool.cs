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

        if (currentTexture != null)
        {
            int offset = 30;
            int previewSize = 256;
            GUILayout.BeginArea(new Rect(0,90,previewSize,previewSize + offset));
            // GUILayout.BeginHorizontal();
            GUILayout.Label("Preview:");
            EditorGUI.DrawPreviewTexture(new Rect(0, offset, previewSize, previewSize), currentTexture);
            // GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        if (GUILayout.Button("Save"))
        {
        }
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
                Color color = CalculateColor(x, y);
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();

        return texture;
    }

    private Color CalculateColor(int x, int y)
    {
        switch (CurrentNoiseType)
        {
            case NoiseType.WhiteNoise:
                return WhiteNoise(x, y);
        }

        return Color.black;
    }

    private Color WhiteNoise(int x, int y)
    {
        return seed.Next(0, 2) == 0 ? Color.black : Color.white;
    }
}