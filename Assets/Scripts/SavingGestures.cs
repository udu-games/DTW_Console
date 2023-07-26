using UnityEngine;
using UnityEngine.UI;

public class SavingGestures : MonoBehaviour
{
    // 1. We have all the console data (orientation, accleration, etc).
    // 2. We need to normalize the console data. (Might be issues with 3D space positioning, similar fix to project *snowball rotation)
    // 3. We need to incorporate the normalized console data into the DTW formula script. (X, Y axis *maybe Z?*)

    // 4. To test and compare gestures we need:
    //      - button press - start recognition.
    //      - console movement,swing,etc - console data values are populating the DTW arrays.
    //      - button release - end of the recognition.

    // 5. We need to calculate the (query gesture) to the set of (reference gestures) using DTW distance to determine the sum of the array.
    //      - If the (query gesture) is similar to a (reference gesture) then we know the gesture already and can play that.
    //      - else if the (query gesture) is NOT similar to a (reference gesture) then we can save the query gesture to the list of (reference gestures).


    // private variables
    private AbstractDataStream abstractDataStream;

    private Quaternion consoleOrientation;

    private Vector3 consoleAcceleration;

    private bool isTriggerButtonPressed = false;

    private bool isValueAboveThreshold = false;

    private float normalizeInputValue;
    private int revolutionCount = 0;
    private int previousRevolutionCount = 0;

    public float convTest;
    public float fakeConsoleQuatData;

    // public variables
    public Text consoleDataTxt;

    public Quaternion quatAngle;

    //
    private void Start()
    {
        abstractDataStream = FindObjectOfType<BLEDataStream>();


        EventsSystemHandler.Instance.onTriggerPressTriggerButton += ConsoleTriggerButtonPress;
        EventsSystemHandler.Instance.onTriggerReleaseTriggerButton += ConsoleTriggerButtonRelease;
    }

    //
    private void Update()
    {
        SetConsoleDataText();
        StartGestureRecognise();

        DataTest();
    }

    private void DataTest()
    {
        Quaternion quat = quatAngle;

        //Matrix4x4 rotMatrix = Matrix4x4.Rotate(quat);
        //
        //float pitch = Mathf.Atan2(rotMatrix.m21, rotMatrix.m22); // Rotation around X-axis
        //float yaw = Mathf.Asin(-rotMatrix.m20); // Rotation around Y-axis
        //float roll = Mathf.Atan2(rotMatrix.m10, rotMatrix.m00); // Rotation around Z-axis

        //pitch *= Mathf.Rad2Deg;
        //yaw *= Mathf.Rad2Deg;
        //roll *= Mathf.Rad2Deg;

        //convTest = ConvertYawToCurve(yaw, 0.0f);

        //convTest = NormalizeValue(fakeConsoleQuatData);

        //Debug.Log("conv : " + conv);
    }

    private float NormalizeValue(float inputValue)
    {
        float maxValue = 1.0f;

        //inputValue = (inputValue > 180.0f) ? inputValue - 360.0f : inputValue;

        normalizeInputValue = inputValue / 360.0f;

        // TODO: When value reaches threshold ( -1.0f, 1.0f ) return value back to 0, as the console does a full revolution.


        //if (Mathf.Abs(convTest) >= maxValue)
        //{
        //    isValueAboveThreshold = true;

        //    if (isValueAboveThreshold)
        //    {
        //        Debug.Log("normalizeInputValue greater then threshold" + normalizeInputValue);
        //        normalizeInputValue = 0f;
        //        isValueAboveThreshold = false;
        //    }
        //}



        float normalizedValueClamped = Mathf.Clamp(normalizeInputValue, -1.0f, maxValue);

        return normalizedValueClamped;
    }

    float ConvertYawToCurve(float yaw, float tiltThreshold)
    {
        // The target range for the curve, with 0 being no curve and ±0.2 being maximum curve in each direction.
        float targetMin = 0f;
        float targetMax = 1f;

        // Clamp the yaw value between -70 and 70.
        yaw = Mathf.Clamp(yaw, 0f, 360f);

        // Calculate the absolute yaw value
        float absYaw = Mathf.Abs(yaw);

        // If the absolute yaw is less than the threshold, no curve is applied.
        if (absYaw < tiltThreshold)
        {
            return 0f;
        }

        // Calculate the normalized value between 0 and 1 based on the yaw.
        float normalizedValue = (absYaw - tiltThreshold) / (360f - tiltThreshold);

        // Calculate the curve value.
        float curveValue = targetMax * normalizedValue;

        // If the original yaw was negative, make the curve negative.
        if (yaw < 0)
        {
            curveValue *= -1;
        }

        return curveValue;
    }

    // 
    private void StartGestureRecognise()
    {
        // if trigger button is pressed ...
        if (isTriggerButtonPressed)
        {

        }
    }

    // display the console data *text UI
    private void SetConsoleDataText()
    {
        consoleOrientation = abstractDataStream.GetOrientation().normalized;

        consoleAcceleration = abstractDataStream.GetAcceleration().normalized;

        consoleDataTxt.text =
            $"Button: {isTriggerButtonPressed}!" +
            $"\n\n" +
            $"Orientation: {consoleOrientation}!" +
            $"\n\n" +
            $"Acceleration: {consoleAcceleration}!";
        //$"convTest: {convTest}!";
    }

    // subscribe to button trigger event , apply bool
    #region Button Press Event
    private void ConsoleTriggerButtonPress()
    {
        isTriggerButtonPressed = true;
    }
    // subscribe to button trigger event , apply bool
    private void ConsoleTriggerButtonRelease()
    {
        isTriggerButtonPressed = false;
    }
    #endregion
}