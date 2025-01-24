using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARunitLab : MonoBehaviour


{
    public TMP_Text text;
    [SerializeField] private TMP_Text _stateText;
    [SerializeField] private ARPlaneManager _arPlaneManager;
    [SerializeField] private ARPointCloudManager _arPointCloudManager;
    [SerializeField] private TMP_Text _planeText;



    private void OnARSessionStateChanged(ARSessionStateChangedEventArgs args)
    {
        // Do something with args ...
        // e.g. _stateText . text = args .[ XXX]
        // Hint : you may want to look at what

    }

    private void Start()
    {
        ARSession.stateChanged += OnARSessionStateChanged;

    }
    private void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        args.ToString();
    }

    private void Update()
    {
        _planeText.ToString();
    }
}
