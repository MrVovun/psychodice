using UnityEngine;

public class TestImpulseMaker : MonoBehaviour
{
    public Die die;

    void Start()
    {
        if (die)
        {
            Rigidbody rb = die.GetComponent<Rigidbody>();
            var impulseSettings = die.impulseSettings;
            Vector3 finalForce = default;
            float forceDeviation = Random.Range(0, impulseSettings.forceRandomDeviation);
            if (impulseSettings.randomize)
            {
                Vector2 deviation = Random.insideUnitCircle * impulseSettings.randomDeviation;
                finalForce = (Vector3.forward + new Vector3(deviation.x, deviation.y, 0)) * (impulseSettings.force + forceDeviation);
            }
            else
            {
                finalForce = Vector3.forward * impulseSettings.force * forceDeviation;
            }

            float finalAngularForce = impulseSettings.angularForce + Random.Range(0, impulseSettings.angularForceRandomDeviation);
            finalForce = transform.TransformDirection(finalForce);

            rb.AddRelativeTorque(Random.insideUnitSphere * finalAngularForce, impulseSettings.forceMode);
            rb.AddForce(finalForce, impulseSettings.forceMode);
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 p = transform.position;
        Vector3 f = transform.forward * 0.2f;
        Gizmos.color = Color.green;
        Gizmos.DrawLine(p, p + f);
    }
}
