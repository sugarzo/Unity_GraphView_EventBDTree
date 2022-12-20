using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Events management system.
/// </summary>
public class EventManager
{

    #region " VARIABLES "

    private static Dictionary<string, UnityEvent> eventDictionary = new Dictionary<string, UnityEvent>();
    private static Dictionary<string, object> sender = new Dictionary<string, object>();
    private static Dictionary<string, bool> paused = new Dictionary<string, bool>();

    // The memory storage containing the data passed with an emitted event.
    private static Dictionary<string, object> storage = new Dictionary<string, object>();

    // The memory storage used by the DataGroup methods.
    private static Dictionary<string, object[]> storage2 = new Dictionary<string, object[]>();

    // The memory storage used by the IndexedDataGroup methods.
    private static Dictionary<string, DataGroup[]> storage3 = new Dictionary<string, DataGroup[]>();

    // Manage unique IDs associated to a callBack function, so as to use IDs as reference instead of callBacks.
    private static Dictionary<string, UnityAction> callBacks = new Dictionary<string, UnityAction>();

    private struct SFilter
    {
        public string value;
        public bool starts;
        public bool ends;
        public bool contains;
        public bool exact;
    }

    #endregion

    #region " START LISTENING "

    /// <summary>
    /// Starts the listening to an event with the given name. If that event is detected, the callBack function is executed. Optionally, you can define a unique ID in the callBackID paramenter.
    /// </summary>
    public static void StartListening(string eventName, UnityAction callBack, string callBackID = "")
    {
        if (eventDictionary.TryGetValue(eventName, out UnityEvent thisEvent))
        {
            thisEvent.AddListener(callBack);
        }
        else
        {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(callBack);
            eventDictionary.Add(eventName, thisEvent);
            paused.Add(eventName, false);
        }

        if (callBackID != "") callBacks.Add(eventName + "_" + callBackID, callBack);
    }

    /// <summary>
    /// Starts the listening to an event with the given name and enabling the use of filters on events emission. If that event is detected, and optional filters are satisfied, the callBack function is executed. Optionally, you can define a unique ID in the callBackID paramenter.
    /// </summary>
    public static void StartListening(string eventName, GameObject target, UnityAction callBack, string callBackID = "")
    {
        if (target == null)
        {
            Debug.LogError("The specified target is not a valid GameObject.");
            return;
        }

        StartListening(eventName, callBack);
        if (callBackID != "") callBacks.Add(eventName + "_" + callBackID, callBack);

        string newName = eventName + "__##name##" + target.name + "##" + "__##tag##" + target.tag + "##" + "__##layer##" + target.layer + "##";
        StartListening(newName, callBack);
        if (callBackID != "") callBacks.Add(eventName + "_" + callBackID + "_EXTRA", callBack);

    }

    #endregion

    #region " STOP LISTENING "

    /// <summary>
    /// Stop listening to the event with the given name and the memory occupied by this event is cleared. The callBack function must be saved by setting 'callBackID' parameter with a unique ID in the StartListening() method.
    /// </summary>
    public static void StopListening(string eventName, string callBackID)
    {
        if (eventDictionary.TryGetValue(eventName, out UnityEvent thisEvent))
        {
            if (callBackID != "")
            {
                if (callBacks.ContainsKey(eventName + "_" + callBackID))
                {
                    thisEvent.RemoveListener(callBacks[eventName + "_" + callBackID]);
                }
                if (callBacks.ContainsKey(eventName + "_" + callBackID + "_EXTRA"))
                {
                    thisEvent.RemoveListener(callBacks[eventName + "_" + callBackID + "_EXTRA"]);
                }
            }
        }
    }


    /// <summary>
    /// Stop listening to the event with the given name and the memory occupied by this event is cleared. The callBack function must be specified.
    /// </summary>
    public static void StopListening(string eventName, UnityAction callBack)
    {
        if (eventDictionary.TryGetValue(eventName, out UnityEvent thisEvent))
        {
            thisEvent.RemoveListener(callBack);
        }
    }

    #endregion

    #region " EMIT EVENT "

    /// <summary>
    /// Emit an event with the given name.
    /// </summary>
    /// <param name="eventName"></param>
    public static void EmitEvent(string eventName)
    {
        if (isPaused(eventName)) return;

        if (eventDictionary.TryGetValue(eventName, out UnityEvent thisEvent))
        {
            thisEvent.Invoke();
        }
    }

    /// <summary>
    /// Emit an event with the given name and save the sender.
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="sender"></param>
    public static void EmitEvent(string eventName, object sender)
    {
        if (isPaused(eventName)) return;

        if (EventManager.sender.ContainsKey(eventName)) EventManager.sender[eventName] = sender; else EventManager.sender.Add(eventName, sender);

        EmitEvent(eventName);
    }

    /// <summary>
    /// Emit the event with the given name after the specified delay seconds.
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="delay"></param>
    public static void EmitEvent(string eventName, float delay)
    {
        if (isPaused(eventName)) return;

        if (eventDictionary.TryGetValue(eventName, out UnityEvent thisEvent))
        {
            if (delay <= 0)
            {
                thisEvent.Invoke();
            }
            else
            {
                int d = (int)(delay * 1000);
                DelayedInvoke(thisEvent, d);
            }
        }
    }

    /// <summary>
    /// Emit the event with the given name after the specified delay seconds and save the sender.
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="delay"></param>
    /// <param name="sender"></param>
    public static void EmitEvent(string eventName, float delay, object sender)
    {
        if (isPaused(eventName)) return;

        if (EventManager.sender.ContainsKey(eventName)) EventManager.sender[eventName] = sender; else EventManager.sender.Add(eventName, sender);

        EmitEvent(eventName, delay);
    }

    /// <summary>
    /// Emit the event with the given name to listeners selected by the specified filter. Optionally, a delay and a sender can be specified.
    /// </summary>
    public static void EmitEvent(string eventName, string filter, float delay = 0f, object sender = null)
    {
        if (sender != null)
        {
            if (EventManager.sender.ContainsKey(eventName)) EventManager.sender[eventName] = sender; else EventManager.sender.Add(eventName, sender);
        }

        // Extract filter data.
        var data = filter.Split(';');
        var filters = new Dictionary<string, SFilter>();

        foreach (string s in data)
        {
            var tmp = s.Split(':');
            if (tmp[0] == "name") filters.Add("name", new SFilter { value = tmp[1].Replace("*", ""), contains = tmp[1].StartsWith("*") && tmp[1].EndsWith("*"), starts = tmp[1].EndsWith("*"), ends = tmp[1].StartsWith("*"), exact = !tmp[1].Contains("*") });
            if (tmp[0] == "tag") filters.Add("tag", new SFilter { value = tmp[1].Replace("*", ""), contains = tmp[1].StartsWith("*") && tmp[1].EndsWith("*"), starts = tmp[1].EndsWith("*"), ends = tmp[1].StartsWith("*"), exact = !tmp[1].Contains("*") });
            if (tmp[0] == "layer") filters.Add("layer", new SFilter { value = tmp[1].Replace("*", ""), contains = tmp[1].StartsWith("*") && tmp[1].EndsWith("*"), starts = tmp[1].EndsWith("*"), ends = tmp[1].StartsWith("*"), exact = !tmp[1].Contains("*") });
        }

        int counter = filters.Count;
        int found = 0;

        // Search for valid events to emit.

        foreach (KeyValuePair<string, UnityEvent> evnt in eventDictionary)
        {
            var key = evnt.Key;

            if (key.Contains("_") && key.StartsWith(eventName))
            {
                data = key.Split('_');

                var name = "";
                var tag = "";
                var layer = "";

                found = 0;

                foreach (string s in data)
                {
                    if (s.Contains("##name##")) name = s.Replace("##name##", "").Replace("#", "");
                    if (s.Contains("##tag##")) tag = s.Replace("##tag##", "").Replace("#", "");
                    if (s.Contains("##layer##")) layer = s.Replace("##layer##", "").Replace("#", "");
                }

                if (filters.ContainsKey("name") && name != "")
                {
                    if (FilterIsValidated(name, filters["name"])) found++;
                }
                if (filters.ContainsKey("tag") && tag != "")
                {
                    if (FilterIsValidated(tag, filters["tag"])) found++;
                }
                if (filters.ContainsKey("layer") && layer != "")
                {
                    if (FilterIsValidated(layer, filters["layer"])) found++;
                }

                if (found == counter) { EmitEvent(key, delay); }
            }

        }

    }

    private static bool FilterIsValidated(string value, SFilter rules)
    {
        if (rules.exact)
        {
            return value == rules.value;
        }
        else if (rules.contains)
        {
            return value.Contains(rules.value);
        }
        else if (rules.starts)
        {
            return value.StartsWith(rules.value);
        }
        else if (rules.ends)
        {
            return value.EndsWith(rules.value);
        }
        return false;
    }

    /// <summary>
    /// Emit an event with the given name and data (optionally, with a delay).
    /// </summary>
    public static void EmitEventData(string eventName, object data, float delay = 0f)
    {
        SetData(eventName, data);
        EmitEvent(eventName, delay);
    }

    #endregion

    #region " UTILS METHODS "

    /// <summary>
    /// Stop all the listeners.
    /// </summary>
    public static void StopAll()
    {
        foreach (KeyValuePair<string, UnityEvent> evnt in eventDictionary)
        {
            evnt.Value.RemoveAllListeners();
        }
        eventDictionary = new Dictionary<string, UnityEvent>();
    }

    private static async void DelayedInvoke(UnityEvent thisEvent, int delay)
    {
        await Task.Delay(delay);
        thisEvent.Invoke();
    }

    /// <summary>
    /// Return true if there is at least one listener.
    /// </summary>
    /// <returns></returns>
    public static bool IsListening()
    {
        return eventDictionary.Count > 0;
    }

    /// <summary>
    /// Suspend the listening.
    /// </summary>
    public static void PauseListening()
    {
        SetPaused(true);
    }

    /// <summary>
    /// Suspend the listening of the event with the given name.
    /// </summary>
    /// <param name="eventName"></param>
    public static void PauseListening(string eventName)
    {
        SetPaused(eventName, true);
    }

    /// <summary>
    /// Restart the listening.
    /// </summary>
    public static void RestartListening()
    {
        SetPaused(false);
    }

    /// <summary>
    /// Restart the listening of the event with the given name.
    /// </summary>
    /// <param name="eventName"></param>
    public static void RestartListening(string eventName)
    {
        SetPaused(eventName, false);
    }

    /// <summary>
    /// Return true if the event with the given name has been paused.
    /// </summary>
    /// <param name="eventName"></param>
    /// <returns></returns>
    public static bool isPaused(string eventName)
    {
        if (paused.ContainsKey(eventName)) return paused[eventName]; else return true;
    }

    private static void SetPaused(bool value)
    {
        Dictionary<string, bool> copy = new Dictionary<string, bool>();

        foreach (KeyValuePair<string, bool> eName in paused)
        {
            copy.Add(eName.Key, value);
        }

        paused = copy;
    }

    private static void SetPaused(string eventName, bool value)
    {
        Dictionary<string, bool> copy = new Dictionary<string, bool>();

        foreach (KeyValuePair<string, bool> eName in paused)
        {
            if (eName.Key == eventName) copy.Add(eName.Key, value); else copy.Add(eName.Key, eName.Value);
        }

        paused = copy;
    }

    /// <summary>
    /// Return true if an event with the given name exists.
    /// </summary>
    public static bool EventExists(string eventName)
    {
        return eventDictionary.ContainsKey(eventName);
    }

    /// <summary>
    /// [DEPRECATED] Use Dispose() or DisposeAll().
    /// </summary>
    public static void ClearData()
    {
        storage = new Dictionary<string, object>();
        sender = new Dictionary<string, object>();
    }

    /// <summary>
    /// Clear the memory occupied by the specified event name. This method clear data only, whereas the listeners continue to work.
    /// </summary>
    /// <param name="eventName"></param>
    public static void Dispose(string eventName)
    {
        if (storage.ContainsKey(eventName)) storage.Remove(eventName);
        if (storage2.ContainsKey(eventName)) storage2.Remove(eventName);
        if (storage3.ContainsKey(eventName)) storage3.Remove(eventName);
    }

    /// <summary>
    /// Clear the memory occupied by the Event Manager system. This method clear data only, whereas the listeners continue to work.
    /// </summary>
    public static void DisposeAll()
    {
        storage.Clear();
        storage2.Clear();
        storage3.Clear();
        sender.Clear();
    }

    #endregion

    #region " GET AND SET DATA "

    /// <summary>
    /// Save data for the Event with the given name.
    /// </summary>
    public static void SetData(string eventName, object data)
    {
        if (storage.ContainsKey(eventName)) storage[eventName] = data; else storage.Add(eventName, data);
    }

    /// <summary>
    /// Return the data for the event with the given name (or null if nothing found).
    /// </summary>
    /// <param name="eventName"></param>
    /// <returns></returns>
    public static object GetData(string eventName)
    {
        try
        {
            if (storage.ContainsKey(eventName)) return storage[eventName]; else return null;
        }
        catch (System.Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// Return the GameObject data for the event with the given name (or null if nothing found).
    /// </summary>
    /// <param name="eventName"></param>
    /// <returns></returns>
    public static GameObject GetGameObject(string eventName)
    {
        try
        {
            if (storage.ContainsKey(eventName)) return (GameObject)storage[eventName]; else return null;
        }
        catch (System.Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// Return the integer data for the event with the given name (or 0 if nothing found).
    /// </summary>
    /// <param name="eventName"></param>
    /// <returns></returns>
    public static int GetInt(string eventName)
    {
        try
        {
            if (storage.ContainsKey(eventName)) return (int)storage[eventName]; else return 0;
        }
        catch (System.Exception)
        {
            return 0;
        }
    }

    /// <summary>
    /// Return the boolean data for the event with the given name (or false if nothing found).
    /// </summary>
    /// <param name="eventName"></param>
    /// <returns></returns>
    public static bool GetBool(string eventName)
    {
        try
        {
            if (storage.ContainsKey(eventName)) return (bool)storage[eventName]; else return false;
        }
        catch (System.Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Return the float data for the event with the given name (or 0 if nothing found).
    /// </summary>
    /// <param name="eventName"></param>
    /// <returns></returns>
    public static float GetFloat(string eventName)
    {
        try
        {
            if (storage.ContainsKey(eventName)) return (float)storage[eventName]; else return 0f;
        }
        catch (System.Exception)
        {
            return 0f;
        }
    }

    /// <summary>
    /// Return the string data for the event with the given name (or "" if nothing found).
    /// </summary>
    /// <param name="eventName"></param>
    /// <returns></returns>
    public static string GetString(string eventName)
    {
        try
        {
            if (storage.ContainsKey(eventName)) return (string)storage[eventName]; else return "";
        }
        catch (System.Exception)
        {
            return "";
        }
    }

    /// <summary>
    /// Return the sender for the event with the given name (or null if nothing found).
    /// </summary>
    /// <param name="eventName"></param>
    /// <returns></returns>
    public static object GetSender(string eventName)
    {
        try
        {
            if (sender.ContainsKey(eventName)) return sender[eventName]; else return null;
        }
        catch (System.Exception)
        {
            return null;
        }
    }

    #endregion

    #region " GET AND SET DATA GROUP "

    public struct DataGroup
    {
        /// <summary>
        /// The raw object data.
        /// </summary>
        public object data;

        /// <summary>
        /// Unique identifier for this DataGroup.
        /// </summary>
        public string id;

        /// <summary>
        /// Cast the object into GameObject.
        /// </summary>
        /// <returns></returns>
        public GameObject ToGameObject()
        {
            try
            {
                return (GameObject)data;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Cast the object into integer value.
        /// </summary>
        /// <returns></returns>
        public int ToInt()
        {
            try
            {
                return (int)data;
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Cast the object into float value.
        /// </summary>
        /// <returns></returns>
        public float ToFloat()
        {
            try
            {
                return (float)data;
            }
            catch (System.Exception)
            {
                return 0f;
            }
        }

        /// <summary>
        /// Cast the object into a string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            try
            {
                return (string)data;
            }
            catch (System.Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// Cast the object into a boolean value.
        /// </summary>
        /// <returns></returns>
        public bool ToBool()
        {
            try
            {
                return (bool)data;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

    }

    /// <summary>
    /// Save a set of data for the Event with the given name. Then use GetDataGroup method to have access to this data.
    /// </summary>
    public static void SetDataGroup(string eventName, params object[] data)
    {
        if (storage3.ContainsKey(eventName)) { Debug.LogWarning(eventName + " Event name is already in use with DataGroup."); return; }
        if (storage2.ContainsKey(eventName)) storage2[eventName] = data; else storage2.Add(eventName, data);
    }

    /// <summary>
    /// Return a structured Array containing all the Event data, or null if nothing found.
    /// </summary>
    public static DataGroup[] GetDataGroup(string eventName)
    {
        if (storage2.ContainsKey(eventName))
        {

            var strg = storage2[eventName];
            DataGroup[] objList = new DataGroup[strg.Length];

            for (var i = 0; i < strg.Length; i++)
            {
                objList[i] = new DataGroup { data = strg[i] };
            }

            return objList;

        }

        return null;
    }


    #endregion

    #region " GET AND SET INDEXED DATA GROUP "

    public struct IndexedDataGroup
    {

        public DataGroup[] data;

        private object objectData;

        /// <summary>
        /// Return the raw object.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public object GetObject(string id)
        {
            return null;
        }

        /// <summary>
        /// Cast the object into GameObject.
        /// </summary>
        /// <returns></returns>
        public GameObject ToGameObject(string id)
        {
            objectData = Find(id);

            try
            {
                return (GameObject)objectData;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Cast the object into integer value.
        /// </summary>
        /// <returns></returns>
        public int ToInt(string id)
        {
            objectData = Find(id);

            try
            {
                return (int)objectData;
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Cast the object into float value.
        /// </summary>
        /// <returns></returns>
        public float ToFloat(string id)
        {
            objectData = Find(id);

            try
            {
                return (float)objectData;
            }
            catch (System.Exception)
            {
                return 0f;
            }
        }

        /// <summary>
        /// Cast the object into a string.
        /// </summary>
        /// <returns></returns>
        public string ToString(string id)
        {
            objectData = Find(id);

            try
            {
                return (string)objectData;
            }
            catch (System.Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// Cast the object into a boolean value.
        /// </summary>
        /// <returns></returns>
        public bool ToBool(string id)
        {
            objectData = Find(id);

            try
            {
                return (bool)objectData;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Return true if there is no data.
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return data.Length == 0;
        }

        private object Find(string id)
        {
            foreach (var obj in data) if (obj.id == id) return obj.data;
            return null;
        }

    }

    /// <summary>
    /// Save a set of DataGroups for the Event with the given name. Then use GetIndexedDataGroup method to have access to this kind of data.
    /// </summary>
    public static void SetIndexedDataGroup(string eventName, params DataGroup[] data)
    {
        if (storage2.ContainsKey(eventName)) { Debug.LogWarning(eventName + " Event name is already in use with DataGroup."); return; }
        if (storage3.ContainsKey(eventName)) storage3[eventName] = data; else storage3.Add(eventName, data);
    }

    /// <summary>
    /// Return a structured DataGroup containing all the Event data, or null if nothing found.
    /// </summary>
    public static IndexedDataGroup GetIndexedDataGroup(string eventName)
    {
        if (storage3.ContainsKey(eventName))
        {

            var strg = storage3[eventName];

            IndexedDataGroup data = new IndexedDataGroup();
            data.data = strg;

            return data;

        }

        return new IndexedDataGroup();
    }

    #endregion

}

#region " EVENTS GROUP "

/// <summary>
/// Create a Group of Events which execution can be started and stopped.
/// </summary>
public class EventsGroup
{

    private struct SEvent
    {
        public string name;
        public UnityAction callBack;
    }

    private List<SEvent> group = new List<SEvent>();

    /// <summary>
    /// Add a new Listener to this EventsGroup.
    /// </summary>
    public void Add(string eventName, UnityAction callBack)
    {
        group.Add(new SEvent { name = eventName, callBack = callBack });
    }

    /// <summary>
    /// Start the listening to all the Events in the Group.
    /// </summary>
    public void StartListening()
    {
        foreach (SEvent g in group)
        {
            EventManager.StartListening(g.name, g.callBack);
        }
    }

    /// <summary>
    /// Stop the listening to all the Events in the Group. If an eventName is specified, only that Event is stopped.
    /// </summary>
    public void StopListening(string eventName = "")
    {
        if (eventName == "")
        {
            foreach (SEvent g in group)
            {
                EventManager.StopListening(g.name, g.callBack);
            }
        }
        else
        {
            List<SEvent> newGroup = new List<SEvent>();
            foreach (SEvent g in group)
            {
                if (g.name != eventName) newGroup.Add(g); else EventManager.StopListening(g.name, g.callBack);
            }
            group = newGroup;
        }

    }

    /// <summary>
    /// Return true if the EventsGroup contains an Event with the given name.
    /// </summary>
    public bool Contains(string eventName)
    {
        foreach (SEvent g in group)
        {
            if (g.name == eventName) return true;
        }
        return false;
    }

}

#endregion