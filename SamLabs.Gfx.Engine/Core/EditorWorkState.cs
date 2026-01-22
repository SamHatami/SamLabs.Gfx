namespace SamLabs.Gfx.Engine.Core;

public class EditorWorkState
{
    private readonly object _lock = new();
    private DateTime _burstEndTime = DateTime.MinValue;
    private bool _isContinuousUpdateRequested;
    
    public void RequestContinuousUpdate()
    {
        lock (_lock)
        {
            _isContinuousUpdateRequested = true;
        }
    }
    
    public void StopContinuousUpdate()
    {
        lock (_lock)
        {
            _isContinuousUpdateRequested = false;
        }
    }
    
    public void RequestBurst(float milliseconds = 150)
    {
        lock (_lock)
        {
            var requestedEndTime = DateTime.Now.AddMilliseconds(milliseconds);
            if (requestedEndTime > _burstEndTime)
            {
                _burstEndTime = requestedEndTime;
            }
        }
    }
    
    public bool ShouldUpdate()
    {
        lock (_lock)
        {
            if (_isContinuousUpdateRequested) return true;
            if (DateTime.Now < _burstEndTime) return true;
            return false;
        }
    }
    
    
    public void Reset()
    {
        lock (_lock)
        {
            _isContinuousUpdateRequested = false;
            _burstEndTime = DateTime.MinValue;
        }
    }
}