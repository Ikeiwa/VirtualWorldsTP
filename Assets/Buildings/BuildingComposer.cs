using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Meta type of building, generable by a <c>BuildingComposer</c></summary>
public enum MetaBuildingType
{
    Debug, BrutalTower, DarkLordHQ, EmpireBuilding
}

/// <summary>A<c>BuildingComposer</c> is an object that creates buildings for a set area. 
/// The design intent is to have a single composer per tile in the generator, and have the
/// composer generate buildings using its inner seed, which is different for each tile.</summary>
public class BuildingComposer
{

    private System.Random rand;
    public static readonly float UniversalFloorSize = 2.8f;

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
            case MetaBuildingType.DarkLordHQ: return ComposeDarkLordHQ(sizeX, sizeZ);
            case MetaBuildingType.EmpireBuilding: return ComposeEmpireBuilding(sizeX, sizeZ);
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
            BuildDecorator.addBrutalSegment(combine, 0.3f, 0.3f, sizeLocalX, sizeLocalZ, rand.Next(5) + 1);
        }
        if (notgenerated != 1)
        {
            float sizeLocalX = (float)rand.NextDouble() * (sizeX / 2.2f) + sizeX / 4f,
                sizeLocalZ = (float)rand.NextDouble() * (sizeZ / 2.5f) + sizeX / 4f;
            BuildDecorator.addBrutalSegment(combine, sizeX - sizeLocalX - 0.15f, 0.1f, sizeLocalX, sizeLocalZ, rand.Next(3) + 1);
        }
        if (notgenerated != 2)
        {
            float sizeLocalX = (float)rand.NextDouble() * (sizeX / 2.2f) + sizeX / 4f,
                sizeLocalZ = (float)rand.NextDouble() * (sizeZ / 2.5f) + sizeX / 4f;
            BuildDecorator.addBrutalSegment(combine, 0.2f, sizeZ - sizeLocalZ - 0.15f, sizeLocalX, sizeLocalZ, rand.Next(3) + 2);
        }
        if (notgenerated != 3)
        {
            float sizeLocalX = (float)rand.NextDouble() * (sizeX / 2.2f) + sizeX / 4f,
                sizeLocalZ = (float)rand.NextDouble() * (sizeZ / 2.5f) + sizeX / 4f;
            BuildDecorator.addBrutalSegment(combine, sizeX - sizeLocalX - 0.17f, sizeZ - sizeLocalZ - 0.17f, sizeLocalX, sizeLocalZ, rand.Next(4) + 2);
        }

        Mesh toreturn = new Mesh();
        toreturn.CombineMeshes(combine.ToArray(), true, true, false);
        return toreturn;
    }

    private Mesh ComposeDarkLordHQ(float sizeX, float sizeZ)
    {
        Mesh cyl = PrimitiveFactory.GetMesh(PrimitiveType.OctoPrism);
        Mesh cube = PrimitiveFactory.GetMesh(PrimitiveType.Cube);
        Mesh penta = PrimitiveFactory.GetMesh(PrimitiveType.PentaPrism);
        Mesh sphere = PrimitiveFactory.GetMesh(PrimitiveType.Sphere);

        float height = UniversalFloorSize * (rand.Next(3) + 10);
        List<CombineInstance> combine = new List<CombineInstance>(20);
        combine.Add(new CombineInstance // Base
        {
            mesh = cube,
            subMeshIndex = 0,
            transform = Matrix4x4.TRS(new Vector3(sizeX / 2, 0.05f, sizeZ / 2), Quaternion.identity, new Vector3(sizeX, 0.1f, sizeZ))
        });

        // Main cylindre
        combine.Add(new CombineInstance
        {
            mesh = cyl,
            subMeshIndex = 0,
            transform = Matrix4x4.TRS(new Vector3((sizeX / 3), 0.01f + height / 2, (sizeZ / 2)), Quaternion.identity, new Vector3(sizeX / 3 * 2, height, sizeZ - 1f))
        });
        combine.Add(new CombineInstance
        {
            mesh = cyl,
            subMeshIndex = 0,
            transform = Matrix4x4.TRS(new Vector3((sizeX / 3), 0.1f + UniversalFloorSize, (sizeZ / 2)), Quaternion.identity, new Vector3(sizeX / 3 * 2 + 0.4f, 0.3f, sizeZ - 0.6f))
        });
        // Entrance
        combine.Add(new CombineInstance
        {
            mesh = penta,
            subMeshIndex = 0,
            transform = Matrix4x4.TRS(new Vector3((sizeX / 3) * 2, 0.01f + UniversalFloorSize / 2, (sizeZ / 4)), Quaternion.identity, new Vector3(sizeX / 3 * 2, UniversalFloorSize, sizeZ / 4))
        });
        combine.Add(new CombineInstance
        {
            mesh = penta,
            subMeshIndex = 0,
            transform = Matrix4x4.TRS(new Vector3((sizeX / 3) * 2, 0.01f + UniversalFloorSize, (sizeZ / 4)), Quaternion.identity, new Vector3(sizeX / 3 * 2 + 0.4f, 0.3f, sizeZ / 4 + 0.4f))
        });

        int toptype = rand.Next(3);
        // Building Top
        if (toptype == 0 || toptype == 1)
            combine.Add(new CombineInstance
            {
                mesh = cyl,
                subMeshIndex = 0,
                transform = Matrix4x4.TRS(new Vector3((sizeX / 3), 0.01f + height, sizeZ / 3), Quaternion.identity, new Vector3((sizeX / 3 * 2) * 0.4f, height / 2, (sizeZ - 1f) * 0.4f))
            });
        if (toptype == 0 || toptype == 2)
            combine.Add(new CombineInstance
            {
                mesh = cyl,
                subMeshIndex = 0,
                transform = Matrix4x4.TRS(new Vector3((sizeX / 3), 0.01f + height, (sizeZ / 3) * 2), Quaternion.identity, new Vector3((sizeX / 3 * 2) * 0.4f, height / 2, (sizeZ - 1f) * 0.6f))
            });
        // Ball on top
        if (toptype == 0)
            combine.Add(new CombineInstance
            {
                mesh = sphere,
                subMeshIndex = 0,
                transform = Matrix4x4.TRS(new Vector3((sizeX / 3), height * 1.25f, sizeZ / 2), Quaternion.identity, new Vector3((sizeX / 3 * 2) * 0.3f, UniversalFloorSize / 2.5f, (sizeZ - 1f) * 0.5f))
            });
        // Add antenas
        int antenas = rand.Next(3);
        if (antenas == 1)
        {
            combine.Add(new CombineInstance
            {
                mesh = cube,
                subMeshIndex = 0,
                transform = Matrix4x4.TRS(new Vector3(sizeX / 4 * 3 + 1, UniversalFloorSize / 2, sizeZ / 4 * 3 + 1), Quaternion.identity, new Vector3(1.7f, UniversalFloorSize, 1.7f))
            });
            BuildDecorator.AddAntenasCluster(combine, sizeX / 4 * 3, UniversalFloorSize, sizeZ / 4 * 3, 2, 2);
        }

        Mesh toreturn = new Mesh();
        toreturn.CombineMeshes(combine.ToArray(), true, true, false);
        return toreturn;
    }

    private Mesh ComposeEmpireBuilding(float sizeX, float sizeZ)
    {
        Mesh cube = PrimitiveFactory.GetMesh(PrimitiveType.Cube);
        List<CombineInstance> combine = new List<CombineInstance>(20);
        combine.Add(new CombineInstance // Base
        {
            mesh = cube,
            subMeshIndex = 0,
            transform = Matrix4x4.TRS(new Vector3(sizeX / 2, 0.05f, sizeZ / 2), Quaternion.identity, new Vector3(sizeX, 0.1f, sizeZ))
        });

        int buildamount = rand.Next(5) + 4;
        float highest = 0, highestX = 0, highestZ = 0;
        for (int i = 0; i < buildamount; ++i)
        {
            float localheight = (rand.Next(10) + 3) * UniversalFloorSize + (float)rand.NextDouble();
            float localSizeX = (float)rand.NextDouble() * (sizeX * 0.8f) + 1, localSizeZ = (float)rand.NextDouble() * (sizeZ * 0.8f) + 1;
            float offsetLocaleX = (float)rand.NextDouble() * (sizeX - localSizeX) + localSizeX / 2, offsetLocaleZ = (float)rand.NextDouble() * (sizeZ - localSizeZ) + localSizeZ / 2;
            if (localheight > highest)
            {
                highest = localheight;
                highestX = offsetLocaleX;
                highestZ = offsetLocaleZ;
            }
            combine.Add(new CombineInstance // Base
            {
                mesh = cube,
                subMeshIndex = 0,
                transform = Matrix4x4.TRS(new Vector3(offsetLocaleX, localheight / 2, offsetLocaleZ), Quaternion.identity, new Vector3(localSizeX, localheight, localSizeZ))
            });
        }

        BuildDecorator.AddAntenasCluster(combine, highestX - 0.6f, highest, highestZ - 0.6f, 1.2f, 1.2f, 5);

        Mesh toreturn = new Mesh();
        toreturn.CombineMeshes(combine.ToArray(), true, true, false);
        return toreturn;
    }

}
