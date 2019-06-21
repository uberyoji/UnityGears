using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UrlParser : MonoBehaviour
{  
    public GearCreator GC;

    // Start is called before the first frame update
    void Start()
    {
        GC.TeethCount = URLParameters.GetSearchParameters().GetInt("teethcount", GC.TeethCount);
        GC.InnerRadius = (float)URLParameters.GetSearchParameters().GetDouble("innerradius", GC.InnerRadius);
        GC.OuterRadius = (float)URLParameters.GetSearchParameters().GetDouble("outerradius", GC.OuterRadius);
        GC.OuterToothDepth = (float)URLParameters.GetSearchParameters().GetDouble("outertoothdepth", GC.OuterToothDepth);
        GC.InnerToothDepth = (float)URLParameters.GetSearchParameters().GetDouble("innertoothdepth", GC.InnerToothDepth);
        GC.Width = (float)URLParameters.GetSearchParameters().GetDouble("width", GC.Width);

        GC.ForceUpdateMesh = true;        
    }
}
