using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTrailManager : MonoBehaviour
{
    public GameObject trailPrefab; // Mesh prefab for the trail
    public Material trailMaterial; // Material for the trail (with fade effect)

    public float trailInterval = 0.1f; // Time between each trail segment
    public float trailDuration = 2f; // Duration the trail segments will remain visible

    private GameObject _currentTrail;
    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;
    private Mesh _trailMesh;
    private float _trailTimer;

    private void Start()
    {
        // Create a new instance of the trail prefab
        _currentTrail = Instantiate(trailPrefab, transform.position, Quaternion.identity);
        _meshFilter = _currentTrail.GetComponent<MeshFilter>();
        _meshRenderer = _currentTrail.GetComponent<MeshRenderer>();
        _trailMesh = new Mesh();
        _meshFilter.mesh = _trailMesh;
        _meshRenderer.material = trailMaterial;

        // Start the timer
        _trailTimer = trailInterval;
    }

    private void Update()
    {
        _trailTimer -= Time.deltaTime;

        if (_trailTimer <= 0f)
        {
            _trailTimer = trailInterval;
            UpdateTrail();
        }

        trailDuration -= Time.deltaTime;
        if (trailDuration <= 0f)
        {
            Destroy(_currentTrail);
            //Destroy(gameObject);
        }
    }

    private void UpdateTrail()
    {
        // Logic to update the trail's mesh vertices, indices, etc.
        // Here you'd update the mesh to follow the enemy's movement
        // For simplicity, I'll create a single trail vertex at the enemy's current position

        Vector3[] vertices = { transform.position };
        int[] indices = { 0 };

        _trailMesh.Clear();
        _trailMesh.vertices = vertices;
        _trailMesh.SetIndices(indices, MeshTopology.Points, 0);
        _trailMesh.RecalculateBounds();
    }
}
