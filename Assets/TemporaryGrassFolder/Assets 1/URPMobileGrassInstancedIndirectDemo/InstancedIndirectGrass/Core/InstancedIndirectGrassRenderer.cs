using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.XR;
using UnityEngine.XR.Management;



[ExecuteAlways]
public class InstancedIndirectGrassRenderer : MonoBehaviour
{
    [Header("Settings")]
    public float drawDistance = 125;
    public Material instanceMaterial;

    [Header("Internal")]
    public ComputeShader cullingComputeShader;

    [NonSerialized] public List<Vector3> allGrassPos = new List<Vector3>();
    [HideInInspector] public static InstancedIndirectGrassRenderer instance;
    public List<Matrix4x4> allGrassMatrices = new List<Matrix4x4>();

    private int cellCountX = -1;
    private int cellCountZ = -1;
    private int dispatchCount = -1;

    private float cellSizeX = 10;
    private float cellSizeZ = 10;

    private int instanceCountCache = -1;
    private Mesh cachedGrassMesh;

    private ComputeBuffer allInstancesPosWSBuffer;
    private ComputeBuffer visibleInstancesOnlyPosWSIDBuffer;
    private ComputeBuffer argsBuffer;

    private List<Vector3>[] cellPosWSsList;
    private float minX, minZ, maxX, maxZ;
    private List<int> visibleCellIDList = new List<int>();
    private Plane[] cameraFrustumPlanes = new Plane[6];

    bool shouldBatchDispatch = true;

    private void OnEnable()
    {
        instance = this;
    }

    void LateUpdate()
    {
        UpdateAllInstanceTransformBufferIfNeeded();

        visibleCellIDList.Clear();
        Camera cam = Camera.main;

        float cameraOriginalFarPlane = cam.farClipPlane;
        cam.farClipPlane = drawDistance;
        GeometryUtility.CalculateFrustumPlanes(cam, cameraFrustumPlanes);
        cam.farClipPlane = cameraOriginalFarPlane;

        Profiler.BeginSample("CPU cell frustum culling (heavy)");

        for (int i = 0; i < cellPosWSsList.Length; i++)
        {
            Vector3 centerPosWS = new Vector3(i % cellCountX + 0.5f, 0, i / cellCountX + 0.5f);
            centerPosWS.x = Mathf.Lerp(minX, maxX, centerPosWS.x / cellCountX);
            centerPosWS.z = Mathf.Lerp(minZ, maxZ, centerPosWS.z / cellCountZ);
            float margin = 10f;
            Vector3 sizeWS = new Vector3(Mathf.Abs(maxX - minX) / cellCountX + margin,30f, // add vertical buffer to avoid popping when looking up/down
            Mathf.Abs(maxX - minX) / cellCountX + margin);
            Bounds cellBound = new Bounds(centerPosWS, sizeWS);


            if (GeometryUtility.TestPlanesAABB(cameraFrustumPlanes, cellBound))
            {
                visibleCellIDList.Add(i);
            }
        }

        Profiler.EndSample();

        // === ✨ FIXED VR/Non-VR Matrix Handling ===
        Matrix4x4 v;
        Matrix4x4 p;


        if (XRSettings.enabled)
        {
            v = cam.GetStereoViewMatrix(Camera.StereoscopicEye.Left);
            p = cam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);
        }
        else
        {
            v = cam.worldToCameraMatrix;
            p = cam.projectionMatrix;
        }


        Matrix4x4 vp = p * v;

        visibleInstancesOnlyPosWSIDBuffer.SetCounterValue(0);
        cullingComputeShader.SetMatrix("_VPMatrix", vp);
        cullingComputeShader.SetFloat("_MaxDrawDistance", drawDistance);

        dispatchCount = 0;

        for (int i = 0; i < visibleCellIDList.Count; i++)
        {
            int targetCellFlattenID = visibleCellIDList[i];
            int memoryOffset = 0;
            for (int j = 0; j < targetCellFlattenID; j++)
            {
                memoryOffset += cellPosWSsList[j].Count;
            }

            int jobLength = cellPosWSsList[targetCellFlattenID].Count;

            if (shouldBatchDispatch)
            {
                while ((i < visibleCellIDList.Count - 1) &&
                       (visibleCellIDList[i + 1] <= visibleCellIDList[i] + 1))
                {
                    jobLength += cellPosWSsList[visibleCellIDList[i + 1]].Count;
                    i++;
                }
            }

            cullingComputeShader.SetInt("_StartOffset", memoryOffset);
            cullingComputeShader.Dispatch(0, Mathf.CeilToInt(jobLength / 64f), 1, 1);
            dispatchCount++;
        }

        if (cellPosWSsList == null)
        {
            Debug.LogWarning("cellPosWSsList is null. Grass positions may not have been generated.");
            return;
        }

        ComputeBuffer.CopyCount(visibleInstancesOnlyPosWSIDBuffer, argsBuffer, 4);

        // === ✅ Optional: Slightly Expand Render Bounds to Avoid Pop-in ===
        Bounds renderBound = new Bounds();
        renderBound.SetMinMax(
         new Vector3(minX - 10f, -20f, minZ - 10f),
         new Vector3(maxX + 10f, 20f, maxZ + 10f)
        );

        Graphics.DrawMeshInstancedIndirect(GetGrassMeshCache(), 0, instanceMaterial, renderBound, argsBuffer);
    }

    /*private void OnGUI()
    {
        GUI.contentColor = Color.black;
        GUI.Label(new Rect(200, 0, 400, 60),
            $"After CPU cell frustum culling,\n" +
            $"-Visible cell count = {visibleCellIDList.Count}/{cellCountX * cellCountZ}\n" +
            $"-Real compute dispatch count = {dispatchCount} (saved by batching = {visibleCellIDList.Count - dispatchCount})");

        shouldBatchDispatch = GUI.Toggle(new Rect(400, 400, 200, 100), shouldBatchDispatch, "shouldBatchDispatch");
    }
    */
    void OnDisable()
    {
        if (allInstancesPosWSBuffer != null) allInstancesPosWSBuffer.Release();
        if (visibleInstancesOnlyPosWSIDBuffer != null) visibleInstancesOnlyPosWSIDBuffer.Release();
        if (argsBuffer != null) argsBuffer.Release();

        allInstancesPosWSBuffer = null;
        visibleInstancesOnlyPosWSIDBuffer = null;
        argsBuffer = null;
        instance = null;
    }

    Mesh GetGrassMeshCache()
    {
        if (!cachedGrassMesh)
        {
            cachedGrassMesh = new Mesh();
            Vector3[] verts = new Vector3[3] {
                new Vector3(-0.25f, 0), new Vector3(+0.25f, 0), new Vector3(0f, 1)
            };
            int[] triangles = new int[3] { 2, 1, 0 };
            cachedGrassMesh.SetVertices(verts);
            cachedGrassMesh.SetTriangles(triangles, 0);
        }
        return cachedGrassMesh;
    }

    void UpdateAllInstanceTransformBufferIfNeeded()
    {
        instanceMaterial.SetVector("_PivotPosWS", transform.position);
        instanceMaterial.SetVector("_BoundSize", new Vector2(transform.localScale.x, transform.localScale.z));

        if (instanceCountCache <= allGrassPos.Count &&
            argsBuffer != null &&
            allInstancesPosWSBuffer != null &&
            visibleInstancesOnlyPosWSIDBuffer != null)
            return;

        if (allGrassPos.Count == 0)
        {
            Debug.LogWarning("allGrassPos is empty. Skipping buffer creation.");
            return;
        }

        Debug.Log("UpdateAllInstanceTransformBuffer (Slow)");

        if (allInstancesPosWSBuffer != null) allInstancesPosWSBuffer.Release();
        allInstancesPosWSBuffer = new ComputeBuffer(allGrassPos.Count, sizeof(float) * 3);

        if (visibleInstancesOnlyPosWSIDBuffer != null) visibleInstancesOnlyPosWSIDBuffer.Release();
        visibleInstancesOnlyPosWSIDBuffer = new ComputeBuffer(allGrassPos.Count, sizeof(uint), ComputeBufferType.Append);

        minX = float.MaxValue; minZ = float.MaxValue;
        maxX = float.MinValue; maxZ = float.MinValue;

        foreach (var target in allGrassPos)
        {
            minX = Mathf.Min(target.x, minX);
            minZ = Mathf.Min(target.z, minZ);
            maxX = Mathf.Max(target.x, maxX);
            maxZ = Mathf.Max(target.z, maxZ);
        }

        cellCountX = Mathf.CeilToInt((maxX - minX) / cellSizeX);
        cellCountZ = Mathf.CeilToInt((maxZ - minZ) / cellSizeZ);
        cellPosWSsList = new List<Vector3>[cellCountX * cellCountZ];
        for (int i = 0; i < cellPosWSsList.Length; i++) cellPosWSsList[i] = new List<Vector3>();

        foreach (var pos in allGrassPos)
        {
            int xID = Mathf.Min(cellCountX - 1, Mathf.FloorToInt(Mathf.InverseLerp(minX, maxX, pos.x) * cellCountX));
            int zID = Mathf.Min(cellCountZ - 1, Mathf.FloorToInt(Mathf.InverseLerp(minZ, maxZ, pos.z) * cellCountZ));
            cellPosWSsList[xID + zID * cellCountX].Add(pos);
        }

        int offset = 0;
        Vector3[] allGrassPosWSSortedByCell = new Vector3[allGrassPos.Count];
        for (int i = 0; i < cellPosWSsList.Length; i++)
        {
            for (int j = 0; j < cellPosWSsList[i].Count; j++)
            {
                allGrassPosWSSortedByCell[offset++] = cellPosWSsList[i][j];
            }
        }

        allInstancesPosWSBuffer.SetData(allGrassPosWSSortedByCell);
        instanceMaterial.SetBuffer("_AllInstancesTransformBuffer", allInstancesPosWSBuffer);
        instanceMaterial.SetBuffer("_VisibleInstanceOnlyTransformIDBuffer", visibleInstancesOnlyPosWSIDBuffer);

        if (argsBuffer != null) argsBuffer.Release();
        uint[] args = new uint[5] {
            (uint)GetGrassMeshCache().GetIndexCount(0),
            (uint)allGrassPos.Count,
            (uint)GetGrassMeshCache().GetIndexStart(0),
            (uint)GetGrassMeshCache().GetBaseVertex(0),
            0
        };
        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        argsBuffer.SetData(args);

        instanceCountCache = allGrassPos.Count;

        cullingComputeShader.SetBuffer(0, "_AllInstancesPosWSBuffer", allInstancesPosWSBuffer);
        cullingComputeShader.SetBuffer(0, "_VisibleInstancesOnlyPosWSIDBuffer", visibleInstancesOnlyPosWSIDBuffer);
    }
}

