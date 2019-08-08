// =========================
// MAKAKA GAMES - MAKAKA.ORG
// =========================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PublisherComponentExampleSimplest : MonoBehaviour 
{

}

#if UNITY_EDITOR
[CustomEditor(typeof(PublisherComponentExampleSimplest)), CanEditMultipleObjects]
public class PublisherComponent_Example_Simplest_Editor : PublisherComponent_Editor
{	

}
#endif