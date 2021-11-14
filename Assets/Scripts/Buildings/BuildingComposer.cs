using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Meta type of building, generable by a <c>BuildingComposer</c></summary>
public enum MetaBuildingType
{
    Debug, BrutalTower, DarkLordHQ, EmpireBuilding, Hive
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
        Mesh toreturn = type switch
        {
            MetaBuildingType.BrutalTower => ComposeBrutalTower(sizeX, sizeZ),
            MetaBuildingType.DarkLordHQ => ComposeDarkLordHQ(sizeX, sizeZ),
            MetaBuildingType.EmpireBuilding => ComposeEmpireBuilding(sizeX, sizeZ),
            MetaBuildingType.Hive => ComposeHive(sizeX, sizeZ),
            _ => ComposeDebug(sizeX, sizeZ)
        };

        Vector2[] uvs = new Vector2[toreturn.vertices.Length];
        float scaler = 16f * UniversalFloorSize;
        for (var i = 0; i < toreturn.vertices.Length; i++)
        {
            float lx = toreturn.vertices[i].x / sizeX, lz = toreturn.vertices[i].z / sizeZ;
            uvs[i] = new Vector2((lx + lz) / 2, toreturn.vertices[i].y / scaler);
        }

        toreturn.uv = uvs;

        toreturn.RecalculateNormals();

        return toreturn;
    }

    private Mesh ComposeDebug(float sizeX, float sizeZ)
    {
        Mesh m1 = PrimitiveFactory.GetMesh(PrimitiveType.Cube);
        Mesh m2 = PrimitiveFactory.GetMesh(PrimitiveType.RoofDouble);

        var combine = GenCombineList();

        //Base
        CombineAdd(combine[0], m1, new Vector3(sizeX / 2, 0.05f, sizeZ / 2), new Vector3(sizeX, 0.1f, sizeZ));

        CombineAdd(combine[0], m1, new Vector3(sizeX / 2, 0.6f, sizeZ / 2), new Vector3(1, 1, 1));
        CombineAdd(combine[0], m2, new Vector3(sizeX / 2, 1.6f, sizeZ / 2), new Vector3(1.5f, 1, 1));

        return ComputeCombine(combine);
    }

    private Mesh ComposeBrutalTower(float sizeX, float sizeZ)
    {
        Mesh cube = PrimitiveFactory.GetMesh(PrimitiveType.Cube);

        var combine = GenCombineList(20);
        //Base
        CombineAdd(combine[0], cube, new Vector3(sizeX / 2, 0.05f, sizeZ / 2), new Vector3(sizeX, 0.1f, sizeZ));

        int notgenerated = rand.Next(5);

        if (notgenerated != 0)
        {
            float sizeLocalX = (float)rand.NextDouble() * (sizeX / 2.2f) + sizeX / 4f,
                sizeLocalZ = (float)rand.NextDouble() * (sizeZ / 2.5f) + sizeX / 4f;
            BuildDecorator.addBrutalSegment(combine[0], 0.3f, 0.3f, sizeLocalX, sizeLocalZ, rand.Next(5) + 1);
        }
        if (notgenerated != 1)
        {
            float sizeLocalX = (float)rand.NextDouble() * (sizeX / 2.2f) + sizeX / 4f,
                sizeLocalZ = (float)rand.NextDouble() * (sizeZ / 2.5f) + sizeX / 4f;
            BuildDecorator.addBrutalSegment(combine[0], sizeX - sizeLocalX - 0.15f, 0.1f, sizeLocalX, sizeLocalZ, rand.Next(3) + 1);
        }
        if (notgenerated != 2)
        {
            float sizeLocalX = (float)rand.NextDouble() * (sizeX / 2.2f) + sizeX / 4f,
                sizeLocalZ = (float)rand.NextDouble() * (sizeZ / 2.5f) + sizeX / 4f;
            BuildDecorator.addBrutalSegment(combine[0], 0.2f, sizeZ - sizeLocalZ - 0.15f, sizeLocalX, sizeLocalZ, rand.Next(3) + 2);
        }
        if (notgenerated != 3)
        {
            float sizeLocalX = (float)rand.NextDouble() * (sizeX / 2.2f) + sizeX / 4f,
                sizeLocalZ = (float)rand.NextDouble() * (sizeZ / 2.5f) + sizeX / 4f;
            BuildDecorator.addBrutalSegment(combine[0], sizeX - sizeLocalX - 0.17f, sizeZ - sizeLocalZ - 0.17f, sizeLocalX, sizeLocalZ, rand.Next(4) + 2);
        }

        return ComputeCombine(combine);
    }

    private Mesh ComposeDarkLordHQ(float sizeX, float sizeZ)
    {
        Mesh cyl = PrimitiveFactory.GetMesh(PrimitiveType.OctoPrism);
        Mesh cube = PrimitiveFactory.GetMesh(PrimitiveType.Cube);
        Mesh penta = PrimitiveFactory.GetMesh(PrimitiveType.PentaPrism);
        Mesh sphere = PrimitiveFactory.GetMesh(PrimitiveType.Sphere);

        float height = UniversalFloorSize * (rand.Next(3) + 10);
        var combine = GenCombineList();
        //Base
        CombineAdd(combine[0], cube, new Vector3(sizeX / 2, 0.05f, sizeZ / 2), new Vector3(sizeX, 0.1f, sizeZ));

        // Main cylindre
        CombineAdd(combine[0], cyl, new Vector3((sizeX / 3), 0.01f + height / 2, (sizeZ / 2)), new Vector3(sizeX / 3 * 2, height, sizeZ - 1f));
        CombineAdd(combine[0], cyl, new Vector3((sizeX / 3), 0.1f + UniversalFloorSize, (sizeZ / 2)), new Vector3(sizeX / 3 * 2 + 0.4f, 0.3f, sizeZ - 0.6f));
        // Entrance
        CombineAdd(combine[0], penta, new Vector3((sizeX / 3) * 2, 0.01f + UniversalFloorSize / 2, (sizeZ / 4)), new Vector3(sizeX / 3 * 2, UniversalFloorSize, sizeZ / 4));
        CombineAdd(combine[0], penta, new Vector3((sizeX / 3) * 2, 0.01f + UniversalFloorSize, (sizeZ / 4)), new Vector3(sizeX / 3 * 2 + 0.4f, 0.3f, sizeZ / 4 + 0.4f));

        int toptype = rand.Next(3);
        // Building Top
        if (toptype == 0 || toptype == 1)
            CombineAdd(combine[0], cyl, new Vector3((sizeX / 3), 0.01f + height, sizeZ / 3), new Vector3((sizeX / 3 * 2) * 0.4f, height / 2, (sizeZ - 1f) * 0.4f));
        if (toptype == 0 || toptype == 2)
            CombineAdd(combine[0], cyl, new Vector3((sizeX / 3), 0.01f + height, (sizeZ / 3) * 2), new Vector3((sizeX / 3 * 2) * 0.4f, height / 2, (sizeZ - 1f) * 0.6f));
        // Ball on top
        if (toptype == 0 && rand.Next(2) == 1)
            CombineAdd(combine[0], sphere, new Vector3((sizeX / 3), height * 1.25f, sizeZ / 2), new Vector3((sizeX / 3 * 2) * 0.3f, UniversalFloorSize / 2.5f, (sizeZ - 1f) * 0.5f));
        // Add antenas
        int antenas = rand.Next(3);
        if (antenas == 1)
        {
            CombineAdd(combine[0], cube, new Vector3(sizeX / 4 * 3 + 1, UniversalFloorSize / 2, sizeZ / 4 * 3 + 1), new Vector3(1.7f, UniversalFloorSize, 1.7f));
            BuildDecorator.AddAntenasCluster(combine[0], sizeX / 4 * 3, UniversalFloorSize, sizeZ / 4 * 3, 2, 2);
        }

        return ComputeCombine(combine);
    }

    private Mesh ComposeEmpireBuilding(float sizeX, float sizeZ)
    {
        Mesh cube = PrimitiveFactory.GetMesh(PrimitiveType.Cube);
        var combine = GenCombineList();

        //Base
        CombineAdd(combine[0], cube, new Vector3(sizeX / 2, 0.05f, sizeZ / 2), new Vector3(sizeX, 0.1f, sizeZ));

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

            CombineAdd(combine[0], cube, new Vector3(offsetLocaleX, localheight / 2, offsetLocaleZ),
                new Vector3(localSizeX, localheight, localSizeZ));

        }

        BuildDecorator.AddAntenasCluster(combine[0], highestX - 0.6f, highest, highestZ - 0.6f, 1.2f, 1.2f, 5);

        return ComputeCombine(combine);
    }

    private Mesh ComposeHive(float sizeX, float sizeZ)
    {
        Mesh cube = PrimitiveFactory.GetMesh(PrimitiveType.Cube);
        Mesh sphere = PrimitiveFactory.GetMesh(PrimitiveType.Sphere);
        var combine = GenCombineList();

        //Base
        CombineAdd(combine[0], cube, new Vector3(sizeX / 2, 0.05f, sizeZ / 2), new Vector3(sizeX, 0.1f, sizeZ));

        int divisionsX = 3 + rand.Next(2), divisionsZ = 3 + rand.Next(2), floors = rand.Next(5) + 4;
        float lenSideX = sizeX / divisionsX, lenSideZ = sizeZ / divisionsZ;
        for (int floor = 0; floor < floors; floor++)
        {
            CombineAdd(combine[0], cube, new Vector3(sizeX / 2, UniversalFloorSize + floor * UniversalFloorSize * 2, sizeZ / 2), new Vector3(sizeX - 1.5f, UniversalFloorSize * 2 - 1.2f, sizeZ - 1.5f));
            for (int x = 0; x < divisionsX; x++)
                for (int z = 0; z < divisionsZ; z++)
                {
                    if (rand.Next(8) != 5)
                        CombineAdd(combine[0], cube,
                            new Vector3(lenSideX / 2 + x * lenSideX,
                                UniversalFloorSize + floor * UniversalFloorSize * 2,
                                lenSideZ / 2 + z * lenSideZ),
                            new Vector3(lenSideX - 0.9f, UniversalFloorSize * 2 - 0.6f, lenSideZ - 0.9f));
                }
        }

        CombineAdd(combine[0], sphere, new Vector3(sizeX / 2, 0.05f, sizeZ / 2), new Vector3(sizeX / 2.6f, (floors - 1 + (float)rand.NextDouble() * 2) * UniversalFloorSize * 2, sizeZ / 2.6f));

        return ComputeCombine(combine);
    }


    private List<CombineInstance>[] GenCombineList(int size = 8)
    {
        return new[] { new List<CombineInstance>(size), new List<CombineInstance>(size) };
    }

    private static Mesh ComputeCombine(List<CombineInstance>[] combine)
    {
        List<CombineInstance> returncombine = new List<CombineInstance>(combine.Length);
        for (int i = 0; i < combine.Length; i++)
        {
            Mesh submesh = new Mesh();
            submesh.CombineMeshes(combine[i].ToArray(), true, true, false);
            returncombine.Add(new CombineInstance { mesh = submesh, subMeshIndex = i, transform = Matrix4x4.identity });
        }
        Mesh toreturn = new Mesh();
        toreturn.CombineMeshes(returncombine.ToArray(), false, true, false);
        return toreturn;
    }

    private static void CombineAdd(List<CombineInstance> combine, Mesh geometry, Vector3 pos, Vector3 scale)
    {
        CombineAdd(combine, geometry, Matrix4x4.TRS(pos, Quaternion.identity, scale));
    }
    private static void CombineAdd(List<CombineInstance> combine, Mesh geometry, Matrix4x4 transform)
    {
        combine.Add(new CombineInstance
        {
            mesh = geometry,
            subMeshIndex = 0,
            transform = transform
        });
    }



}
