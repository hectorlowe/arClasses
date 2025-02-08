using System;
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
    [SerializeField] private ARTrackedImageManager _arTrackedImageManager;
    [SerializeField] private GameObject _robotPrefab;
    private GameObject robot = null;
    private float theta = 0f;
    private List<ARPlane> _trackedPlanes = new List<ARPlane>();
    private List<Vector3> _trackedPoints = new List<Vector3>();

    
    private void OnEnable()
    {
        //Subscribe to the Track Image Changes
        _arTrackedImageManager.trackedImagesChanged += onTrackedImageChanged;
    }

    private void OnDisable()
    {
        //Unsubscribe to the Track Image Changes?
        _arTrackedImageManager.trackedImagesChanged -= onTrackedImageChanged;
    }

    private void onTrackedImageChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (var addedImage in args.added)
        {
            // Debug log to confirm the image was detected
            Debug.Log("Image detected: " + addedImage.referenceImage.name);

            if (_robotPrefab != null)
            {
                // Instantiate the prefab at the position of the detected image
                GameObject spawnedRobot = Instantiate(_robotPrefab, addedImage.transform.position, Quaternion.identity);

                // Parent the robot to the tracked image (this ensures it moves with the image)
                spawnedRobot.transform.SetParent(addedImage.transform);

                // Optionally, reset the local position and rotation to ensure it's centered on the image
                spawnedRobot.transform.localPosition = Vector3.zero;  // Keeps the robot at the center of the image
                spawnedRobot.transform.localRotation = Quaternion.identity;  // Align the robot’s rotation to the image's rotation
            }
        }
    }


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


        // Subscribe to the point cloud changed event
        _arPointCloudManager.pointCloudsChanged += OnPointCloudChanged;


    }
    private void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        // This function will be called whenever the detected planes change (added, updated, or removed)

        // Handle added planes
        foreach (var plane in args.added)
        {
            Debug.Log("Plane added: " + plane.trackableId);
            _trackedPlanes.Add(plane);  // Add the plane to the tracked list
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
            _trackedPlanes.Remove(plane);  // Remove the plane from the tracked list
        }

        // Update the plane count (you could display this in the UI as well)
        plane_count = _trackedPlanes.Count;

        // Update the UI with the new plane count
        _planeText.text = $"Planes: {plane_count}";
    }

    private void OnPointCloudChanged(ARPointCloudChangedEventArgs args)
    {
        // Handle added points
        foreach (var addedPointCloud in args.added)
        {
            Debug.Log("Point cloud added: " + addedPointCloud.trackableId);
            _trackedPoints.AddRange(addedPointCloud.positions);
        }

        // Handle updated points
        foreach (var updatedPointCloud in args.updated)
        {
            Debug.Log("Point cloud updated: " + updatedPointCloud.trackableId);
            // You may want to update positions here if needed
        }

        // Handle removed points
        foreach (var removedPointCloud in args.removed)
        {
            Debug.Log("Point cloud removed: " + removedPointCloud.trackableId);
            // You can choose to remove the points here if necessary (not always needed)
            // But we are keeping track of all points, so we'll just reset the list for simplicity
            _trackedPoints.Clear();
        }

        // Update the point count (based on the positions stored in _trackedPoints)
        point_count = _trackedPoints.Count;

        // Update the UI to show the point count
        text.text = $"Points: {point_count}";
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
        _planeText.text = $"Planes: {plane_count} Points: {point_count}";
        text.text = $"Points: {point_count}";

    }
    private void OnDestroy()
    {
        // Unsubscribe from events to avoid memory leaks
        ARSession.stateChanged -= OnARSessionStateChanged;

        _arPointCloudManager.pointCloudsChanged -= OnPointCloudChanged;
        _arPlaneManager.planesChanged -= OnPlanesChanged;
    }
}
