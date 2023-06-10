using System;

public static class EventBusUI
{
    public static Action<float, float> OnHealthUpdated;
    public static Action<float> OnExpUpdated;
    public static Action<float> OnLevelUpdated;
}
