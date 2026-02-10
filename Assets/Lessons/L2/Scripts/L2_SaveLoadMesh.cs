using System.IO;
using UnityEngine;

namespace Lessons.L2
{
    // ═══════════════════════════════════════════════════════════════
    // L2 Topic: Save / Load mesh to JSON  (Q8)
    //
    //   Unity's JsonUtility cannot serialise Vector3[] directly, so
    //   we wrap the arrays in a plain [Serializable] class (MeshData).
    //
    //   What gets saved:
    //     • cols / rows   – vertex grid dimensions (needed to rebuild)
    //     • vertices[]    – all vertex positions in local space
    //
    //   What does NOT get saved (can be recomputed from cols × rows):
    //     • triangles[] – always the same winding for a flat grid
    //     • uv[]        – always (x/(cols-1), y/(rows-1)) per vertex
    //
    //   File location:  Application.dataPath/Storage/L2_mesh.json
    //                   (same folder used by the main SaveLoad.cs)
    // ═══════════════════════════════════════════════════════════════
    [RequireComponent(typeof(MeshFilter))]
    public class L2_SaveLoadMesh : MonoBehaviour
    {
        [Header("Filename without extension")]
        public string fileName = "L2_mesh";

        [Header("Grid dimensions (set by whatever generated this mesh)")]
        public int cols = 21;
        public int rows = 21;

        // ── Serialisable container ───────────────────────────────────
        [System.Serializable]
        class MeshData
        {
            public int       cols;
            public int       rows;
            public Vector3[] vertices;
        }

        private MeshFilter _mf;

        void Start() => _mf = GetComponent<MeshFilter>();

        string StoragePath => Application.dataPath + "/Storage";
        string FilePath    => StoragePath + "/" + fileName + ".json";


        // ─────────────────────────────────────────────────────────────
        // SAVE  –  mesh vertices → JSON file
        //
        //   Steps:
        //     1. Read vertices from the MeshFilter's current mesh.
        //     2. Pack them into a MeshData object.
        //     3. JsonUtility.ToJson  →  human-readable JSON string.
        //     4. Write string to disk.
        // ─────────────────────────────────────────────────────────────
        [ContextMenu("Save Mesh to JSON")]
        public void SaveMesh()
        {
            Mesh mesh = _mf != null ? _mf.sharedMesh : null;
            if (mesh == null)
            {
                Debug.LogWarning("[L2 SaveLoad] No mesh on MeshFilter.");
                return;
            }

            MeshData data = new MeshData
            {
                cols     = cols,
                rows     = rows,
                vertices = mesh.vertices,   // snapshot of current positions
            };

            if (!Directory.Exists(StoragePath))
                Directory.CreateDirectory(StoragePath);

            // prettyPrint = true makes the JSON human-readable
            File.WriteAllText(FilePath, JsonUtility.ToJson(data, prettyPrint: true));

            Debug.Log("[L2 SaveLoad] Saved " + mesh.vertexCount +
                      " vertices to: " + FilePath);
        }


        // ─────────────────────────────────────────────────────────────
        // LOAD  –  JSON file → rebuild mesh and apply vertices
        //
        //   Steps:
        //     1. Read JSON string from disk.
        //     2. JsonUtility.FromJson  →  MeshData object.
        //     3. Rebuild a flat grid mesh from saved cols × rows.
        //     4. Overwrite its vertex positions with the saved data.
        //     5. Recalculate bounds and normals.
        // ─────────────────────────────────────────────────────────────
        [ContextMenu("Load Mesh from JSON")]
        public void LoadMesh()
        {
            if (!File.Exists(FilePath))
            {
                Debug.LogWarning("[L2 SaveLoad] File not found: " + FilePath);
                return;
            }

            MeshData data = JsonUtility.FromJson<MeshData>(
                                File.ReadAllText(FilePath));

            // Rebuild the base topology from saved dimensions
            Mesh mesh = L2_MeshConcepts.BuildMesh(
                "LoadedMesh", data.cols, data.rows, 1f, 1f, data.vertices);

            if (_mf == null) _mf = GetComponent<MeshFilter>();
            _mf.mesh = mesh;

            // Sync inspector fields
            cols = data.cols;
            rows = data.rows;

            Debug.Log("[L2 SaveLoad] Loaded " + data.vertices.Length +
                      " vertices (" + data.cols + "×" + data.rows + ") from: " + FilePath);
        }
    }
}
