using System.IO;
using UnityEngine;

namespace Lessons.L3
{
    // ═══════════════════════════════════════════════════════════════
    // L3 Topic: Save Mesh  (Q4, Q8)
    //
    //   Companion to L3_VertexInteraction.
    //   Provides two independent ways to save the current mesh:
    //
    //   ┌──────────────────────────────────────────────────────────┐
    //   │  A) Save as Unity .mesh asset  (Editor-only, binary)     │
    //   │     Full topology stored — drag onto any MeshFilter.     │
    //   │                                                          │
    //   │  B) Save as .json  (runtime + editor, human-readable)    │
    //   │     Same format as L2_SaveLoadMesh — cross-compatible.   │
    //   └──────────────────────────────────────────────────────────┘
    //
    //   Why two formats?
    //     .mesh — stores everything (verts, tris, UVs, normals).
    //             Best for keeping the asset inside the Unity project.
    //     .json — stores only vertex positions + grid dimensions.
    //             Triangles and UVs are always recomputable for a
    //             regular grid, so they are omitted to keep the file
    //             small and human-readable.  Works at runtime too.
    // ═══════════════════════════════════════════════════════════════
    [RequireComponent(typeof(MeshFilter))]
    public class L3_SaveMesh : MonoBehaviour
    {
        // ── Inspector ────────────────────────────────────────────────

        [Header("Output Paths")]
        [Tooltip("Sub-folder inside Assets/ for the .mesh file.\nExample: Lessons/L3/SavedMeshes")]
        public string meshAssetSubfolder = "Lessons/L3/SavedMeshes";

        [Tooltip("Folder for the JSON file, relative to Application.dataPath.\nExample: Storage")]
        public string jsonFolder = "Storage";

        [Tooltip("Filename without extension — used for both outputs.")]
        public string fileName = "L3_mesh";

        [Header("Grid Dimensions  (must match the source mesh)")]
        [Tooltip("Vertex columns — keeps JSON compatible with L2_SaveLoadMesh.LoadMesh().")]
        public int cols = 21;
        [Tooltip("Vertex rows.")]
        public int rows = 21;


        // ── Serialisable container (same layout as L2_SaveLoadMesh) ──
        //   Using the identical struct means L2_SaveLoadMesh.LoadMesh()
        //   can read files saved by this component without any changes.
        [System.Serializable]
        class MeshData
        {
            public int       cols;
            public int       rows;
            public Vector3[] vertices;
        }

        private MeshFilter _mf;

        void Start() => _mf = GetComponent<MeshFilter>();


        // ─────────────────────────────────────────────────────────────
        // SaveMeshAsset  (Editor-only)
        //
        //   Writes the mesh as a binary .mesh asset inside the project.
        //   Steps:
        //     1. Get the live mesh from MeshFilter.
        //     2. Clone it with Instantiate() so the scene copy is safe.
        //     3. Build an "Assets/..." path for AssetDatabase.
        //     4. Create the output directory if it does not exist.
        //     5. AssetDatabase.CreateAsset() writes the binary file.
        //     6. SaveAssets + Refresh → visible in the Project window.
        //
        //   This method is stripped from runtime builds (#if UNITY_EDITOR).
        //   For in-game saving use SaveMeshAsJson() instead.
        // ─────────────────────────────────────────────────────────────
#if UNITY_EDITOR
        [ContextMenu("Save Mesh as .mesh Asset")]
        public void SaveMeshAsset()
        {
            Mesh mesh = GetCurrentMesh();
            if (mesh == null) return;

            string assetFolder = "Assets/" + meshAssetSubfolder;
            string assetPath   = assetFolder + "/" + fileName + ".mesh";
            string fullFolder  = Application.dataPath + "/" + meshAssetSubfolder;

            if (!Directory.Exists(fullFolder))
                Directory.CreateDirectory(fullFolder);

            // Clone so we don't destroy the live instance
            Mesh clone = UnityEngine.Object.Instantiate(mesh);
            clone.name = fileName;

            UnityEditor.AssetDatabase.CreateAsset(clone, assetPath);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();

            Debug.Log("[L3 SaveMesh] .mesh saved → " + assetPath +
                      "  (" + clone.vertexCount + " vertices)");
        }
#endif


        // ─────────────────────────────────────────────────────────────
        // SaveMeshAsJson  (runtime + editor)
        //
        //   Serialises vertex positions and grid dimensions to JSON.
        //   Steps:
        //     1. Read vertices from the live MeshFilter mesh.
        //     2. Pack into MeshData (identical struct to L2_SaveLoadMesh).
        //     3. JsonUtility.ToJson with prettyPrint=true → JSON string.
        //     4. Write to  dataPath / jsonFolder / fileName.json.
        //
        //   Only vertex positions are stored — triangles and UVs are
        //   deterministic for a regular grid and are recomputed on load
        //   by L2_MeshConcepts.BuildMesh().
        // ─────────────────────────────────────────────────────────────
        [ContextMenu("Save Mesh as JSON")]
        public void SaveMeshAsJson()
        {
            Mesh mesh = GetCurrentMesh();
            if (mesh == null) return;

            MeshData data = new MeshData
            {
                cols     = cols,
                rows     = rows,
                vertices = mesh.vertices,
            };

            string folder   = Application.dataPath + "/" + jsonFolder;
            string filePath = folder + "/" + fileName + ".json";

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            File.WriteAllText(filePath, JsonUtility.ToJson(data, prettyPrint: true));

            Debug.Log("[L3 SaveMesh] JSON saved → " + filePath +
                      "  (" + data.vertices.Length + " vertices)");
        }


        // ─────────────────────────────────────────────────────────────
        // GetCurrentMesh
        //   Internal helper — returns the mesh from MeshFilter.
        //   Prints a clear warning if nothing is assigned.
        // ─────────────────────────────────────────────────────────────
        Mesh GetCurrentMesh()
        {
            if (_mf == null) _mf = GetComponent<MeshFilter>();
            Mesh mesh = _mf != null ? _mf.mesh : null;
            if (mesh == null)
                Debug.LogWarning("[L3 SaveMesh] No mesh found on MeshFilter.");
            return mesh;
        }
    }
}
