using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public static class SelectionConstants
{
    private const string infinity = "infinity";
    private const string cylinder = "cylinder";
    private const string pyramid = "pyramid";
    private const string sphere = "sphere";
    private const string cube = "cube";
    private const string star = "star";

    public static readonly IList<string> objTypeNames = new ReadOnlyCollection<string>
        (new List<string> { infinity, cylinder, pyramid, sphere, cube, star });
}