using UnityEngine;
using System.Collections.Generic;

public class MeshGenerator : MonoBehaviour
{
    public int CellsCount = 20;
    public float WorldSize = 1f;
    public float Thinness = 0.005f;

    private Mesh _mesh;

    private Mesh GenerateGridMesh(int cellsCount, float worldSize, float thinness)
    {
        var centerOffset = -worldSize / 2f;
        var zero = new Vector3(centerOffset, centerOffset);
        var up = new Vector3(centerOffset, centerOffset + thinness);
        var down = new Vector3(centerOffset, centerOffset - thinness);
        var left = new Vector3(centerOffset - thinness, centerOffset);
        var right = new Vector3(centerOffset + thinness, centerOffset);

        var cellSize = worldSize / cellsCount;
        var facesIndicesOffset = 0;
        var vertices = new Vector3[4];
        var colors = new Color[4] { Color.white, Color.white, Color.white, Color.white };

        var meshVertices = new List<Vector3>();
        var meshFaces = new List<int>();
        var meshColors = new List<Color>();

        // generate center crosshair
        vertices[0] = new Vector3(0, cellsCount);
        vertices[1] = new Vector3(cellsCount, 0);
        vertices[2] = new Vector3(cellsCount, cellsCount);
        vertices[3] = new Vector3(0, 0);

        var crosshairVerticesLayout = new int[6 * 2]
        {
            0, 0, 1, 1, 1, 0,
            2, 2, 3, 3, 3, 2,
        };
        var crosshairVerticesOffsetLayout = new Vector3[6 * 2]
        {
            zero, right, up, zero, left, down,
            zero, down, right, zero, up, left,
        };
        var crosshaitVerticesColorLayout = new Color[6 * 2]
{
            colors[0], Color.clear, Color.clear, colors[1], Color.clear, Color.clear,
            colors[2], Color.clear, Color.clear, colors[3], Color.clear, Color.clear,
        };
        var crosshairFacesLayout = new int[3 * 4 * 2]
        {
            0, 1, 2,
            2, 3, 0,
            0, 3, 5,
            3, 4, 5,

            6, 7, 8,
            8, 9, 6,
            6, 9, 11,
            9, 10, 11,
        };

        facesIndicesOffset = meshVertices.Count;
        for (int i = 0, l = crosshairVerticesLayout.Length; i < l; ++i)
        {
            meshVertices.Add(vertices[crosshairVerticesLayout[i]] * cellSize + crosshairVerticesOffsetLayout[i]);
        }
        for (int i = 0, l = crosshairFacesLayout.Length / 3; i < l; ++i)
        {
            meshFaces.Add(facesIndicesOffset + crosshairFacesLayout[i * 3 + 0]);
            meshFaces.Add(facesIndicesOffset + crosshairFacesLayout[i * 3 + 1]);
            meshFaces.Add(facesIndicesOffset + crosshairFacesLayout[i * 3 + 2]);
        }
        meshColors.AddRange(crosshaitVerticesColorLayout);

        // generate cells
        var cellsVerticesLayout = new int[16]
        {
            0, 0, 1, 1, 1, 2,
            2, 2, 3, 3, 3, 0,
            0, 1, 2, 3
        };
        var cellsVerticesOffsetLayout = new Vector3[16]
        {
            zero, right, up, zero, down, right,
            zero, left, down, zero, up, left,
            down, left, up, right
        };
        var cellsVerticesColorLayout = new Color[16]
        {
            colors[0], Color.clear, Color.clear, colors[1], Color.clear, Color.clear,
            colors[2], Color.clear, Color.clear, colors[3], Color.clear, Color.clear,
            Color.clear, Color.clear, Color.clear, Color.clear
        };
        var cellsFacesLayout = new int[3 * 4 * 4]
        {
            0, 1, 2,
            3, 0, 2,
            0, 3, 12,
            12, 3, 13,

            3, 4, 5,
            5, 6, 3,
            3, 6, 13,
            13, 6, 14,

            6, 7, 8,
            8, 9, 6,
            6, 9, 14,
            9, 15, 14,

            9, 10, 11,
            11, 0, 9,
            9, 0, 15,
            0, 12, 15,
        };

        for (int idx = 1; idx < cellsCount; ++idx)
        {
            facesIndicesOffset = meshVertices.Count;

            vertices[0] = new Vector3(cellsCount - idx, cellsCount);
            vertices[1] = new Vector3(cellsCount, cellsCount - idx);
            vertices[2] = new Vector3(idx, 0);
            vertices[3] = new Vector3(0, idx);

            for (int i = 0, l = cellsVerticesLayout.Length; i < l; ++i)
            {
                meshVertices.Add(vertices[cellsVerticesLayout[i]] * cellSize + cellsVerticesOffsetLayout[i]);
            }

            for (int i = 0, l = cellsFacesLayout.Length / 3; i < l; ++i)
            {
                meshFaces.Add(facesIndicesOffset + cellsFacesLayout[i * 3 + 0]);
                meshFaces.Add(facesIndicesOffset + cellsFacesLayout[i * 3 + 1]);
                meshFaces.Add(facesIndicesOffset + cellsFacesLayout[i * 3 + 2]);
            }

            meshColors.AddRange(cellsVerticesColorLayout);
        }

        // fill mesh
        var mesh = new Mesh();
        mesh.name = "dynamicGridMesh";
        mesh.SetVertices(meshVertices);
        mesh.SetTriangles(meshFaces, 0, true);
        mesh.SetColors(meshColors);
        mesh.UploadMeshData(true);

        return mesh;
    }

    private void Awake()
    {
        _mesh = GenerateGridMesh(CellsCount, WorldSize, Thinness);
        GetComponent<MeshFilter>().sharedMesh = _mesh;
    }

    private void OnDestroy()
    {
        if (_mesh) Destroy(_mesh);
    }
}
