namespace Editor
{
    public enum NoiseType
    {
        WhiteNoise = 0,
        PerlinNoise = 1,
        ValueNoise = 2,
        SimplexNoise = 3,
        WorleyNoise = 4,
        VoronoiNoise = 5,
        FBMNoise = 6,
    }

    public enum TextureSize
    {
        Size1 = 1,
        Size2 = 1 << 1,
        Size4 = 1 << 2,
        Size8 = 1 << 3,
        Size16 = 1 << 4,
        Size32 = 1 << 5,
        Size64 = 1 << 6,
        Size128 = 1 << 7,
        Size256 = 1 << 8,
        Size512 = 1 << 9,
        Size1024 = 1 << 10,
        Size2048 = 1 << 11,
        Size4196 = 1 << 12,
    }
}