using UnityEngine;

public class MatchGenerator : MonoBehaviour
{
    public Material classicArena;
    public Material darkArena;
    public Material extremeArena;

    public Material redRope;
    public Material blueRope;
    public Material whiteRope;
    public Material blackRope;

    public Material blackFloor;
    public Material grayFloor;
    public Material redFloor;
    private GameObject generatedArena;
    private float ringHeight;
    private WrestlerSpawner wrestlerSpawner;
    void Start()
    {
        Debug.Log("PCG MATCH GENERATOR IS RUNNING!");
    }

    public void GenerateArenaInEditor()
    {
        ClearArena();

        generatedArena = new GameObject("Generated Arena");

        
        // Random ring size
        float ringSize = Random.Range(5f, 8f);

        // Random ring elevation
        ringHeight = Random.Range(0.3f, 1.0f);


        Debug.Log("Generated Ring Size: " + ringSize);
        Debug.Log("Generated Ring Height: " + ringHeight);

        // Random floor material
        Material selectedFloorMaterial;

        int floorType = Random.Range(0, 3);

        if (floorType == 0)
        {
            selectedFloorMaterial = blackFloor;
        }
        else if (floorType == 1)
        {
            selectedFloorMaterial = grayFloor;
        }
        else
        {
            selectedFloorMaterial = redFloor;
        }

        Debug.Log("Generated Floor: " + selectedFloorMaterial.name);

        // Create the arena floor based on ring size
        GameObject arenaFloor = CreateCube(
            "Arena Floor",
            new Vector3(0, -0.2f, 0),
            new Vector3(ringSize + 5f, 0.2f, ringSize + 5f)
        );
        arenaFloor.GetComponent<Renderer>().material = selectedFloorMaterial;

        //Random arena theme
        Material selectedMaterial;

        int theme = Random.Range(0, 3);

        if (theme == 0)
        {
            selectedMaterial = classicArena;
        }
        else if (theme == 1)
        {
            selectedMaterial = darkArena;
        }
        else
        {
            selectedMaterial = extremeArena;
        }

        Debug.Log("Generated Arena Theme: " + selectedMaterial.name);

        // Random rope color

        Material selectedRopeMaterial;

        int ropeColor = Random.Range(0, 4);

        if (ropeColor == 0)
        {
            selectedRopeMaterial = redRope;
        }
        else if (ropeColor == 1)
        {
            selectedRopeMaterial = blueRope;
        }
        else if (ropeColor == 2)
        {
            selectedRopeMaterial = whiteRope;
        }
        else
        {
            selectedRopeMaterial = blackRope;
        }

        Debug.Log("Generated Rope Color: " + selectedRopeMaterial.name);
        // Ring floor
        GameObject ringFloor = CreateCube(
            "Ring Floor",
            new Vector3(0, ringHeight, 0),
            new Vector3(ringSize, 0.3f, ringSize)
        );
        
        ringFloor.GetComponent<Renderer>().material = selectedMaterial;

        // Four posts
        CreatePost(
            "Post 1",
            new Vector3(-ringSize + 1, 1, -ringSize + 1)
        );

        CreatePost(
            "Post 2",
            new Vector3(ringSize - 1, 1, -ringSize + 1)
        );

        CreatePost(
            "Post 3",
            new Vector3(-ringSize + 1, 1, ringSize - 1)
        );

        CreatePost(
            "Post 4",
            new Vector3(ringSize - 1, 1, ringSize - 1)
        );

        // Generate the ropes
        CreateRopes(ringSize);

        // Generate turnbuckle pads
        CreateTurnbucklePads(ringSize);

        //Generate random barricades
        CreateBarricades(ringSize);

        // Generate random entrance
        CreateEntrance(ringSize);

       // Apply the selected arena theme
       Renderer[] renderers = generatedArena.GetComponentsInChildren<Renderer>();

       foreach (Renderer renderer in renderers)
       {
           if (renderer.gameObject.name.Contains("Rope"))
           {
               renderer.material = selectedRopeMaterial;
           }
           else if (renderer.gameObject.name.Contains("ArenaFloor"))
           {
               renderer.material = selectedFloorMaterial;
           }
           else if (renderer.gameObject.name.Contains("Turnbuckle Pad"))
           {
               // Keep the pad's randomly selected material
           }
           else
            {
                renderer.material = selectedMaterial;
            }
       }

       Debug.Log("Arena generated successfully!");
       wrestlerSpawner.PositionWrestlers();
 }

    void CreateRopes(float ringSize)
    {
         float ropeLength = ringSize - 1f;

    // Randomly choose 2 or 3 ropes
    int ropeCount = Random.Range(2, 4);

    Debug.Log("Generated Rope Count: " + ropeCount);

    float[] ropeHeights;

    if (ropeCount == 2)
    {
        ropeHeights = new float[] { 0.8f, 1.5f };
    }
    else
    {
        ropeHeights = new float[] { 0.7f, 1.2f, 1.7f };
    }

    

    // FRONT
    foreach (float height in ropeHeights)
    {
        CreateCube(
            "Front Rope",
            new Vector3(0, height + ringHeight, -ringSize + 1),
            new Vector3(ropeLength, 0.08f, 0.08f)
        );
    }

    // BACK
    foreach (float height in ropeHeights)
    {
        CreateCube(
            "Back Rope",
            new Vector3(0, height + ringHeight, ringSize - 1),
            new Vector3(ropeLength, 0.08f, 0.08f)
        );
    }

    // LEFT
    foreach (float height in ropeHeights)
    {
        CreateCube(
            "Left Rope",
            new Vector3(-ringSize + 1, height + ringHeight, 0),
            new Vector3(0.08f, 0.08f, ropeLength)
        );
    }

    // RIGHT
    foreach (float height in ropeHeights)
    {
        CreateCube(
            "Right Rope",
            new Vector3(ringSize - 1, height + ringHeight, 0),
            new Vector3(0.08f, 0.08f, ropeLength)
        );
    }

}

    void CreateBarricades(float ringSize)
    {
        // Randomly choose 2, 4, or 6 barricades
        int barricadeCount = Random.Range(1,4) * 2;

        Debug.Log("Generated Barricade Count:" + barricadeCount);

        float distance = ringSize + 1.8f;

        for (int i = 0; i < barricadeCount; i++){
            float angle = (360f / barricadeCount) * i;

            float x = Mathf.Cos(angle * Mathf.Deg2Rad) * distance;
            float z = Mathf.Sin(angle * Mathf.Deg2Rad) * distance;

            GameObject barricade = CreateCube(
                "Barricade" + (i + 1),
                new Vector3(x, 0.45f, z),
                new Vector3(2f, 0.9f, 0.25f)
            );

            barricade.transform.LookAt(
                new Vector3(0, 0.45f, 0));
        }
    }

    void CreateEntrance(float ringSize)
{
    int entranceSide = Random.Range(0, 4);

    float distance = ringSize + 3.5f;

    Vector3 position;

    if (entranceSide == 0)
    {
        // Front
        position = new Vector3(0, 0.3f, -distance);
    }
    else if (entranceSide == 1)
    {
        // Back
        position = new Vector3(0, 0.3f, distance);
    }
    else if (entranceSide == 2)
    {
        // Left
        position = new Vector3(-distance, 0.3f, 0);
    }
    else
    {
        // Right
        position = new Vector3(distance, 0.3f, 0);
    }

    // Random entrance size
    int entranceSize = Random.Range(0, 3);

    float stageWidth;
    float stageDepth;
    float backdropHeight;

    if (entranceSize == 0)
    {
        //small
        stageWidth = 2.5f;
        stageDepth = 2f;
        backdropHeight = 1.5f;
    }
    else if (entranceSize == 1)
    {
        //Medium
        stageWidth = 3.5f;
        stageDepth = 2.5f;
        backdropHeight = 2f;
    }
    else
    {
        //Large
        stageWidth = 5f;
        stageDepth = 3.5f;
        backdropHeight = 2.5f;
    }

    // Main entrance stage
    GameObject entrance = CreateCube(
        "Entrance Stage",
        position,
        new Vector3(stageWidth, 0.6f, stageDepth)
    );

    entrance.transform.LookAt(
        new Vector3(0, 0.3f, 0)
    );

    // Entrance backdrop
    Vector3 backdropPosition = position + Vector3.up * (backdropHeight + 0.3f);

    GameObject backdrop = CreateCube(
        "Entrance Backdrop",
        backdropPosition,
        new Vector3(stageWidth, backdropHeight, 0.3f)
    );

    backdrop.transform.LookAt(
        new Vector3(0, backdropHeight, 0)
    );

    Debug.Log("Generated Entrance Side: " + entranceSide);
    Debug.Log("Generated Entrance Size: " + entranceSize);
}

    GameObject CreateCube(string objectName, Vector3 position, Vector3 scale)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);

        obj.name = objectName;
        obj.transform.position = position;
        obj.transform.localScale = scale;

        obj.transform.SetParent(generatedArena.transform);

        return obj;

    }

    void CreatePost(string objectName, Vector3 position)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);

        obj.name = objectName;
        obj.transform.position = position;
        
        // Random post height
        float postHeight = Random.Range(0.8f, 1.4f);

        obj.transform.position = new Vector3(
            position.x, 
            postHeight + ringHeight, 
            position.z
        );

        obj.transform.localScale = new Vector3(0.2f, postHeight, 0.2f);

        obj.transform.SetParent(generatedArena.transform);
    }

    void CreateTurnbucklePads(float ringSize)
{
    float padSize = 0.35f;

    Vector3[] corners =
    {
        new Vector3(-ringSize + 1, 1 + ringHeight, -ringSize + 1),
        new Vector3(ringSize - 1, 1 + ringHeight, -ringSize + 1),
        new Vector3(-ringSize + 1, 1 + ringHeight, ringSize - 1),
        new Vector3(ringSize - 1, 1 + ringHeight, ringSize - 1)
    };

    for (int i = 0; i < corners.Length; i++)
    {
        GameObject pad = CreateCube(
            "Turnbuckle Pad " + (i + 1),
            corners[i],
            new Vector3(padSize, padSize, padSize)
        );

        // Randomly choose a pad color
        int colorChoice = Random.Range(0, 3);

        if (colorChoice == 0)
        {
            pad.GetComponent<Renderer>().material = redRope;
        }
        else if (colorChoice == 1)
        {
            pad.GetComponent<Renderer>().material = blueRope;
        }
        else
        {
            pad.GetComponent<Renderer>().material = blackRope;
        }
    }
}
    public void ClearArena()
    {
        GameObject existingArena = GameObject.Find("Generated Arena");

        if (existingArena != null)
        {
            DestroyImmediate(existingArena);
        }

        generatedArena = null;
    }

    public float GetRingHeight()
    {
        return ringHeight;
    }
}