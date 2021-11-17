# VirtualWorldsTP

VirtualWorldsTP is a student project about procedural city generation.
This prototype creates a random set of buidlings, and enables simple exploration of an everchanging city landfill between them.

This aims at creating a megastructure like explorative feeling, and serves as tech demo for urban generation.

## Overview

Wip: screenshots

## Installing

<details><summary>Run release</summary>
<p>
 
 - <a href="https://github.com/Ikeiwa/VirtualWorldsTP/releases/latest">Download latest version</a>

 - Run the release executable file. 

 </p>
</details>
<details><summary>Build from source</summary>
 
<p>

 - Clone the repository using git.
```bash
git clone https://github.com/Ikeiwa/VirtualWorldsTP
```
 - Open the root folder as a unity project:
 In unity hub:
 ```
 Installs > Install Editor > 2020.3.18f1
 Projects > Open > <Repository root path>
 ```
 
 - Either run in unity editor,
 or follow <a href="https://docs.unity3d.com/Manual/PublishingBuilds.html">Unity build procedure</a>.
 

</p>
</details>

## Implementation details

<details><summary>Building generation</summary>
<p>
<h3>Building definition</h3>
 
A building is defined by the following elements:
  - a <b>mesh</b>
  - a <b>2D position</b> on the map (x,z)
  - a <b>rectangular footprint</b> (sizeX,sizeZ)
  - a <b>procedural material structure</b> to customize the interior mapping shader for this specific building 
  - a <b>basetype</b> to generate the mesh
 
 Generating a building is as simple as:
 ```csharp
 // Initialize a new composer with a random seed
 BuildingComposer composer = new BuildingComposer(seed);
 // Compose a new building mesh of a given type, of the given size (in float, 1f = 1 meter).
 Mesh m = composer.ComposeNew(MetaBuildingType.Hive, 8, 8);
 // Instanciate the building prefab, containing the right materials already
 GameObject building = Instantiate(buildingPrefab, buildingRoot);
 // Add the mesh to the building instance
 building.GetComponent<MeshFilter>().mesh = buildingMesh;
 building.GetComponent<MeshCollider>().sharedMesh = buildingMesh;
 ```
 
 Note that this code does not procedurally generate random data for the interior mapping shader.<br/>
 To do so, you may use:
 ```
  Material procMat = new Material(InteriorMapping);
  procMat.SetTextureScale(WindowsAlbedo,new Vector2(8,16));
        
 // TODO : change shader data here
        
 building.GetComponent<MeshRenderer>().material = procMat;
 ```
 
 Building types are limited to a set list of basetypes:
 ```csharp
 /// <summary>Meta type of building, generable by a <c>BuildingComposer</c></summary>
public enum MetaBuildingType {
    Debug, BrutalTower, DarkLordHQ, EmpireBuilding, Hive
}
```
Each basetype will generate differently according to the state of the composer's internal random number generator, providing diversity.<br/>
 If need be, it is possible to implement new basetypes:<br/>
 Start by adding an enum value of your type, with a compile time value.
  ```csharp
public enum MetaBuildingType {
    [...], mytypename
}
```
 In BuildingComposer.cs, add a function that takes a size and parametters you may need, and returns a mesh of your building contained within 0 and sizeN.
The minimal code to return a base with a cube on top is the following:
 ```csharp
 private Mesh ComposeMybuilding(float sizeX, float sizeZ) {
  var combine = GenCombineList();
  //Base
  CombineAdd(combine[1], PrimitiveFactory.GetMesh(PrimitiveType.Cube), new Vector3(sizeX / 2, 0.05f, sizeZ / 2), new Vector3(sizeX, 0.1f, sizeZ));
  // Cuve in the middle
  CombineAdd(combine[0], PrimitiveFactory.GetMesh(PrimitiveType.Cube), new Vector3(sizeX / 2, 0.6f, sizeZ / 2), new Vector3(1, 1, 1));
  return ComputeCombine(combine);
 }
 ```
 Then, simply add a mapping from your enum value to your function in the ComposeNew function:
 ```csharp
 Mesh toreturn = type switch {
  MetaBuildingType.BrutalTower => ComposeBrutalTower(sizeX, sizeZ)
  [...]
 ```
</p>
</details>
<details><summary>Tiled map generation</summary>
<p>
WIP
</p>
</details>
