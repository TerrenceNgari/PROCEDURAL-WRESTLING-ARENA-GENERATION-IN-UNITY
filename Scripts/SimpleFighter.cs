using UnityEngine;

public class SimpleFighter : MonoBehaviour
{
    public Transform target;

    public Transform attackArm;
    public Transform kickLeg;

    public Animator fighterAnimator;

    public float moveSpeed = 2.2f;
    public float preferredDistance = 0.85f;

    public float attackContactDistance = 0.75f;
    public float attackLungeSpeed = 3.5f;

    public float punchDamage = 15f;
    public float punchDistance = 1.8f;
    public float punchDuration = 0.8f;
    public float punchKnockback = 0.3f;

    public float kickDamage = 25f;
    public float kickDistance = 2f;
    public float kickDuration = 0.5f;
    public float kickKnockback = 0.6f;

    public float roundhouseDamage = 30f;
    public float roundhouseDistance = 2.2f;
    public float roundhouseDuration = 0.6f;
    public float roundhouseKnockback = 0.9f;

    public float uppercutDamage = 30f;
    public float uppercutDistance = 1.8f;
    public float uppercutDuration = 0.65f;
    public float uppercutKnockback = 1f;

    public float minimumAttackDelay = 0.7f;
    public float maximumAttackDelay = 1.4f;

    private float nextAttackTime;

    private Transform rightUpperLeg;
    private Transform rightLowerLeg;
    private Transform rightUpperArm;
    private Transform rightLowerArm;

    private enum AttackType
    {
        None,
        Punch,
        FrontKick,
        Roundhouse,
        Uppercut
    }

    private AttackType currentAttack =
        AttackType.None;

    private float attackTimer;
    private float visualExtension;

    private bool damageDealt;

    private Vector3 armStartPosition;
    private Quaternion armStartRotation;

    private Vector3 legStartPosition;
    private Quaternion legStartRotation;

    void Start()
    {
        if (attackArm != null)
        {
            armStartPosition =
                attackArm.localPosition;

            armStartRotation =
                attackArm.localRotation;
        }

        if (kickLeg != null)
        {
            legStartPosition =
                kickLeg.localPosition;

            legStartRotation =
                kickLeg.localRotation;
        }

        if (fighterAnimator == null)
            fighterAnimator =
                GetComponentInChildren<Animator>();

        if (fighterAnimator != null)
        {
            // Remove the old visible-model offset.
            Vector3 p =
                fighterAnimator.transform.localPosition;

            fighterAnimator.transform.localPosition =
                new Vector3(
                    0f,
                    p.y,
                    0f
                );

            // AI visual should follow parent facing.
            fighterAnimator.transform.localRotation =
                Quaternion.identity;

            rightUpperLeg =
                fighterAnimator.GetBoneTransform(
                    HumanBodyBones.RightUpperLeg);

            rightLowerLeg =
                fighterAnimator.GetBoneTransform(
                    HumanBodyBones.RightLowerLeg);

            rightUpperArm =
                fighterAnimator.GetBoneTransform(
                    HumanBodyBones.RightUpperArm);

            rightLowerArm =
                fighterAnimator.GetBoneTransform(
                    HumanBodyBones.RightLowerArm);

            fighterAnimator.Play(
                "Base Layer.Idle",
                0,
                0f);
        }

        FaceTarget();
        KeepInsideRing();
        ScheduleNextAttack();
    }

    void Update()
    {
        if (target == null)
            return;

        // Always face Wrestler 1
        FaceTarget();

        if (currentAttack ==
            AttackType.None)
        {
            HandleMovement();
            HandleAICombat();
        }
        else
        {
            UpdateCurrentAttack();
        }

        KeepInsideRing();
    }

    void FaceTarget()
    {
        if (target == null)
            return;

        Vector3 direction =
            target.position -
            transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            transform.rotation =
                Quaternion.LookRotation(direction);
        }
    }

    void HandleMovement()
    {
        float distance =
            Vector3.Distance(
                transform.position,
                target.position);

        if (distance >
            preferredDistance)
        {
            Vector3 direction =
                target.position -
                transform.position;

            direction.y = 0f;

            if (direction.sqrMagnitude >
                0.001f)
            {
                transform.position +=
                    direction.normalized *
                    moveSpeed *
                    Time.deltaTime;

                KeepInsideRing();
            }
        }
    }

    void MoveIntoContact()
    {
        if (target == null)
            return;

        Vector3 direction =
            target.position -
            transform.position;

        direction.y = 0f;

        float distance =
            direction.magnitude;

        if (distance >
            attackContactDistance)
        {
            transform.position +=
                direction.normalized *
                attackLungeSpeed *
                Time.deltaTime;

            KeepInsideRing();
        }
    }

    void HandleAICombat()
    {
        if (Time.time < nextAttackTime)
            return;

        float distance =
            Vector3.Distance(
                transform.position,
                target.position);

        if (distance > 2.2f)
            return;

        int attack =
            Random.Range(0, 4);

        switch (attack)
        {
            case 0:
                StartPunch();
                break;

            case 1:
                StartFrontKick();
                break;

            case 2:
                StartRoundhouse();
                break;

            case 3:
                StartUppercut();
                break;
        }
    }

    void ScheduleNextAttack()
    {
        nextAttackTime =
            Time.time +
            Random.Range(
                minimumAttackDelay,
                maximumAttackDelay);
    }

    void StartPunch()
    {
        FaceTarget();

        currentAttack =
            AttackType.Punch;

        attackTimer = 0f;
        damageDealt = false;

        if (fighterAnimator != null)
        {
            fighterAnimator.Play(
                "Base Layer.Punch",
                0,
                0f);
        }
    }

    void UpdatePunch()
    {
        FaceTarget();
        MoveIntoContact();

        attackTimer += Time.deltaTime;

        float progress =
            Mathf.Clamp01(
                attackTimer /
                punchDuration);

        visualExtension =
            Mathf.Sin(progress * Mathf.PI);

        if (!damageDealt &&
            progress >= 0.5f)
        {
            DealDamage(
                punchDamage,
                punchDistance,
                punchKnockback);

            damageDealt = true;
        }

        if (progress >= 1f)
        {
            ResetArm();

            if (fighterAnimator != null)
            {
                fighterAnimator.CrossFade(
                    "Base Layer.Idle",
                    0.1f);
            }

            FinishAttack();
        }
    }

    void StartFrontKick()
    {
        FaceTarget();

        currentAttack =
            AttackType.FrontKick;

        attackTimer = 0f;
        visualExtension = 0f;
        damageDealt = false;
    }

    void UpdateFrontKick()
    {
        FaceTarget();
        MoveIntoContact();

        attackTimer += Time.deltaTime;

        float progress =
            Mathf.Clamp01(
                attackTimer /
                kickDuration);

        visualExtension =
            Mathf.Sin(progress * Mathf.PI);

        if (!damageDealt &&
            progress >= 0.5f)
        {
            DealDamage(
                kickDamage,
                kickDistance,
                kickKnockback);

            damageDealt = true;
        }

        if (progress >= 1f)
        {
            ResetLeg();
            FinishAttack();
        }
    }

    void StartRoundhouse()
    {
        FaceTarget();

        currentAttack =
            AttackType.Roundhouse;

        attackTimer = 0f;
        visualExtension = 0f;
        damageDealt = false;
    }

    void UpdateRoundhouse()
    {
        FaceTarget();
        MoveIntoContact();

        attackTimer += Time.deltaTime;

        float progress =
            Mathf.Clamp01(
                attackTimer /
                roundhouseDuration);

        visualExtension =
            Mathf.Sin(progress * Mathf.PI);

        if (!damageDealt &&
            progress >= 0.5f)
        {
            DealDamage(
                roundhouseDamage,
                roundhouseDistance,
                roundhouseKnockback);

            damageDealt = true;
        }

        if (progress >= 1f)
            FinishAttack();
    }

    void StartUppercut()
    {
        FaceTarget();

        currentAttack =
            AttackType.Uppercut;

        attackTimer = 0f;
        visualExtension = 0f;
        damageDealt = false;
    }

    void UpdateUppercut()
    {
        FaceTarget();
        MoveIntoContact();

        attackTimer += Time.deltaTime;

        float progress =
            Mathf.Clamp01(
                attackTimer /
                uppercutDuration);

        visualExtension =
            Mathf.Sin(progress * Mathf.PI);

        if (!damageDealt &&
            progress >= 0.45f)
        {
            DealDamage(
                uppercutDamage,
                uppercutDistance,
                uppercutKnockback);

            damageDealt = true;
        }

        if (progress >= 1f)
        {
            ResetArm();
            FinishAttack();
        }
    }

    void UpdateCurrentAttack()
    {
        switch (currentAttack)
        {
            case AttackType.Punch:
                UpdatePunch();
                break;

            case AttackType.FrontKick:
                UpdateFrontKick();
                break;

            case AttackType.Roundhouse:
                UpdateRoundhouse();
                break;

            case AttackType.Uppercut:
                UpdateUppercut();
                break;
        }
    }

    void LateUpdate()
    {
        if (currentAttack ==
            AttackType.FrontKick)
        {
            if (rightUpperLeg != null)
            {
                rightUpperLeg.rotation =
                    Quaternion.AngleAxis(
                        -105f *
                        visualExtension,
                        transform.right) *
                    rightUpperLeg.rotation;
            }

            if (rightLowerLeg != null)
            {
                rightLowerLeg.rotation =
                    Quaternion.AngleAxis(
                        45f *
                        visualExtension,
                        transform.right) *
                    rightLowerLeg.rotation;
            }
        }

        if (currentAttack ==
            AttackType.Roundhouse)
        {
            if (rightUpperLeg != null)
            {
                rightUpperLeg.rotation =
                    Quaternion.AngleAxis(
                        -80f *
                        visualExtension,
                        transform.right) *
                    rightUpperLeg.rotation;

                rightUpperLeg.rotation =
                    Quaternion.AngleAxis(
                        100f *
                        visualExtension,
                        transform.up) *
                    rightUpperLeg.rotation;
            }

            if (rightLowerLeg != null)
            {
                rightLowerLeg.rotation =
                    Quaternion.AngleAxis(
                        30f *
                        visualExtension,
                        transform.right) *
                    rightLowerLeg.rotation;
            }
        }

        if (currentAttack ==
            AttackType.Uppercut)
        {
            if (rightUpperArm != null)
            {
                rightUpperArm.rotation =
                    Quaternion.AngleAxis(
                        -120f *
                        visualExtension,
                        transform.right) *
                    rightUpperArm.rotation;
            }

            if (rightLowerArm != null)
            {
                rightLowerArm.rotation =
                    Quaternion.AngleAxis(
                        -70f *
                        visualExtension,
                        transform.right) *
                    rightLowerArm.rotation;
            }
        }
    }

    void DealDamage(
        float amount,
        float requiredDistance,
        float knockback)
    {
        if (target == null)
            return;

        float distance =
            Vector3.Distance(
                transform.position,
                target.position);

        if (distance > requiredDistance)
            return;

        FighterHealth health =
            target.GetComponent<FighterHealth>();

        if (health == null ||
            health.IsKnockedOut())
            return;

        health.TakeDamage(amount);

        Vector3 direction =
            target.position -
            transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            target.position +=
                direction.normalized *
                knockback;

            ClampToCenter(target);
        }
    }

    void FinishAttack()
    {
        currentAttack =
            AttackType.None;

        visualExtension = 0f;
        damageDealt = false;

        ScheduleNextAttack();
    }

    void ResetArm()
    {
        if (attackArm != null)
        {
            attackArm.localPosition =
                armStartPosition;

            attackArm.localRotation =
                armStartRotation;
        }
    }

    void ResetLeg()
    {
        if (kickLeg != null)
        {
            kickLeg.localPosition =
                legStartPosition;

            kickLeg.localRotation =
                legStartRotation;
        }
    }

    void KeepInsideRing()
    {
        ClampToCenter(transform);
    }

    void ClampToCenter(
        Transform fighter)
    {
        GameObject ring =
            GameObject.Find("Ring Floor");

        if (ring == null)
            return;

        Renderer renderer =
            ring.GetComponent<Renderer>();

        if (renderer == null)
            return;

        Bounds bounds =
            renderer.bounds;

        Vector3 center =
            bounds.center;

        Vector3 position =
            fighter.position;

        // Keep the AI and player close to
        // the real generated ring centre.
        float allowedX =
            bounds.extents.x * 0.18f;

        float allowedZ =
            bounds.extents.z * 0.18f;

        position.x =
            Mathf.Clamp(
                position.x,
                center.x - allowedX,
                center.x + allowedX);

        position.z =
            Mathf.Clamp(
                position.z,
                center.z - allowedZ,
                center.z + allowedZ);

        fighter.position = position;
    }
}