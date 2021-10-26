using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Meta type of building, generable by a <c>BuildingComposer</c></summary>
public enum MetaBuildingType{
    Debug
}

public class BuildingComposer {

    private Random rand;

    public BuildingComposer(int32 seed){
        rand = new Random(seed);
    }

    public BuildingComposer(){
        rand = new Random();
    }

    /// <summary>Composes a new Mesh for a given type. Multiple calls of this function may result in different outputs,</summary>
    public Mesh ComposeNew(MetaBuildingType type){
        Mesh cube = PrimitiveFactory.GetMesh(PrimitiveType.Cube);
        Mesh roof = PrimitiveFactory.GetMesh(PrimitiveType.RoofDouble);
        
        return cube;
    }

}
