using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Mirror;
using Random = UnityEngine.Random;

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
	float magnification = 10.0f;

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
        magnification = Random.Range(8, 15);
        x_offset = Random.Range(-999999, 999999);
        y_offset = Random.Range(-999999, 999999);
        for(int x = 0; x < map_width; x++) {
    		noise_grid.Add(new List<int>());
    		for(int y = 0; y < map_height; y++) {
    			int tile_id = GetIdUsingPerlin(x, y);
    			noise_grid[x].Add(tile_id);
    		}
    	}
    }
    
    // Todo Post treatment map 

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
	        if (tile_id == 3 || tile_id == 4) {
		        int[] tabcornerId = new int[9];
		        //[ 6 , 7 , 8 ]
		        //[ 3 , 4 , 5 ]		Tab idCorner  3 = dirtgrass    2 = dirt
		        //[ 0 , 1 , 2 ]
		        int counter = 0;
		        for (int a = -1; a <= 1; a++) {
			        for (int b = -1; b <= 1; b++) {
				        tabcornerId[counter] = noise_grid[x+b][y+a];
				        Debug.Log("b:"+b+" a:"+a+" counter:"+counter);
				        counter++;
			        }
		        }
		        int whichTile = whichDirtGrass(tabcornerId);
		        switch (whichTile) {
			        case 0:
				        tile_prefab = tileset[3][0];
				        tile_group = tile_groups[3]; 
				        break;
			        case 1:
				        tile_prefab = tileset[3][1];
				        tile_group = tile_groups[3]; 
				        break;
			        case 2:
				        tile_prefab = tileset[3][2];
				        tile_group = tile_groups[3];  
				        break;
			        case 3:
				        tile_prefab = tileset[3][3];
				        tile_group = tile_groups[3]; 
				        break;
			        case 4:
				        tile_prefab = tileset[3][4];
				        tile_group = tile_groups[3]; 
				        break;
			        case 5:
				        tile_prefab = tileset[3][5];
				        tile_group = tile_groups[3]; 
				        break;
			        case 6:
				        tile_prefab = tileset[3][6];
				        tile_group = tile_groups[3]; 
				        break;
			        case 7:
				        tile_prefab = tileset[3][7];
				        tile_group = tile_groups[3]; 
				        break;
			        case 8:
				        tile_prefab = tileset[3][8];
				        tile_group = tile_groups[3]; 
				        break;
			        case 9:
				        tile_prefab = tileset[3][9];
				        tile_group = tile_groups[3]; 
				        break;
			        case 10:
				        tile_prefab = tileset[3][10];
				        tile_group = tile_groups[3]; 
				        break;
			        case 11:
				        tile_prefab = tileset[3][11];
				        tile_group = tile_groups[3]; 
				        break;
			        case 12:
				        int size = tileset[4].Count;
				        if (size > 1) {
					        numberTile = Random.Range(0, size);
				        }
				        tile_prefab = tileset[4][numberTile];
				        tile_group = tile_groups[3]; 
				        break;
			        default:
				        int size2 = tileset[2].Count;
				        if (size2 > 1) {
					        numberTile = Random.Range(0, size2);
				        }
				        tile_prefab = tileset[2][numberTile];
				        tile_group = tile_groups[3]; 
				        break;
		        }
	        }
	        else {
		        int size = tileset[tile_id].Count;
		        if (size > 1) {
			        numberTile = Random.Range(0, size);
		        }
		        tile_prefab = tileset[tile_id][numberTile];
		        tile_group = tile_groups[tile_id]; 
	        }
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

    private int whichDirtGrass(int[] tabcornerId) {
	    bool[] isDirt = new bool[9];
	    for (int i = 0; i < tabcornerId.Length; i++) {
		    isDirt[i] = tabcornerId[i] == 2 || tabcornerId[i] == 1;
	    }
	    
	    
	    // center
	    if (!isDirt[0] && !isDirt[1] && !isDirt[2] && !isDirt[3] && !isDirt[5] && !isDirt[6] && !isDirt[7] && !isDirt[8]) {
		    return 12;
	    }
	    
	    // inner corner
	    // BR
	    if (!isDirt[0] && !isDirt[1] && isDirt[2] && !isDirt[3] && !isDirt[5] && !isDirt[6] && !isDirt[7] && !isDirt[8]) {
		    return 0;
	    }
	    // BL
	    if (isDirt[0] && !isDirt[1] && !isDirt[2] && !isDirt[3] && !isDirt[5] && !isDirt[6] && !isDirt[7] && !isDirt[8]) {
		    return 1;
	    }
	    // TR
	    if (!isDirt[0] && !isDirt[1] && !isDirt[2] && !isDirt[3] && !isDirt[5] && !isDirt[6] && !isDirt[7] && isDirt[8]) {
		    return 4;
	    }
	    // TL
	    if (!isDirt[0] && !isDirt[1] && !isDirt[2] && !isDirt[3] && !isDirt[5] && isDirt[6] && !isDirt[7] && !isDirt[8]) {
		    return 5;
	    }
	    
	    // sides
	    // top / bottom
	    if (!isDirt[3] && !isDirt[5]) {
		    //up
		    if (!isDirt[7]) {
			    return 2;
		    }
		    // down
		    if (!isDirt[1]) {
			    return 6;
		    }
	    }
	    // right / left 
	    if (!isDirt[1] && !isDirt[7]) {
		    //up
		    if (!isDirt[3]) {
			    return 7;
		    }
		    // down
		    if (!isDirt[5]) {
			    return 3;
		    }
	    }


	    // outer Corner
	    // BR
	    if (isDirt[1] && isDirt[5]) {
		    return 8;
	    }
	    // BL
	    if (isDirt[1] && isDirt[3]) {
		    return 9;
	    }
	    // TR
	    if (isDirt[5] && isDirt[7]) {
		    return 10;
	    }
	    // TL
	    if (isDirt[3] && isDirt[7]) {
		    return 11;
	    }
	    return 13;
    }

    public SyncList<List<int>> getNoiseGrid() {
	    return noise_grid;
    }

    private void Start() {
	    CreateTileset();
	    CreateTileGroups();
	    if (isServer) {
		    GenerateMap();
	    }
	    RenderMap();
    }

    /*
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
    */
}
