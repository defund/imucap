CubeTerrain Documentation
Version 1.0

# Quick Start

1. Open Demo scene.
2. Select Cube Terrain gameObject.
3. Change the heightmap to a different texture.
4. Click "Generate Terrain."

# Cube Terrain Object Settings

## Heightmap
The heightmap to generate vertice data from.  Textures must be square.  
Images may be grayscale or not, but this utility will not take color 
data into account.

## Flatland Material / Slope Material
The materials to apply to flat polygons and sloped polygons.

## Block Size
The size of the block that makes up the terrain.

As an example, a 16x16 pixel terrain with a Map Size of 16 and block 
size of 2 will consist of 4 tiles (with 5 connecting tiles).

## Map Size
The size of the terrain.

## Slope Angle
The minimum polygon normal angle for the triangle to be considered a
sloping edge.

## Map Height
The max height for this mesh.  Heights are calculated by normalizing
the alpha of each pixel, then multiplying by this value.

# Bonus!

## Create Text Asset
This editor script adds Text Asset to the list of assets available
in the Assets/Create/ menu.  Quite handy if you ask me!