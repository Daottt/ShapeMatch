using System;

public class EventBus
{
    private EventBus() { }

    private static EventBus instance;
    public static EventBus Instance
    {
        get
        {
            if (instance == null)
                instance = new EventBus();
            return instance;
        }
    }

    public Action GameStarted;

    public Action GameEnded;

    public Action<Shape> ShapeSelected;

}
