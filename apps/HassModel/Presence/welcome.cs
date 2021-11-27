/// <summary>
///     Greets (or insults) people when coming home :)
/// </summary>
public class WelcomeHomeManager
{

    #region -- Config properties --

    /// <summary>
    ///     Used to search the correct device trackers by naming
    /// </summary>
    public string? PresenceCriteria { get; set; }

    public MediaPlayerEntity? HallwayMediaPlayer { get; set; }

    public BinarySensorEntity? DoorSensor { get; set; }

    public IEnumerable<string>? Greetings { get; set; }

    #endregion

    Dictionary<string, DateTime> _lastTimeGreeted = new Dictionary<string, DateTime>(5);
    Random _randomizer = new Random();

    private readonly IHaContext _ha;
    private readonly Entities _entities;
    private readonly Services _services;
    private readonly ILogger<WelcomeHomeManager> _log;
    private readonly INetDaemonScheduler _scheduler;

    private readonly ITextToSpeechService _tts;
    public WelcomeHomeManager(IHaContext ha, ILogger<WelcomeHomeManager> logger, INetDaemonScheduler scheduler, ITextToSpeechService tts)
    {
        _ha = ha;
        _entities = new Entities(ha);
        _services = new Services(ha);
        _log = logger;
        _scheduler = scheduler;
        _tts = tts;

        DoorSensor?
            .StateChanges()
            .Where(e => e.New.IsOn())
            .Subscribe(s => GreetIfJustArrived(s.New?.EntityId));

        _ha.StateChanges()
            .Where(
                e => e.New?.EntityId.EndsWith(PresenceCriteria!) ?? false &&
                     e.New?.State == "Nyss anlänt")
            .Subscribe(s => GreetIfJustArrived(s.New?.EntityId));

    }
    private void GreetIfJustArrived(string? entityId)
    {
        if (entityId?.StartsWith("binary_sensor.") ?? false)
        {
            // The door opened, lets check if someone just arrived
            var trackerJustArrived = _ha.GetAllEntities()
                .Where(n => n.EntityId.EndsWith(PresenceCriteria!) && n.State == "Nyss anlänt");
            foreach (var tracker in trackerJustArrived)
            {
                Greet(tracker.EntityId);
            }
        }
        else if (entityId?.StartsWith("device_tracker.") ?? false)
        {
            if (DoorSensor.IsOn())
            {
                // Door is open, greet
                Greet(entityId);
            }
            else if (DoorSensor.IsOff())
            {
                // It is closed, lets check if it was recently opened
                if (DateTime.Now.Subtract(DoorSensor?.EntityState?.LastChanged ?? DateTime.MinValue) <= TimeSpan.FromMinutes(5))
                {
                    // It was recently opened, probably when someone got home
                    Greet(entityId);
                }
            }
        }
    }

    private void Greet(string tracker)
    {
        // Get the name from tracker i.e. device_tracer.name_presense
        var nameOfPerson = tracker[15..^PresenceCriteria!.Length];

        if (!OkToGreet(nameOfPerson))
            return;                     // We can not greet person just yet

        _tts.Speak(HallwayMediaPlayer?.EntityId!, GetGreeting(nameOfPerson));
    }

    private bool OkToGreet(string nameOfPersion)
    {
        if (_lastTimeGreeted.ContainsKey(nameOfPersion) == false)
        {
            _lastTimeGreeted[nameOfPersion] = DateTime.Now;
            return true;
        }

        if (DateTime.Now.Subtract(_lastTimeGreeted[nameOfPersion]) < TimeSpan.FromMinutes(15))
            return false; // To early to greet again

        _lastTimeGreeted[nameOfPersion] = DateTime.Now;
        return true; // It is ok to greet now
    }

    private string GetGreeting(string name)
    {
        var randomMessageIndex = _randomizer.Next(0, Greetings!.Count() - 1);
        return Greetings!.ElementAt(randomMessageIndex).Replace("{namn}", name);
    }
}