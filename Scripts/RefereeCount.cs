using System.Collections;
using UnityEngine;
using TMPro;

public class RefereeCount : MonoBehaviour
{
    public Transform wrestler1;
    public Transform wrestler2;

    public Transform countArm;
    public TMP_Text winnerText;

    public float moveSpeed = 2.5f;
    public float stopDistance = 1.5f;

    private FighterHealth health1;
    private FighterHealth health2;

    private Transform knockedOutWrestler;

    private Quaternion armStartRotation;

    private bool counting = false;
    private bool resultDeclared = false;

    private RefereeFollower refereeFollower;

    void Start()
    {
        health1 = wrestler1.GetComponent<FighterHealth>();
        health2 = wrestler2.GetComponent<FighterHealth>();

        refereeFollower = GetComponent<RefereeFollower>();

        if (countArm != null)
        {
            armStartRotation = countArm.localRotation;
        }

        if (winnerText != null)
        {
            winnerText.text = "";
        }
    }

    void Update()
    {
        if (health1 == null || health2 == null)
            return;

        // Automatically reset referee for a new match
        if ((counting || resultDeclared) &&
            !health1.IsKnockedOut() &&
            !health2.IsKnockedOut())
        {
            ResetCount();
            return;
        }

        if (counting || resultDeclared)
            return;

        if (health1.IsKnockedOut())
        {
            BeginCount(wrestler1);
        }
        else if (health2.IsKnockedOut())
        {
            BeginCount(wrestler2);
        }
    }

    void BeginCount(Transform defeatedWrestler)
    {
        knockedOutWrestler = defeatedWrestler;
        counting = true;

        // Stop normal referee following
        if (refereeFollower != null)
        {
            refereeFollower.enabled = false;
        }

        StartCoroutine(CountRoutine());
    }

    IEnumerator CountRoutine()
    {
        // Move referee beside the knocked-out wrestler
        while (Vector3.Distance(
            transform.position,
            knockedOutWrestler.position) > stopDistance)
        {
            Vector3 targetPosition =
                knockedOutWrestler.position +
                new Vector3(1.2f, 0, 0);

            targetPosition.y = transform.position.y;

            transform.position =
                Vector3.MoveTowards(
                    transform.position,
                    targetPosition,
                    moveSpeed * Time.deltaTime
                );

            FaceKnockedOutWrestler();

            yield return null;
        }

        FaceKnockedOutWrestler();

        // THREE COUNT
        for (int count = 1; count <= 3; count++)
        {
            yield return StartCoroutine(TapMat());

            Debug.Log("REFEREE COUNT: " + count);

            if (winnerText != null)
            {
                winnerText.text = count.ToString();
            }

            yield return new WaitForSeconds(0.7f);
        }

        DeclareWinner();
    }

    IEnumerator TapMat()
    {
        if (countArm == null)
            yield break;

        Quaternion downRotation =
            armStartRotation *
            Quaternion.Euler(90f, 0, 0);

        float duration = 0.2f;
        float timer = 0f;

        // Arm goes down
        while (timer < duration)
        {
            timer += Time.deltaTime;

            countArm.localRotation =
                Quaternion.Lerp(
                    armStartRotation,
                    downRotation,
                    timer / duration
                );

            yield return null;
        }

        // Arm comes back up
        timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            countArm.localRotation =
                Quaternion.Lerp(
                    downRotation,
                    armStartRotation,
                    timer / duration
                );

            yield return null;
        }

        countArm.localRotation = armStartRotation;
    }

    void FaceKnockedOutWrestler()
    {
        Vector3 direction =
            knockedOutWrestler.position -
            transform.position;

        direction.y = 0;

        if (direction.sqrMagnitude > 0.001f)
        {
            transform.rotation =
                Quaternion.LookRotation(direction);
        }
    }

    void DeclareWinner()
    {
        counting = false;
        resultDeclared = true;

        if (winnerText == null)
            return;

        if (knockedOutWrestler == wrestler1)
        {
            winnerText.text = "WRESTLER 2 WINS!";
        }
        else
        {
            winnerText.text = "WRESTLER 1 WINS!";
        }
    }

    void ResetCount()
    {
        StopAllCoroutines();

        counting = false;
        resultDeclared = false;
        knockedOutWrestler = null;

        if (countArm != null)
        {
            countArm.localRotation = armStartRotation;
        }

        if (winnerText != null)
        {
            winnerText.text = "";
        }

        if (refereeFollower != null)
        {
            refereeFollower.enabled = true;
        }
    }
}