using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolNode : MonoBehaviour
{
    public PatrolNode[] _adjacentNodes;
    public bool _isStoppingNode = true;
    public bool _useY = false;
}
