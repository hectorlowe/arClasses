using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARunitLab : MonoBehaviour


{
    int plane_count = 0;
    int point_count = 0;
    public TMP_Text text;
    [SerializeField] private TMP_Text _stateText;
    [SerializeField] private ARPlaneManager _arPlaneManager;
    [SerializeField] private ARPointCloudManager _arPointCloudManager;
    [SerializeField] private TMP_Text _planeText;



    private void OnARSessionStateChanged(ARSessionStateChangedEventArgs args)
    {
        // You could log or process session state changes here if needed
        Debug.Log("AR session state changed: " + args.state);
    }

    private void Start()
    {
        // Subscribe to AR session state changes
        ARSession.stateChanged += OnARSessionStateChanged;

        // Subscribe to the planes changed event
        _arPlaneManager.planesChanged += OnPlanesChanged;

    }
    private void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        // This function will be called whenever the detected planes change (added, updated, or removed)

        // Handle added planes
        foreach (var plane in args.added)
        {
            Debug.Log("Plane added: " + plane.trackableId);
        }

        // Handle updated planes
        foreach (var plane in args.updated)
        {
            Debug.Log("Plane updated: " + plane.trackableId);
        }

        // Handle removed planes
        foreach (var plane in args.removed)
        {
            Debug.Log("Plane removed: " + plane.trackableId);
        }

        // Update the plane count (you could display this in the UI as well)
        plane_count = _arPlaneManager.trackables.count;

        // Update the UI with the new plane count
        _planeText.text = $"Planes: {plane_count}";
    }

    private void Update()
    {
        // gets the number of tracked planes
        plane_count = _arPlaneManager.trackables.count;

        //reset point count
        point_count = 0;

        // Calculate point count from ARPointCloudManager
        foreach (var trackable in _arPointCloudManager.trackables)
        {
            if (trackable.positions is NativeSlice<Vector3> non_null_positions)
            {
                point_count += non_null_positions.Length;
            }
        }

        // Update the UI text
        _planeText.text = $"Planes: {plane_count}";
        text.text = $"Points: {point_count}";

    }
    private void OnDestroy()
    {
        // Unsubscribe from events to avoid memory leaks
        ARSession.stateChanged -= OnARSessionStateChanged;
        _arPlaneManager.planesChanged -= OnPlanesChanged;
    }
}
