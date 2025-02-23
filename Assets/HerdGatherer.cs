using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class HerdGatherer : MonoBehaviour
{
    [Tooltip("Radius within which to signal nearby units to follow.")]
    public float signalRadius = 20f;
    [Tooltip("Layer on which ChozoAI units reside.")]
    public LayerMask chozoLayer;
    [Tooltip("Key that, when held, makes nearby units follow the player.")]
    public KeyCode followKey = KeyCode.LeftControl;
    public KeyCode dropKey = KeyCode.LeftControl;
    public List<ChozosAI> GatheredHerd;
    public CanvasGroup Ring;
    public Action OnHerdStarted;
    public Action OnHerdEnded;
    public Candy Candy;

    void Update()
    {
        // When the follow key is held down...
        if (Input.GetKeyDown(followKey))
        {
            OnHerdStarted?.Invoke();
            ShowRing();
            Candy.Show();
        }
        if (Input.GetKey(followKey))
        {
            // Find all ChozoAI units within the signal radius.
            Collider[] colliders = Physics.OverlapSphere(transform.position, signalRadius, chozoLayer);
            foreach (Collider col in colliders)
            {
                ChozosAI chozo = col.GetComponent<ChozosAI>();
                if (chozo != null && !GatheredHerd.Contains(chozo))
                {
                    chozo.SetFollowTarget(transform);
                    GatheredHerd.Add(chozo);
                }
            }
        }
        if (Input.GetKeyUp(followKey))
        {
            HideRing();
        }
        if (Input.GetKeyDown(dropKey))
        {
            OnHerdEnded?.Invoke();
            Candy.Hide();
            HideRing();
            foreach (ChozosAI chozo in GatheredHerd)
            {
                chozo.ClearFollowTarget();
            }
            GatheredHerd.Clear();
        }
    }

    public void HideRing()
    {
        Ring.DOFade(0f, 0.4f);
    }

    public void ShowRing()
    {
        Ring.DOFade(1f, 0.4f);
    }

    // Optional: visualize the signal radius in the editor.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, signalRadius);
    }
}
