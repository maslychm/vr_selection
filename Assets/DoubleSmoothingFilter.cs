using System.Collections.Generic;
using UnityEngine;

// applies smoothing to the ray kebab when checked
public class DoubleSmoothingFilter : MonoBehaviour
{
    // store the inital data point which should be the transform of this gameObject
    private Vector3 x_0;

    private Vector3 s_0, s_t;

    private Vector3 b_t, x_t, b_0;

    private Vector3 PredictedForecast; 

    // add tyhe smoothiung factor 
    [Range(0, 1)]
    [SerializeField] private float smoothingFactor = 0.3f;

    [Range(0, 1)]
    [SerializeField] private float dataSmoothingFactor = 0.3f;

    public bool applySmoothing = false;

    private void Start()
    {
        x_0 = this.gameObject.transform.position;

        s_0 = x_0;
    }

    private void Update()
    {

        // working withb vector 3 should mnake direct arithmetic operations
        x_t = this.gameObject.transform.position;
        s_0 = x_0;
        //b_0 = new Vector3(x_t.x - x_0.x, x_t.y - x_0.y, x_t.z - x_0.z);
        b_0 = x_t - x_0;

        s_t = smoothingFactor * x_t + (1 - smoothingFactor) * (s_0 + b_0);
        b_t = dataSmoothingFactor * (s_t - s_0) + b_0 * (1 - dataSmoothingFactor);
        if (applySmoothing == true)
            apply();

        x_0 = this.gameObject.transform.position;

    }

    private void apply()
    {

        PredictedForecast = s_t + (b_t * Time.deltaTime);

        this.gameObject.transform.position = s_t;

    }

}