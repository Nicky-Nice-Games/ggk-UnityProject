using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class lineManager : MonoBehaviour
{
    private LineRenderer _lineRenderer;

    [SerializeField] private Transform[] _gizmoArms;

    // Start is called before the first frame update
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();

       

    }

    // Update is called once per frame
    void Update()
    {
        _lineRenderer.positionCount = _gizmoArms.Length;
        for (int i = 0; i < _gizmoArms.Length; i++)
        {
            _lineRenderer.SetPosition(i, _gizmoArms[i].position);
        }
    }
}
