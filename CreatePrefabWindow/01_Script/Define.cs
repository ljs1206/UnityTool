using System;

public static class Define{
    [Flags]
    public enum ViewSetting{
        None ,View2D, View3D
    }
    [Flags]
    public enum AIType{
        None, FSM, BT
    }
    [Flags]
    public enum ColliderType2D{
        None, Box, Circle, Capsule
    }
    [Flags]
    public enum ColliderType3D{
        None, Box, Sphere, Capsule
    }
}