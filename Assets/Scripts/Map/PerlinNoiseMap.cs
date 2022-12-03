using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Mirror;

public class PerlinNoiseMap : NetworkBehaviour {
	
	Dictionary<int, List<GameObject>> tileset;
	Dictionary<int, GameObject> tile_groups;
	[Space(3)]
	[Header("Tiles Set")]
	[Space(1)]
	public List<GameObject> prefab_Rock = new List<GameObject>();
	public List<GameObject> prefab_Dirt_Rock = new List<GameObject>();
	public List<GameObject> prefab_Dirt = new List<GameObject>();
	public List<GameObject> prefab_DirtyGrass = new List<GameObject>();
	public List<GameObject> prefab_Grass = new List<GameObject>();
	public List<GameObject> prefab_Tree = new List<GameObject>();
	public List<GameObject> prefab_Wall = new List<GameObject>();
	public GameObject prefab_default;

	int map_width = 100;
	int map_height = 100;

	SyncList<List<int>> noise_grid = new SyncList<List<int>>();
	List<List<GameObject>> tile_grid = new List<List<GameObject>>();

	// recommend 4 to 20
	float magnification = 7.0f;

	int x_offset = 0; // <- +>
	int y_offset = 0; // v- +^
	

    void CreateTileset() {
	    // Liste de Game object pour avoir un truc randoms dan le tileset
	    tileset = new Dictionary<int, List<GameObject>>();
	    tileset.Add(0, prefab_Rock);
	    tileset.Add(1, prefab_Dirt_Rock);
    	tileset.Add(2, prefab_Dirt);
    	tileset.Add(3, prefab_DirtyGrass);
        tileset.Add(4, prefab_Grass);
        tileset.Add(5, prefab_Grass);
        tileset.Add(6, prefab_Tree);
        tileset.Add(7, prefab_Tree);
    }

    void CreateTileGroups() {
    	/** Create empty gameobjects for grouping tiles of the same type, ie
    		forest tiles **/

    	tile_groups = new Dictionary<int, GameObject>();
    	foreach(KeyValuePair<int, List<GameObject>> prefab_pair in tileset) {
    		GameObject tile_group = new GameObject("TileType");
    		tile_group.transform.parent = gameObject.transform;
    		tile_group.transform.localPosition = new Vector3(0, 0, 0);
    		tile_groups.Add(prefab_pair.Key, tile_group);
    	}
        GameObject default_tile_group = new GameObject("NoSprite");
        default_tile_group.transform.parent = gameObject.transform;
        default_tile_group.transform.localPosition = new Vector3(0, 0, 0);
        tile_groups.Add(666, default_tile_group);
        GameObject Wall_tile_group = new GameObject("WALL");
        Wall_tile_group.transform.parent = gameObject.transform;
        Wall_tile_group.transform.localPosition = new Vector3(0, 0, 0);
        tile_groups.Add(999, Wall_tile_group);
    }

    void GenerateMap() {
    	/** Generate a 2D grid using the Perlin noise fuction, storing it as
    		both raw ID values and tile gameobjects **/

    	for(int x = 0; x < map_width; x++) {
    		noise_grid.Add(new List<int>());
    		for(int y = 0; y < map_height; y++) {
    			int tile_id = GetIdUsingPerlin(x, y);
    			noise_grid[x].Add(tile_id);
    		}
    	}
    }

    void RenderMap() {
	    for(int x = 0; x < map_width; x++) {
		    tile_grid.Add(new List<GameObject>());
		    for(int y = 0; y < map_height; y++) {
			    CreateTile(noise_grid[x][y], x, y);
		    }
	    }
    }

    int GetIdUsingPerlin(int x, int y) {
    	/** Using a grid coordinate input, generate a Perlin noise value to be
    		converted into a tile ID code. Rescale the normalised Perlin value
    		to the number of tiles available. **/

    	float raw_perlin = Mathf.PerlinNoise(
    		(x - x_offset) / magnification,
    		(y - y_offset) / magnification
    	);
    	float clamp_perlin = Mathf.Clamp01(raw_perlin);
    	float scaled_perlin = clamp_perlin * tileset.Count;

		// Replaced 4 with tileset.Count to make adding tiles easier
    	if(scaled_perlin == tileset.Count) {
    		scaled_perlin = (tileset.Count - 1);
    	}
    	return Mathf.FloorToInt(scaled_perlin);
    }

    void CreateTile(int tile_id, int x, int y) {
    	/** Creates a new tile using the type id code, group it with common
    		tiles, set it's position and store the gameobject. **/
        
        GameObject tile_prefab = prefab_default;
        GameObject tile_group = tile_groups[666];
        
        int numberTile = 0;
        if (tileset[tile_id].Any() && !(x==0 || x== map_width-1 || y == 0 || y == map_height-1)) {
	        int size = tileset[tile_id].Count;
	        if (size > 1) {
		        numberTile = Random.Range(0, size);
	        }
	        tile_prefab = tileset[tile_id][numberTile];
	        tile_group = tile_groups[tile_id];
        }
        else if ((x==0 || x== map_width-1 || y == 0 || y == map_height-1)) {
	        int size = prefab_Wall.Count;
	        if (size > 1) {
		        numberTile = Random.Range(0, size);
	        }
	        if (prefab_Wall.Any()) {
		        tile_prefab = prefab_Wall[numberTile];
	        }
	        tile_group = tile_groups[999];
        }
        
        GameObject tile = Instantiate(tile_prefab, tile_group.transform);
        tile.name = string.Format("tile_x{0}_y{1}", x, y);
        tile.transform.localPosition = new Vector3(x, y, 0);
        tile_grid[x].Add(tile);
        
    }

    public SyncList<List<int>> getNoiseGrid() {
	    return noise_grid;
    }

    public override void OnStartServer() {
	    CreateTileset();
	    CreateTileGroups();
	    GenerateMap();
    }

    public override void OnStartClient() {
	    CreateTileset();
	    CreateTileGroups();
	    RenderMap();
    }
}
