using UnityEngine;
using UnityEngine.SceneManagement;

public class RefractionCompanion : Companion
{
    public LineRenderer mLineRenderer;
    public LayerMask mCollisionLayerMask;
    public float mMaxDistance = 200.0f;
    private bool mCreateRefraction;
    public AudioClip mLaserSound;

    void Update()
    {
        mLineRenderer.gameObject.SetActive(mCreateRefraction);
        mCreateRefraction = false;
    }

    public void CreateRefraction(Vector3 position)
    {
        mLineRenderer.material.color = Color.red;
        mCreateRefraction = true;
        Vector3 lEndRaycastPosition = Vector3.forward * mMaxDistance;
        RaycastHit lRaycastHit;
        if (Physics.Raycast(new Ray(mLineRenderer.transform.position, CalculateDestination(position)),
            out lRaycastHit, mMaxDistance,
            mCollisionLayerMask.value))
        {
            lEndRaycastPosition = lRaycastHit.point;
            switch (lRaycastHit.collider.tag)
            {
                case "Player":
                    SceneManager.LoadScene("GameOver");
                    break;
                case "Refraction Companion":
                    var lCompanion = lRaycastHit.collider.gameObject.GetComponent<RefractionCompanion>();
                    if (!lCompanion.mAttached)
                        lRaycastHit.collider.gameObject.GetComponent<RefractionCompanion>()
                            .CreateRefraction(lRaycastHit.point);
                    else
                        lRaycastHit.collider.gameObject.GetComponent<RefractionCompanion>()
                            .CreatePreviewRefraction(lRaycastHit.point);
                    break;
                case "Turret":
                    GameObject.Find("Player").GetComponent<Weapon>().DetachObject(0);
                    Destroy(lRaycastHit.collider.gameObject);
                    break;
                case "Button":
                    lRaycastHit.collider.gameObject.GetComponent<PortalButton>().PlayEvent();
                    break;
            }
        }

        mLineRenderer.SetPosition(0, mLineRenderer.transform.position);
        mLineRenderer.SetPosition(1, lEndRaycastPosition);
        if (!mLineRenderer.gameObject.activeInHierarchy)
            mAudioSource.Stop();
        else if (!mAudioSource.isPlaying)
        {
            mAudioSource.volume = .5f;
            mAudioSource.loop = true;
            mAudioSource.clip = mLaserSound;
            mAudioSource.Play();
        }
    }

    public void CreatePreviewRefraction(Vector3 position)
    {
        mCreateRefraction = true;
        mLineRenderer.material.color = Color.blue;
        Vector3 lEndRaycastPosition = Vector3.forward * mMaxDistance;
        RaycastHit lRaycastHit;
        if (Physics.Raycast(new Ray(mLineRenderer.transform.position, CalculateDestination(position)),
            out lRaycastHit, mMaxDistance,
            mCollisionLayerMask.value))
        {
            lEndRaycastPosition = lRaycastHit.point;
            switch (lRaycastHit.collider.tag)
            {
                case "Refraction Companion":
                    var lCompanion = lRaycastHit.collider.gameObject.GetComponent<RefractionCompanion>();
                    if (lCompanion.mAttached)
                        lRaycastHit.collider.gameObject.GetComponent<RefractionCompanion>()
                            .CreateRefraction(lRaycastHit.point);
                    else
                        lRaycastHit.collider.gameObject.GetComponent<RefractionCompanion>()
                            .CreatePreviewRefraction(lRaycastHit.point);
                    break;
                case "Turret":
                    GameObject.Find("Player").GetComponent<Weapon>().DetachObject(0);
                    Destroy(lRaycastHit.collider.gameObject);
                    break;
            }
        }

        mLineRenderer.SetPosition(0, mLineRenderer.transform.position);
        mLineRenderer.SetPosition(1, lEndRaycastPosition);
    }

    private Vector3 CalculateDestination(Vector3 position)
    {
        Collider lCollider = GetComponent<BoxCollider>();
        if (lCollider.bounds.center.x < position.x)
            return mLineRenderer.transform.forward;
        return -mLineRenderer.transform.forward;
    }
}