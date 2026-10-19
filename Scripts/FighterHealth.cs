using UnityEngine;
using TMPro;

public class FighterHealth : MonoBehaviour
{
    public float health = 100f;
    public TMP_Text healthText;
    public TMP_Text winnerText;

    private bool knockedOut = false;

    void Start()
    {
        health = 100f;
        knockedOut = false;

        UpdateHealthText();

        if (winnerText != null)
        {
            winnerText.text = "";
        }
    }

    public void TakeDamage(float damage)
    {
        if (knockedOut)
            return;

        health -= damage;

        if (health < 0)
            health = 0;

        UpdateHealthText();

        Debug.Log(gameObject.name + " Health: " + health);

        if (health <= 0)
        {
            KnockOut();
        }
    }

    void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text =
                gameObject.name + " HP: " + health;
        }
    }

    void KnockOut()
    {
        knockedOut = true;

        // Fall sideways
        transform.rotation *= Quaternion.Euler(0, 0, 90);

        // Stop this wrestler from fighting
        SimpleFighter fighter = GetComponent<SimpleFighter>();


        if (fighter != null)
        {
            fighter.enabled = false;
        }

        PlayerFighter playerFighter = GetComponent<PlayerFighter>();

        if (playerFighter != null)
        {
            playerFighter.enabled = false;
        }

        if (winnerText != null)
        {
            if (gameObject.name == "Wrestler 1")
            {
                winnerText.text = "WRESTLER 2 WINS!";
            }
            else
            {
                winnerText.text = "WRESTLER 1 WINS!";
            }
        }

        // Display winner
        

        Debug.Log(gameObject.name + " has been knocked out!");
    }

    public void ResetFighter()
{
    health = 100f;
    knockedOut = false;

    // Stand the wrestler back up
    transform.rotation = Quaternion.identity;

    // Re-enable fighting
    SimpleFighter fighter = GetComponent<SimpleFighter>();
    PlayerFighter playerFighter = GetComponent<PlayerFighter>();


    if (playerFighter != null)
    {
        playerFighter.enabled = true;
    }

    if (fighter != null)
    {
        fighter.enabled = true;
    }

   

    UpdateHealthText();

    if (winnerText != null)
    {
        winnerText.text = "";
    }
}
    public bool IsKnockedOut()
    {
        return knockedOut;
    }
}