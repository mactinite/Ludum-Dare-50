using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainTexturizer : MonoBehaviour
{

    [SerializeField] private float maxAngle = 90;
    [SerializeField]private Terrain t;
    // Blend the two terrain textures according to the steepness of
    // the slope at each point.
    [ContextMenu("Generate Textures")]
    void GenerateSplatMap()
    {
        var terrainData = t.terrainData;
        float[,,] map = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, 2];

        // For each point on the alphamap...
        for (int y = 0; y < terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < terrainData.alphamapWidth; x++)
            {
                // Get the normalized terrain coordinate that
                // corresponds to the the point.
                float normX = x * 1.0f / (terrainData.alphamapWidth - 1);
                float normY = y * 1.0f / (terrainData.alphamapHeight - 1);

                // Get the steepness value at the normalized coordinate.
                var angle = t.terrainData.GetSteepness(normX, normY);

                // Steepness is given as an angle, 0..90 degrees. Divide
                // by 90 to get an alpha blending value in the range 0..1.
                var frac = angle / maxAngle;
                map[y, x, 0] = (float)frac;
                map[y, x, 1] = (float)(1 - frac);
            }
        }
        terrainData.SetAlphamaps(0, 0, map);
    }

    public void PostGenerate()
    {
        GenerateSplatMap();
    }
}
