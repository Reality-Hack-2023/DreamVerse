using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AffectiveMetrics : MonoBehaviour
{
    private double enjoyment { get; set;}
    private double enjoymentDelta { get; set;}
    private double focus { get; set;}
    private double focusDelta { get; set;}
    private GameObject shaderObject;
    private GameObject shaderObject2;
    private GameObject shaderObject3;
    private GameObject shaderObject4;
    private GameObject shaderObject5;
    private GameObject shaderObject6;

    private GameObject positionObject;

    private GameObject playerObject;
    private GameObject playerObject2;

    double newEnjoyment, newFocus;

    private float time = 0.0f;
    private const float refreshRate = 1f;

    [Range(0.0f, 1.0f)]
    public float testVar;
    [Range(0.0f, 1.0f)]
    public float testVar2;
    //[232, 115, 207] pink
    Color joyMaxFocMax = new Color(0.9059f, 0.4509f, 0.8118f, 0.0f);
    //[84, 226, 242] blue
    Color joyMinFocMax = new Color(0.3294f, 0.8863f, 0.9490f, 0.0f);
    //[255, 234, 0] yellow
    Color joyMaxFocMin = new Color(1.0f, 0.9176f, 0.0f, 0.0f);
    //[180, 236, 60] green
    Color joyMinFocMin = new Color(0.7059f, 0.9255f, 0.2353f, 0.0f);

    private Dictionary<string, string> fields = new Dictionary<string, string>();

    Color Blerp(Color c00, Color c10, Color c01, Color c11, float tx, float ty)
    {
        return Color.Lerp(
            Color.Lerp(c00, c10, tx),
            Color.Lerp(c01, c11, tx),
            ty);
    }

    Color PaletteBlerp(float joy, float foc)
    {
        return Blerp(joyMinFocMin, joyMaxFocMin, joyMinFocMax, joyMaxFocMax, joy, foc);
    }

    // Start is called before the first frame update
    void Start()
    {
        enjoyment = 0.5;
        enjoymentDelta = 0;
        focus = 0.5;
        focusDelta = 0;
        
        // Change this to all objects that need to be affected by the affective metrics
        // shaderObject = GameObject.Find("Sphere");
        shaderObject = GameObject.Find("Triangle-tonel");
        shaderObject2 = GameObject.Find("cube-tonel");
        shaderObject3 = GameObject.Find("plane-tonel");
        shaderObject4 = GameObject.Find("plane-tonel2");
        shaderObject5 = GameObject.Find("5sides-tonel");
        shaderObject6 = GameObject.Find("7sides-tonel");
        positionObject = GameObject.Find("XR Origin");
        playerObject = GameObject.Find("Player-Cylinder");
        playerObject2 = GameObject.Find("Player-Sphere");

        // vfxObject
    }

    // Expected values in 0-1 range
    float ValueTransformer(float value, float delta) {
        value = value * 2f - 1f;
        float sign = Mathf.Sign(value);
        float abs = Mathf.Abs(value);
        
        value = (sign * Mathf.Pow(abs, 0.20f));

        value = value + Mathf.Sign(delta + Random.Range(-0.2f, 0.2f)) * 0.3f;
        
        value = value + Random.Range(-0.2f, 0.2f);

        value = value / 2f + 0.5f;

        value = Mathf.Min( Mathf.Max(0f, value), 1f);

        return value;
    }

    float ValueTransformer(float value) {
        value = value * 2f - 1f;
        float sign = Mathf.Sign(value);
        float abs = Mathf.Abs(value);
        
        value = (sign * Mathf.Pow(abs, 0.20f));
        
        value = value + Random.Range(-0.2f, 0.2f);

        value = value / 2f + 0.5f;

        value = Mathf.Min( Mathf.Max(0f, value), 1f);

        return value;
    }

    float sigmoid(float x) {
        return 1 / (1 + Mathf.Exp(-x));
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        fields = NeuosController.GetFields();
        
        if(fields.ContainsKey("enjoyment")) {
            newEnjoyment = double.Parse(fields["enjoyment"]);
        }
        if(fields.ContainsKey("focus")) {
            newFocus = double.Parse(fields["focus"]);
        }

        // Debug.Log("Enjoyment: " + fields["enjoyment"] + " Focus: " + fields["focus"]);

        if(newEnjoyment == -1) {
            newEnjoyment = enjoyment;
        }
        if(newFocus == -1) {
            newFocus = focus;
        }
        enjoymentDelta = enjoyment - newEnjoyment;
        enjoyment = newEnjoyment;
        focusDelta = focus - newFocus;
        focus = newFocus;

        //Color transformations
        float joyTransformed = ValueTransformer((float) enjoyment / 100f, (float) enjoymentDelta);
        float focTransformed = ValueTransformer((float) focus / 100f, (float) focusDelta);

        //When not using BCI
        // joyTransformed = testVar;
        // focTransformed = testVar2;

        // Debug.Log("Enjoyment: " + enjoyment + " Focus: " + focus);
        Debug.Log("Joy: " + joyTransformed + " Foc: " + focTransformed);

        // Color objectColor = PaletteBlerp((float)testVar, (float)testVar2);
        Color objectColor2D = PaletteBlerp(joyTransformed, focTransformed);
        Color objectColorJoy = Color.Lerp(joyMinFocMax, joyMaxFocMax, joyTransformed);
        Color objectColorFoc = Color.Lerp(joyMinFocMin, joyMaxFocMin, focTransformed);

        Debug.Log(objectColorJoy);

        // APPLY COLORS HERE
        if(time >= refreshRate) {
            time = 0.0f;
            Color playerObjectColor = PaletteBlerp(Random.Range(0f, 1f), Random.Range(0f, 1f));
            playerObjectColor.a = 1f;
            playerObject.GetComponent<Renderer>().material.SetColor("_BaseColor", playerObjectColor);
            playerObject2.GetComponent<Renderer>().material.SetColor("_BaseColor", playerObjectColor);
        }

        if(positionObject.transform.position.z < 75) {
        //Triangle tonel
        shaderObject.GetComponent<Renderer>().material.SetColor("Color_f27694d2b9a04aa799589620b1a39151", objectColorFoc);
        shaderObject.GetComponent<Renderer>().material.SetColor("Color_93ffa2c63bd74bb3bfb1272d80977706", objectColorJoy);
        } else if(positionObject.transform.position.z < 160) {
        // //cube tonel
        shaderObject2.GetComponent<Renderer>().material.SetColor("Color_9b414519946d49bc845c9dc7d45d12d1", objectColorFoc);
        shaderObject2.GetComponent<Renderer>().material.SetColor("Color_a08ad18c38f94c46804da0a0e0957000", objectColorJoy);
        } else if(positionObject.transform.position.z < 240) {
        // //plane tonel
        shaderObject3.GetComponent<Renderer>().material.SetColor("Color_3acf6ca6c5a945738836c8c7a9af6021", objectColorFoc);
        shaderObject3.GetComponent<Renderer>().material.SetColor("Color_9ea6dc98a8ec4f718ef4d94bc1865bce", objectColorJoy);
        } else if(positionObject.transform.position.z < 320) {
        // //plane tonel2
        shaderObject4.GetComponent<Renderer>().material.SetColor("Color_72f9eb13d3924520899f633289634db1", objectColorFoc);
        shaderObject4.GetComponent<Renderer>().material.SetColor("Color_3409cc49f83148ffbeae96e8fa2939ec", objectColorJoy);
        } else if(positionObject.transform.position.z < 430) {
        // //5sides tonel
        shaderObject5.GetComponent<Renderer>().material.SetColor("Color_b43e5a5c4f0a4f96aa888632f781e6dd", objectColorFoc);
        shaderObject5.GetComponent<Renderer>().material.SetColor("Color_9a33c7e78ea241f59219335f65e398f2", objectColorJoy);
        } else {
        // //7sides tonel
        shaderObject6.GetComponent<Renderer>().material.SetColor("Color_b0ca6ee0c6164e1cb90470777c6db7ca", objectColorFoc);
        shaderObject6.GetComponent<Renderer>().material.SetColor("Color_304dea80045b490d87293b6cd8fa4a83", objectColorJoy);
        }
        // shaderObject.GetComponent<Renderer>().material.SetColor("_BaseColor", objectColor);
    }
}
