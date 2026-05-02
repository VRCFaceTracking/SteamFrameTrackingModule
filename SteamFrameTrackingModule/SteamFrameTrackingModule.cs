using Microsoft.Extensions.Logging;
using Valve.VR;
using VRCFaceTracking;

namespace SteamFrameTrackingModule;

public class SteamFrameTrackingModule : ExtTrackingModule
{
    public override (bool SupportsEye, bool SupportsExpression) Supported => (true, false);

    private CVRSystem _system;
    private HmdVector2_t _pNdcLef;
    private HmdVector2_t _pNdcRight;
    
    public override (bool eyeSuccess, bool expressionSuccess) Initialize(bool eyeAvailable, bool expressionAvailable)
    {
        ModuleInformation.Name = "Steam Frame Module";

        EVRInitError initError = EVRInitError.None;
        _system = OpenVR.Init(ref initError, EVRApplicationType.VRApplication_Background);
        if (initError != EVRInitError.None)
        {
            Logger.LogError($"Failed to initialize Steam Frame Module: {initError}");
            return (false, false);
        }
        
        return (true, false);
    }

    public override void Update()
    {
        var result = _system.GetEyeTrackedFoveationCenter(ref _pNdcLef, ref _pNdcRight);
        if (!result) return;

        UnifiedTracking.Data.Eye.Left.Gaze.x = _pNdcLef.v0;
        UnifiedTracking.Data.Eye.Left.Gaze.y = _pNdcLef.v1;
        UnifiedTracking.Data.Eye.Right.Gaze.x = _pNdcRight.v0;
        UnifiedTracking.Data.Eye.Right.Gaze.y = _pNdcRight.v1;
    }

    public override void Teardown()
    {
        OpenVR.Shutdown();
    }
}
