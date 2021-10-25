using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Type of primitive, generable by a <c>PrimitiveFactory</c></summary>
public enum PrimitiveType {
    Sphere, Isocaedron, Cube, Pyramid4, Cylindre
}

/// <summary>Factory for all Primitive mesh types, that may be used for composition.</summary>
public class PrimitiveFactory {

    /// <summary>Internal buffer for all primitive geometry</summary>
    private static Dictionary<PrimitiveType, Mesh> geometryBuffer = new Dictionary<PrimitiveType, Mesh>();

    /// <summary>Generates all meshes of this factory in an intern buffer. This forces regeneration of all meshes, including already cached ones.</summary>
    public static void GenerateAll() {
        foreach (PrimitiveType type in PrimitiveType.GetValues(typeof(PrimitiveType))) {
            geometryBuffer.Add(type, GenerateMeshFor(type));
        }
    }

    /// <summary>Returns a given mesh from the internal mesh buffer. Will generate the mesh if it isn't currently cached. Otherwise, simply returns a pointer to the cache.</summary>
    public static Mesh GetMesh(PrimitiveType type) {
        if (geometryBuffer.TryGetValue(type, out Mesh toreturn))
            return toreturn;
        toreturn = GenerateMeshFor(type);
        geometryBuffer.Add(type, toreturn);
        return toreturn;
    }

    /// <summary>Invalidates the cache of a given mesh type. Returns true if the given type had a cache that was invalidated. 
    /// Current pointers to the invalidated object mesh will not be destroyed, and keep their current data.
    /// Invalidating a mesh will force regeneration the next time this type is fetched.</summary>
    public static bool InvalidateMesh(PrimitiveType type) {
        return geometryBuffer.Remove(type);
    }

    private static Mesh GenerateMeshFor(PrimitiveType type) {
        // TODO: generate a mesh
        Mesh toreturn = null;
        switch (type) {
            case PrimitiveType.Sphere:
                toreturn = SphereBuilder.make1t1Sphere(3);
                break;
            case PrimitiveType.Isocaedron:
                toreturn = SphereBuilder.make1t1Sphere(0);
                break;
            case PrimitiveType.Cube:
                toreturn = CubeBuilder.buildCube1t1();
                break;
            case PrimitiveType.Pyramid4:
                toreturn = PyramidBuilder.buildPyramidt1();
                break;
            case PrimitiveType.Cylindre:
                toreturn = CylindreBuilder.CreateCylinder(0.5f, 0.5f, 1, 12, 1, false);
                break;
        }
        geometryBuffer.Add(type, toreturn);
        return toreturn;
    }

}
