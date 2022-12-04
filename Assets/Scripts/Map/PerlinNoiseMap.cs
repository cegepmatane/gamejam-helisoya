
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;
using Random = UnityEngine.Random;
using UnityEngine.Tilemaps;


public class PerlinNoiseMap : NetworkBehaviour {

    public GameObject MiniMapCam;

    Dictionary<int, List<TileBase>> tileset;
    [Space(3)]
    [Header("Tiles Set")]
    [Space(1)]
    public List<TileBase> prefab_Rock = new List<TileBase>();
    public List<TileBase> prefab_Dirt_Rock = new List<TileBase>();
    public List<TileBase> prefab_Dirt = new List<TileBase>();
    public List<TileBase> prefab_DirtyGrass = new List<TileBase>();
    public List<TileBase> prefab_Grass = new List<TileBase>();
    public List<TileBase> prefab_Tree = new List<TileBase>();
    public List<TileBase> prefab_Wall = new List<TileBase>();
    public TileBase prefab_default;
    public GameObject prefab_AmmunitionCrate;

    public int map_width = 100;
    public int map_height = 100;

    SyncList<List<int>> noise_grid = new SyncList<List<int>>();
    //List<List<GameObject>> tile_grid = new List<List<GameObject>>();

    // recommend 4 to 20
    float magnification = 7.0f;

    int x_offset = 0; // <- +>
    int y_offset = 0; // v- +^

    // BigFoot
    private List<Vector3> Targets = new List<Vector3>();


    [Header("TileMaps")]
    public Tilemap groundTilemap;
    public Tilemap impassibleTilemap;
    public Tilemap treeTilemap;

    private void Awake() {
        MiniMapCam = GetComponentInChildren<Camera>().gameObject;
    }

    void CreateTileset() {
        // Liste de Game object pour avoir un truc randoms dan le tileset
        tileset = new Dictionary<int, List<TileBase>>();
        tileset.Add(0, prefab_Rock);
        tileset.Add(1, prefab_Dirt_Rock);
        tileset.Add(2, prefab_Dirt);
        tileset.Add(3, prefab_DirtyGrass);
        tileset.Add(4, prefab_Grass);
        tileset.Add(5, prefab_Grass);
        tileset.Add(6, prefab_Tree);
        tileset.Add(7, prefab_Tree);
    }

    void GenerateMap() {
        /** Generate a 2D grid using the Perlin noise fuction, storing it as
    		both raw ID values and tile gameobjects **/
        magnification = Random.Range(8, 15);
        x_offset = Random.Range(-999999, 999999);
        y_offset = Random.Range(-999999, 999999);
        for (int x = 0; x < map_width; x++) {
            noise_grid.Add(new List<int>());
            for (int y = 0; y < map_height; y++) {
                int tile_id = GetIdUsingPerlin(x, y);
                noise_grid[x].Add(tile_id);
            }
        }
        potGenerateTreatment();
    }

    void potGenerateTreatment() {
        // player spawn point Folder
        GameObject SpawnPoints = new GameObject("SpawnPoints");
        SpawnPoints.transform.parent = gameObject.transform;
        SpawnPoints.transform.localPosition = new Vector3(0, 0, 0);
        // Ammo spawn point Folder
        GameObject AmmunitionsCrates = new GameObject("AmmunitionsCrates");
        AmmunitionsCrates.transform.parent = gameObject.transform;
        AmmunitionsCrates.transform.localPosition = new Vector3(0, 0, 0);
        for (int x = 0; x < map_width; x++) {
            for (int y = 0; y < map_height; y++) {
                // wall
                if (x == 0 || x == map_width - 1 || y == 0 || y == map_height - 1) {
                    noise_grid[x][y] = 10;
                }
                if (noise_grid[x][y] > 0 && noise_grid[x][y] < 6)
                {
                    // player spawn point
                    if (Random.Range(0, 100) == 0) {
                        GameObject spawnPoint = new GameObject();
                        spawnPoint.transform.parent = SpawnPoints.transform;
                        spawnPoint.name = string.Format("spawnPoint_x{0}_y{1}", x, y);
                        spawnPoint.transform.localPosition = new Vector3(x, y, 0);
                        spawnPoint.AddComponent<NetworkStartPosition>();
                    }
                    // Ammo spawn point
                    if (Random.Range(0, 200) == 0) {
                        GameObject AmmoSpawnPoint = new GameObject();
                        AmmoSpawnPoint.name = string.Format("AmmoSpawnPoint_x{0}_y{1}", x, y);
                        AmmoSpawnPoint.transform.position = new Vector3(x, y, -1);
                        AmmoSpawnPoint.transform.parent = AmmunitionsCrates.transform;
                        GameObject AmmunitionsCrate = Instantiate(prefab_AmmunitionCrate, AmmoSpawnPoint.transform);
                        AmmunitionsCrate.name = string.Format("Ammunition_x{0}_y{1}", x, y);
                        AmmunitionsCrate.transform.localPosition = new Vector3(x, y, -1);
                        NetworkServer.Spawn(AmmunitionsCrate);
                    }
                    // BigFoot targets
                    if (Random.Range(0, 900) == 0) {
                        Targets.Add(new Vector3(x, y, 0));
                    }

                }

            }
        }
        FindObjectOfType<BigfootController>().Init();
    }



    void RenderMap()
    {
        for (int x = 0; x < map_width; x++) {
            //tile_grid.Add(new List<GameObject>());
            for (int y = 0; y < map_height; y++) {
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
        if (scaled_perlin == tileset.Count) {
            scaled_perlin = (tileset.Count - 1);
        }
        return Mathf.FloorToInt(scaled_perlin);
    }

    void CreateTile(int tile_id, int x, int y) {
        /** Creates a new tile using the type id code, group it with common
    		tiles, set it's position and store the gameobject. **/

        TileBase tile_prefab = prefab_default;
        Tilemap tile_group = groundTilemap;

        if (tile_id <= 0) {
            tile_group = impassibleTilemap;
        }
        if (tile_id >= 6) {
            groundTilemap.SetTile(new Vector3Int(x, y, 0), prefab_Grass[0]);
            tile_group = treeTilemap;
        }

        int numberTile = 0;
        if (tile_id != 10 && tileset[tile_id].Any()) {
            if (tile_id == 3) {
                int[] tabcornerId = new int[9];
                //[ 6 , 7 , 8 ]
                //[ 3 , 4 , 5 ]		Tab idCorner  3 = dirtgrass    2 = dirt
                //[ 0 , 1 , 2 ]
                int counter = 0;
                for (int a = -1; a <= 1; a++) {
                    for (int b = -1; b <= 1; b++) {
                        tabcornerId[counter] = noise_grid[x + b][y + a];
                        counter++;
                    }
                }
                int whichTile = whichDirtGrass(tabcornerId);

                if (whichTile <= 11 && whichTile >= 0) {
                    tile_prefab = tileset[3][whichTile];
                }
                else if (whichTile == 12) {
                    int size = tileset[4].Count;
                    if (size > 1) {
                        numberTile = Random.Range(0, size);
                    }
                    tile_prefab = tileset[4][numberTile];
                }
                else {
                    int size2 = tileset[2].Count;
                    if (size2 > 1) {
                        numberTile = Random.Range(0, size2);
                    }
                    tile_prefab = tileset[2][numberTile];
                }
            }
            else {
                int size = tileset[tile_id].Count;
                if (size > 1) {
                    numberTile = Random.Range(0, size);
                }
                tile_prefab = tileset[tile_id][numberTile];
            }
        }
        else {
            int size = prefab_Wall.Count;
            if (size > 1) {
                numberTile = Random.Range(0, size);
            }
            if (prefab_Wall.Any()) {
                tile_prefab = prefab_Wall[numberTile];
            }
            tile_group = impassibleTilemap;
        }

        tile_group.SetTile(new Vector3Int(x, y, 0), tile_prefab);
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
    
    
    public Vector3 getBigFootSpawn() {
        if (Targets.Any()) {
            return Targets[0];
        }
        return Vector3.zero;
    }

    public List<Vector3> getTargets() {
        return Targets;
    }

    void SpawCamera() {
        float xCam = ((map_width / 2f) - 0.5f);
        float yCam = ((map_height / 2f) - 0.5f);
        MiniMapCam.transform.position = new Vector3(xCam, yCam, -10);
        MiniMapCam.GetComponent<Camera>().orthographicSize = map_width / 2f;
    }

    private void Start() {
        CreateTileset();
        if (isServer) {
            GenerateMap();
        }
        RenderMap();
        SpawCamera();
    }

    private void OnDrawGizmos() {
        if (Event.current.control) {
            drawGrid();
            drawValueGen();
        }
    }
    
    #if UNITY_EDITOR
    private void drawGrid() {
        Vector3 _position = transform.position;
        float lenght;
        Vector3 t_D, t_A;
        Gizmos.color = Color.white;
        // vertical lignes
        for (int i = 0; i <= map_width; i++) {
            lenght = map_height;
            t_D = _position + (Vector3.right * i);
            t_A = t_D + (Vector3.up * lenght); 
            Gizmos.DrawLine(t_D, t_A);
        }
        // horizontal lignes
        for (int i = 0; i <= map_height; i++) {
            lenght = map_width;
            t_D = _position + (Vector3.up * i );
            t_A = t_D + (Vector3.right * lenght);
            Gizmos.DrawLine(t_D, t_A);
        }
    }
    
    private void drawValueGen() {
        int _tempNoiseValue;
        for (int x = 0; x < map_width; x++) {
            for (int y = 0; y < map_height; y++) {
                _tempNoiseValue = noise_grid[x][y];
                /*
        tileset.Add(0, prefab_Rock);
        tileset.Add(1, prefab_Dirt_Rock);
        tileset.Add(2, prefab_Dirt);
        tileset.Add(3, prefab_DirtyGrass);
        tileset.Add(4, prefab_Grass);
        tileset.Add(5, prefab_Grass);
        tileset.Add(6, prefab_Tree);
        tileset.Add(7, prefab_Tree);
                 */
                switch (_tempNoiseValue) {
                    case 0:
                        Gizmos.color = new Color32(150, 150, 150, 204);
                        break;
                    case 1:
                        Gizmos.color = new Color32(75, 47, 23, 204);
                        break;
                    case 2:
                        Gizmos.color = new Color32(65, 33, 7, 232);
                        break;
                    case 3:
                        Gizmos.color = new Color32(40, 51, 10, 232);
                        break;
                    case 4:
                        Gizmos.color = new Color32(29, 141, 22, 255);
                        break;
                    case 5:
                        Gizmos.color = new Color32(29, 141, 22, 255);
                        break;
                    case 6:
                        Gizmos.color = new Color32(19, 82, 15, 255);
                        break;
                    case 7:
                        Gizmos.color = new Color32(19, 82, 15, 255);
                        break;
                    default:
                        Gizmos.color = Color.magenta;
                        break;
                }
                Gizmos.DrawCube(new Vector3(x+0.5f, y+0.5f, 0), new Vector3(0.5f, 0.5f, 0.5f));
            }
        }
    }
    
    #endif
}
