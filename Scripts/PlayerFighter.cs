using UnityEngine;

public class PlayerFighter : MonoBehaviour
{
    public Transform opponent;

    public Transform attackArm;
    public Transform kickLeg;

    public Animator fighterAnimator;

    public float moveSpeed = 3f;

    public float attackContactDistance = 0.75f;
    public float attackLungeSpeed = 3.5f;

    public float attackDistance = 1.8f;
    public float damage = 20f;
    public float knockback = 0.4f;
    public float punchReach = 0.8f;
    public float punchDuration = 1.25f;
    public float attackCooldown = 0.6f;

    public float kickDamage = 30f;
    public float kickDistance = 2f;
    public float kickReach = 2.5f;
    public float kickDuration = 0.5f;
    public float kickCooldown = 1f;
    public float kickKnockback = 0.7f;

    public float roundhouseDamage = 35f;
    public float roundhouseDistance = 2.2f;
    public float roundhouseDuration = 0.6f;
    public float roundhouseCooldown = 1.4f;
    public float roundhouseKnockback = 1.1f;

    public float uppercutDamage = 35f;
    public float uppercutDistance = 1.8f;
    public float uppercutReach = 1.2f;
    public float uppercutDuration = 0.7f;
    public float uppercutCooldown = 1.3f;
    public float uppercutKnockback = 1.2f;

    public float spinKickDamage = 25f;
    public float spinKickDistance = 2.2f;
    public float spinKickReach = 1.5f;
    public float spinKickDuration = 0.65f;
    public float spinKickCooldown = 1.5f;
    public float spinKickKnockback = 1.4f;

    public float shoulderDamage = 40f;
    public float shoulderSpeed = 7f;
    public float shoulderDistance = 1.7f;
    public float shoulderDuration = 0.55f;
    public float shoulderCooldown = 1.8f;
    public float shoulderKnockback = 1.8f;

    private Vector3 armStartPosition;
    private Quaternion armStartRotation;

    private Vector3 legStartPosition;
    private Quaternion legStartRotation;

    private Transform rightUpperLeg;
    private Transform rightLowerLeg;
    private Transform rightUpperArm;
    private Transform rightLowerArm;

    private float kickVisual;
    private float roundhouseVisual;
    private float uppercutVisual;

    private bool punching;
    private bool kicking;
    private bool roundhouseKicking;
    private bool uppercutting;
    private bool spinKicking;
    private bool shoulderCharging;

    private bool damageDealt;

    private float punchTimer;
    private float kickTimer;
    private float roundhouseTimer;
    private float uppercutTimer;
    private float spinKickTimer;
    private float shoulderTimer;

    private float nextPunch;
    private float nextKick;
    private float nextRoundhouse;
    private float nextUppercut;
    private float nextSpinKick;
    private float nextShoulder;

    private Quaternion spinStartRotation;
    private Vector3 shoulderDirection;

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
            // Remove old sideways model offset.
            Vector3 p =
                fighterAnimator.transform.localPosition;

            fighterAnimator.transform.localPosition =
                new Vector3(
                    0f,
                    p.y,
                    0f
                );

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

        KeepInsideRing();
    }

    void Update()
    {
        if (!shoulderCharging)
            MovePlayer();

        if (opponent != null &&
            !spinKicking &&
            !shoulderCharging)
        {
            FaceOpponent();
        }

        if (Input.GetKeyDown(KeyCode.Space) &&
            !AnyAttack() &&
            Time.time >= nextPunch)
            StartPunch();

        if (Input.GetKeyDown(KeyCode.E) &&
            !AnyAttack() &&
            Time.time >= nextKick)
            StartKick();

        if (Input.GetKeyDown(KeyCode.C) &&
            !AnyAttack() &&
            Time.time >= nextRoundhouse)
            StartRoundhouse();

        if (Input.GetKeyDown(KeyCode.Q) &&
            !AnyAttack() &&
            Time.time >= nextUppercut)
            StartUppercut();

        if (Input.GetKeyDown(KeyCode.R) &&
            !AnyAttack() &&
            Time.time >= nextSpinKick)
            StartSpinKick();

        if (Input.GetKeyDown(KeyCode.F) &&
            !AnyAttack() &&
            Time.time >= nextShoulder)
            StartShoulder();

        if (punching)
            UpdatePunch();

        if (kicking)
            UpdateKick();

        if (roundhouseKicking)
            UpdateRoundhouse();

        if (uppercutting)
            UpdateUppercut();

        if (spinKicking)
            UpdateSpinKick();

        if (shoulderCharging)
            UpdateShoulder();

        KeepInsideRing();
    }

    bool AnyAttack()
    {
        return punching ||
               kicking ||
               roundhouseKicking ||
               uppercutting ||
               spinKicking ||
               shoulderCharging;
    }

    void LateUpdate()
    {
        if (kicking)
        {
            if (rightUpperLeg != null)
            {
                rightUpperLeg.rotation =
                    Quaternion.AngleAxis(
                        -105f * kickVisual,
                        transform.right) *
                    rightUpperLeg.rotation;
            }

            if (rightLowerLeg != null)
            {
                rightLowerLeg.rotation =
                    Quaternion.AngleAxis(
                        45f * kickVisual,
                        transform.right) *
                    rightLowerLeg.rotation;
            }
        }

        if (roundhouseKicking)
        {
            if (rightUpperLeg != null)
            {
                rightUpperLeg.rotation =
                    Quaternion.AngleAxis(
                        -80f * roundhouseVisual,
                        transform.right) *
                    rightUpperLeg.rotation;

                rightUpperLeg.rotation =
                    Quaternion.AngleAxis(
                        100f * roundhouseVisual,
                        transform.up) *
                    rightUpperLeg.rotation;
            }

            if (rightLowerLeg != null)
            {
                rightLowerLeg.rotation =
                    Quaternion.AngleAxis(
                        30f * roundhouseVisual,
                        transform.right) *
                    rightLowerLeg.rotation;
            }
        }

        if (uppercutting)
        {
            if (rightUpperArm != null)
            {
                rightUpperArm.rotation =
                    Quaternion.AngleAxis(
                        -120f * uppercutVisual,
                        transform.right) *
                    rightUpperArm.rotation;
            }

            if (rightLowerArm != null)
            {
                rightLowerArm.rotation =
                    Quaternion.AngleAxis(
                        -70f * uppercutVisual,
                        transform.right) *
                    rightLowerArm.rotation;
            }
        }
    }

    void MovePlayer()
    {
        float x = 0f;
        float z = 0f;

        if (Input.GetKey(KeyCode.A)) x = -1f;
        if (Input.GetKey(KeyCode.D)) x = 1f;
        if (Input.GetKey(KeyCode.W)) z = 1f;
        if (Input.GetKey(KeyCode.S)) z = -1f;

        Vector3 movement =
            new Vector3(x, 0f, z).normalized;

        transform.position +=
            movement *
            moveSpeed *
            Time.deltaTime;
    }

    void FaceOpponent()
    {
        if (opponent == null)
            return;

        Vector3 direction =
            opponent.position -
            transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
            transform.rotation =
                Quaternion.LookRotation(direction);
    }

    void MoveIntoContact()
    {
        if (opponent == null)
            return;

        Vector3 direction =
            opponent.position -
            transform.position;

        direction.y = 0f;

        float distance =
            direction.magnitude;

        if (distance > attackContactDistance)
        {
            transform.position +=
                direction.normalized *
                attackLungeSpeed *
                Time.deltaTime;

            KeepInsideRing();
        }
    }

    void StartPunch()
    {
        punching = true;
        damageDealt = false;
        punchTimer = 0f;

        nextPunch =
            Time.time + attackCooldown;

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
        MoveIntoContact();

        punchTimer += Time.deltaTime;

        float progress =
            Mathf.Clamp01(
                punchTimer /
                punchDuration);

        float extension =
            Mathf.Sin(progress * Mathf.PI);

        if (attackArm != null)
        {
            attackArm.localPosition =
                armStartPosition +
                Vector3.forward *
                extension *
                punchReach;
        }

        if (!damageDealt &&
            progress >= 0.5f)
        {
            DealDamage(
                damage,
                attackDistance,
                knockback);

            damageDealt = true;
        }

        if (progress >= 1f)
        {
            punching = false;

            ResetArm();

            if (fighterAnimator != null)
            {
                fighterAnimator.CrossFade(
                    "Base Layer.Idle",
                    0.1f);
            }
        }
    }

    void StartKick()
    {
        kicking = true;
        damageDealt = false;

        kickTimer = 0f;
        kickVisual = 0f;

        nextKick =
            Time.time + kickCooldown;
    }

    void UpdateKick()
    {
        MoveIntoContact();

        kickTimer += Time.deltaTime;

        float progress =
            Mathf.Clamp01(
                kickTimer /
                kickDuration);

        kickVisual =
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
            kicking = false;
            kickVisual = 0f;
            ResetLeg();
        }
    }

    void StartRoundhouse()
    {
        roundhouseKicking = true;
        damageDealt = false;

        roundhouseTimer = 0f;
        roundhouseVisual = 0f;

        nextRoundhouse =
            Time.time +
            roundhouseCooldown;
    }

    void UpdateRoundhouse()
    {
        MoveIntoContact();

        roundhouseTimer +=
            Time.deltaTime;

        float progress =
            Mathf.Clamp01(
                roundhouseTimer /
                roundhouseDuration);

        roundhouseVisual =
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
        {
            roundhouseKicking = false;
            roundhouseVisual = 0f;
        }
    }

    void StartUppercut()
    {
        uppercutting = true;
        damageDealt = false;

        uppercutTimer = 0f;
        uppercutVisual = 0f;

        nextUppercut =
            Time.time +
            uppercutCooldown;
    }

    void UpdateUppercut()
    {
        MoveIntoContact();

        uppercutTimer +=
            Time.deltaTime;

        float progress =
            Mathf.Clamp01(
                uppercutTimer /
                uppercutDuration);

        uppercutVisual =
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
            uppercutting = false;
            uppercutVisual = 0f;
            ResetArm();
        }
    }

    void StartSpinKick()
    {
        spinKicking = true;
        damageDealt = false;

        spinKickTimer = 0f;

        spinStartRotation =
            transform.rotation;

        nextSpinKick =
            Time.time +
            spinKickCooldown;
    }

    void UpdateSpinKick()
    {
        MoveIntoContact();

        spinKickTimer +=
            Time.deltaTime;

        float progress =
            Mathf.Clamp01(
                spinKickTimer /
                spinKickDuration);

        transform.rotation =
            spinStartRotation *
            Quaternion.Euler(
                0f,
                progress * 360f,
                0f);

        if (!damageDealt &&
            progress >= 0.45f)
        {
            DealDamage(
                spinKickDamage,
                spinKickDistance,
                spinKickKnockback);

            damageDealt = true;
        }

        if (progress >= 1f)
        {
            spinKicking = false;

            transform.rotation =
                spinStartRotation;

            ResetLeg();
        }
    }

    void StartShoulder()
    {
        shoulderCharging = true;
        damageDealt = false;

        shoulderTimer = 0f;

        nextShoulder =
            Time.time +
            shoulderCooldown;

        if (opponent != null)
        {
            shoulderDirection =
                opponent.position -
                transform.position;

            shoulderDirection.y = 0f;

            if (shoulderDirection.sqrMagnitude >
                0.001f)
            {
                shoulderDirection.Normalize();

                transform.rotation =
                    Quaternion.LookRotation(
                        shoulderDirection);
            }
        }
    }

    void UpdateShoulder()
    {
        shoulderTimer += Time.deltaTime;

        transform.position +=
            shoulderDirection *
            shoulderSpeed *
            Time.deltaTime;

        KeepInsideRing();

        if (!damageDealt &&
            opponent != null)
        {
            float distance =
                Vector3.Distance(
                    transform.position,
                    opponent.position);

            if (distance <= shoulderDistance)
            {
                DealDamage(
                    shoulderDamage,
                    shoulderDistance,
                    shoulderKnockback);

                damageDealt = true;
            }
        }

        if (shoulderTimer >=
            shoulderDuration)
        {
            shoulderCharging = false;
        }
    }

    void DealDamage(
        float amount,
        float requiredDistance,
        float attackKnockback)
    {
        if (opponent == null)
            return;

        float distance =
            Vector3.Distance(
                transform.position,
                opponent.position);

        if (distance > requiredDistance)
            return;

        FighterHealth health =
            opponent.GetComponent<FighterHealth>();

        if (health == null ||
            health.IsKnockedOut())
            return;

        health.TakeDamage(amount);

        Vector3 direction =
            opponent.position -
            transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            opponent.position +=
                direction.normalized *
                attackKnockback;

            ClampToCenter(opponent);
        }
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

    void ClampToCenter(Transform fighter)
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

        // Tight central fighting zone
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