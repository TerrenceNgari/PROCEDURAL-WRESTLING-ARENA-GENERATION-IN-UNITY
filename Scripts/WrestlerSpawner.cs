using UnityEngine;

public class WrestlerSpawner : MonoBehaviour
{
    public GameObject wrestler1;
    public GameObject wrestler2;
    public MatchGenerator matchGenerator;

    public float spawnDistance = 0.45f;
    public float heightOffset = 0.8f;

    void Start()
    {
        PositionWrestlers();
    }

    public void PositionWrestlers()
    {
        if (wrestler1 == null ||
            wrestler2 == null ||
            matchGenerator == null)
            return;

        float ringHeight =
            matchGenerator.GetRingHeight();

        Vector3 center = Vector3.zero;

        GameObject ringFloor =
            GameObject.Find("Ring Floor");

        if (ringFloor != null)
        {
            Renderer renderer =
                ringFloor.GetComponent<Renderer>();

            if (renderer != null)
                center = renderer.bounds.center;
            else
                center = ringFloor.transform.position;
        }

        float y =
            ringHeight + heightOffset;

        wrestler1.transform.position =
            new Vector3(
                center.x + spawnDistance,
                y,
                center.z
            );

        wrestler2.transform.position =
            new Vector3(
                center.x - spawnDistance,
                y,
                center.z
            );

        FaceTowards(
            wrestler1.transform,
            wrestler2.transform
        );

        FaceTowards(
            wrestler2.transform,
            wrestler1.transform
        );

        FighterHealth health1 =
            wrestler1.GetComponent<FighterHealth>();

        if (health1 != null)
            health1.ResetFighter();

        FighterHealth health2 =
            wrestler2.GetComponent<FighterHealth>();

        if (health2 != null)
            health2.ResetFighter();
    }

    void FaceTowards(
        Transform fighter,
        Transform opponent)
    {
        Vector3 direction =
            opponent.position -
            fighter.position;

        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            fighter.rotation =
                Quaternion.LookRotation(direction);
        }
    }
}