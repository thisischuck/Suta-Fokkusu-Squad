
	int height;
	int width;
	int length;
    private Dictionary<Vector2, Chunk> objectChunks;
    private CellularAutomata cellular;
    private Vector2 currentChunk;
    private Transform player;
            GameObject newObj = Instantiate(o.GameObject, vertices[x + z * dungeon[y].width + y * dungeon[y].width * dungeon[y].length], Quaternion.Euler(new Vector3(0.0f,0.0f,0.0f))); //buscar a normal do vertice para rotação
			newObj.transform.localScale = Vector3.one * Random.Range(0, maxSize);
	;		newObj.transform.parent = this.transform;
            if(newObj.tag != "Stalactite")
                newObj.transform.localRotation = Quaternion.FromToRotation(-newObj.transform.up, normals[x + z * dungeon[y].width + y * dungeon[y].width * dungeon[y].length]) * newObj.transform.rotation;
			newObj.transform.rotation = Quaternion.AngleAxis(Random.Range(-180, 180), Vector3.up);
			positionsUsed.Add(new Vector3(x, y, z), newObj);
            GameObject newObj = Instantiate(o.GameObject, vertices[x + z * dungeon[y].width + y * dungeon[y].width * dungeon[y].length], Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f)));
			newObj.transform.localScale = Vector3.one * Random.Range(0, maxSize);
			newObj.transform.parent = this.transform;
			if (newObj.tag != "Tree")
				newObj.transform.rotation = Quaternion.FromToRotation(newObj.transform.up, normals[x + z * dungeon[y].width + y * dungeon[y].width * dungeon[y].length]) * newObj.transform.rotation;
			newObj.transform.rotation = Quaternion.AngleAxis(Random.Range(-180, 180), Vector3.up);
			positionsUsed.Add(new Vector3(x, y, z), newObj);
            GameObject newObj = Instantiate(o.GameObject, vertices[x + z * dungeon[y].width + y * dungeon[y].width * dungeon[y].length], Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f)));
			newObj.transform.localScale = Vector3.one * Random.Range(0, maxSize);
			newObj.transform.parent = this.transform;
            newObj.transform.rotation = Quaternion.FromToRotation(newObj.transform.up, normals[x + z * dungeon[y].width + y * dungeon[y].width * dungeon[y].length]) * newObj.transform.rotation;
			newObj.transform.rotation = Quaternion.AngleAxis(Random.Range(-180, 180), Vector3.up);
			positionsUsed.Add(new Vector3(x, y, z), newObj);