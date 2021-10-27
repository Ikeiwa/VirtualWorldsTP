using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Meta type of building, generable by a <c>BuildingComposer</c></summary>
public enum MetaBuildingType{
    Debug
}

/// <summary>A<c>BuildingComposer</c> is an object that creates buildings for a set area. 
/// The design intent is to have a single composer per tile in the generator, and have the
/// composer generate buildings using its inner seed, which is different for each tile.</summary>
public class BuildingComposer {

    private System.Random rand;

    public BuildingComposer(int seed){
        rand = new System.Random(seed);
    }

    public BuildingComposer(){
        rand = new System.Random();
    }

    /// <summary>Composes a new Mesh for a given type. Multiple calls of this function may result in different outputs,
    /// as calls iterate the inner random generator. THe returned mesh's vertices are all between 0 and size parameters.</summary>
    public Mesh ComposeNew(MetaBuildingType type, float sizeX, float sizeZ){
        switch (type) {
            default: return ComposeDebug(sizeX, sizeZ);
        }
    }

    private Mesh ComposeDebug(float sizeX, float sizeZ) {
        Mesh bse = PrimitiveFactory.GetMesh(PrimitiveType.Cube);
        Mesh m1 = PrimitiveFactory.GetMesh(PrimitiveType.Cube);
        Mesh m2 = PrimitiveFactory.GetMesh(PrimitiveType.RoofDouble);

        CombineInstance[] combine = new CombineInstance[3];

        combine[0] = new CombineInstance
        {
            mesh = m1,
            subMeshIndex = 0,
            transform = Matrix4x4.TRS(new Vector3(sizeX/2, 0, sizeZ/2), Quaternion.identity, new Vector3(sizeX, 0.1f, sizeZ))
        };
        combine[1] = new CombineInstance
        {
            mesh = m1,
            subMeshIndex = 0,
            transform = Matrix4x4.TRS(new Vector3(sizeX / 2, 0.55f, sizeZ / 2), Quaternion.identity, new Vector3(1, 1, 1))
        };
        combine[2] = new CombineInstance
        {
            mesh = m2,
            subMeshIndex = 0,
            transform = Matrix4x4.TRS(new Vector3(sizeX / 2, 1.55f, sizeZ / 2), Quaternion.identity, new Vector3(1.5f, 1, 1))
        };

        Mesh toreturn = new Mesh();
        toreturn.CombineMeshes(combine, true, true, false);
        return toreturn;
    }

}
