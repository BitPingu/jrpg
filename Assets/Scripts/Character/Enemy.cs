using System.Collections;
using UnityEngine;

public class Enemy : FighterBase
{
    public float SightRadius;
    public LayerMask playerLayer;
    private Vector2 _curDir;
    private float _moveCounter, _waitCounter, _waitTime;

    public void SetRandomDir()
    {
        // choose random direction
        _curDir.x = Random.Range(-1f, 1f);
        _curDir.y = Random.Range(-1f, 1f);

        // choose random wait time
        _waitTime = Random.Range(3f, 5f);
    }

    public void InterruptIdle()
    {
        // stop
        _curDir = Vector2.zero;
    }

    public override void Idle()
    {
        // call base class
        base.Idle();

        if (_moveCounter > 0)
        {
            // update move counter
            _moveCounter -= Time.deltaTime;

            // stop
            if (_moveCounter < 0.1f)
                _waitCounter = _waitTime;
        }
        else
        {
            // stop
            InterruptIdle();

            // update wait counter
            _waitCounter -= Time.deltaTime;

            if (_waitCounter < 0)
            {
                SetRandomDir();

                // reset move time
                _moveCounter = 1f;
            }
        }

        // allow enemy movement
        Move(_curDir);
    }

    public void Chase(CharacterBase opponent)
    {
        // chase opponent
        float distance = Vector2.Distance(opponent.transform.position, transform.position);
        if (distance > SightRadius)
        {
            Move(opponent.transform.position - transform.position);
        }
        else
        {
            Move(Vector2.zero);
        }
    }

    public override void Battle()
    {
        // call base class
        base.Battle();

        // attack
        if (BattleTurn)
        {
            StartCoroutine(CallAttack());
            BattleTurn = false;
        }
    }

    public override void Damage(int damageAmount)
    {
        // call base class
        base.Damage(damageAmount);

        if (CurrentHealth <= 0)
        {
            Debug.Log(name + " was defeated!");
            StartCoroutine(Die());            
        }
    }

    private IEnumerator Die()
    {
        // set color
        Sprite.material.SetFloat("_FlashAmount", 1f);
        float flashTime = 2f;

        float currentFlashAmount;
        float elapsedTime = 0f;
        while (elapsedTime < flashTime)
        {
            elapsedTime += Time.deltaTime;
            // lerp flash amount
            currentFlashAmount = Mathf.Lerp(0f, 1f, (elapsedTime / flashTime));
            // set flash amount
            Sprite.material.SetFloat("_Alpha", currentFlashAmount);
            yield return null;
        }

        // Die
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, SightRadius);
    }
}

