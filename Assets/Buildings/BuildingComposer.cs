using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Meta type of building, generable by a <c>BuildingComposer</c></summary>
public enum MetaBuildingType
{
    Debug, BrutalTower
}

/// <summary>A<c>BuildingComposer</c> is an object that creates buildings for a set area. 
/// The design intent is to have a single composer per tile in the generator, and have the
/// composer generate buildings using its inner seed, which is different for each tile.</summary>
public class BuildingComposer
{

    private System.Random rand;
    public static readonly float UniversalFloorSize = 2.4f;

    public BuildingComposer(int seed)
    {
        rand = new System.Random(seed);
    }

    public BuildingComposer()
    {
        rand = new System.Random();
    }

    /// <summary>Composes a new Mesh for a given type. Multiple calls of this function may result in different outputs,
    /// as calls iterate the inner random generator. THe returned mesh's vertices are all between 0 and size parameters.</summary>
    public Mesh ComposeNew(MetaBuildingType type, float sizeX, float sizeZ)
    {
        switch (type)
        {
            case MetaBuildingType.BrutalTower: return ComposeBrutalTower(sizeX, sizeZ);
            default: return ComposeDebug(sizeX, sizeZ);
        }
    }

    private Mesh ComposeDebug(float sizeX, float sizeZ)
    {
        Mesh bse = PrimitiveFactory.GetMesh(PrimitiveType.Cube);
        Mesh m1 = PrimitiveFactory.GetMesh(PrimitiveType.Cube);
        Mesh m2 = PrimitiveFactory.GetMesh(PrimitiveType.RoofDouble);

        CombineInstance[] combine = new CombineInstance[3];

        combine[0] = new CombineInstance
        {
            mesh = m1,
            subMeshIndex = 0,
            transform = Matrix4x4.TRS(new Vector3(sizeX / 2, 0.05f, sizeZ / 2), Quaternion.identity, new Vector3(sizeX, 0.1f, sizeZ))
        };
        combine[1] = new CombineInstance
        {
            mesh = m1,
            subMeshIndex = 0,
            transform = Matrix4x4.TRS(new Vector3(sizeX / 2, 0.6f, sizeZ / 2), Quaternion.identity, new Vector3(1, 1, 1))
        };
        combine[2] = new CombineInstance
        {
            mesh = m2,
            subMeshIndex = 0,
            transform = Matrix4x4.TRS(new Vector3(sizeX / 2, 1.6f, sizeZ / 2), Quaternion.identity, new Vector3(1.5f, 1, 1))
        };

        Mesh toreturn = new Mesh();
        toreturn.CombineMeshes(combine, true, true, false);
        return toreturn;
    }

    private Mesh ComposeBrutalTower(float sizeX, float sizeZ)
    {
        Mesh cube = PrimitiveFactory.GetMesh(PrimitiveType.Cube);

        List<CombineInstance> combine = new List<CombineInstance>(20);
        combine.Add(new CombineInstance // Base
        {
            mesh = cube,
            subMeshIndex = 0,
            transform = Matrix4x4.TRS(new Vector3(sizeX / 2, 0.05f, sizeZ / 2), Quaternion.identity, new Vector3(sizeX, 0.1f, sizeZ))
        });

        int notgenerated = rand.Next(5);

        if (notgenerated != 0)
        {
            float sizeLocalX = (float)rand.NextDouble() * (sizeX / 2.2f) + sizeX / 4f, 
                sizeLocalZ = (float)rand.NextDouble() * (sizeZ / 2.5f) + sizeX / 4f;
            addBrutalSegment(combine, 0.3f, 0.3f, sizeLocalX, sizeLocalZ, rand.Next(5) + 1);
        }
        if (notgenerated != 1)
        {
            float sizeLocalX = (float)rand.NextDouble() * (sizeX / 2.2f) + sizeX / 4f,
                sizeLocalZ = (float)rand.NextDouble() * (sizeZ / 2.5f) + sizeX / 4f;
            addBrutalSegment(combine, sizeX - sizeLocalX - 0.15f, 0.1f, sizeLocalX, sizeLocalZ, rand.Next(3) + 1);
        }
        if (notgenerated != 2)
        {
            float sizeLocalX = (float)rand.NextDouble() * (sizeX / 2.2f) + sizeX / 4f,
                sizeLocalZ = (float)rand.NextDouble() * (sizeZ / 2.5f) + sizeX / 4f;
            addBrutalSegment(combine, 0.2f, sizeZ - sizeLocalZ - 0.15f, sizeLocalX, sizeLocalZ, rand.Next(3) + 2);
        }
        if (notgenerated != 3)
        {
            float sizeLocalX = (float)rand.NextDouble() * (sizeX / 2.2f) + sizeX / 4f,
                sizeLocalZ = (float)rand.NextDouble() * (sizeZ / 2.5f) + sizeX / 4f;
            addBrutalSegment(combine, sizeX - sizeLocalX - 0.17f, sizeZ - sizeLocalZ - 0.17f, sizeLocalX, sizeLocalZ, rand.Next(4) + 2);
        }

        Mesh toreturn = new Mesh();
        toreturn.CombineMeshes(combine.ToArray(), true, true, false);
        return toreturn;
    }

    private void addBrutalSegment(List<CombineInstance> combine, float x, float z, float sizeX, float sizeZ, int floors)
    {
        Mesh cube = PrimitiveFactory.GetMesh(PrimitiveType.Cube);
        Mesh pyra = PrimitiveFactory.GetMesh(PrimitiveType.Pyramid4);
        float height = UniversalFloorSize * floors;
        combine.Add(new CombineInstance
        {
            mesh = cube,
            subMeshIndex = 0,
            transform = Matrix4x4.TRS(new Vector3(x + (sizeX / 2), 0.01f + height / 2, z + (sizeZ / 2)), Quaternion.identity, new Vector3(sizeX, height, sizeZ))
        });
        for (int f = 1; f <= floors; f++)
        {
            combine.Add(new CombineInstance
            {
                mesh = cube,
                subMeshIndex = 0,
                transform = Matrix4x4.TRS(new Vector3(x + (sizeX / 2), 0.01f + (f * height / floors), z + (sizeZ / 2)), Quaternion.identity, new Vector3(sizeX + 0.2f, 0.1f, sizeZ + 0.2f))
            });
        }
        combine.Add(new CombineInstance
        {
            mesh = pyra,
            subMeshIndex = 0,
            transform = Matrix4x4.TRS(new Vector3(x + (sizeX / 2), 0.51f + height, z + (sizeZ / 2)), Quaternion.identity, new Vector3(sizeX, 1, sizeZ))
        });

    }

}
