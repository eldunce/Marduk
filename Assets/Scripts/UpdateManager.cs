using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Marduk
{
    public class UpdateManager : MonoBehaviour
    {
        struct UpdatableSchedule
        {
            public IUpdatable updatable;
            public float startTimeMin, startTimeMax, updateIntervalMin, updateIntervalMax;
            public float timeTillNextUpdate;
            public bool valid;
            public UpdatableSchedule(IUpdatable _updatable, float _startTimeMin, float _startTimeMax, float _updateIntervalMin, float _updateIntervalMax)
            {
                updatable = _updatable;
                startTimeMin = _startTimeMin;
                startTimeMax = _startTimeMax;
                updateIntervalMin = _updateIntervalMin;
                updateIntervalMax = _updateIntervalMax;
                timeTillNextUpdate = 0f;
                valid = false;
            }

        }

        struct UpdatableInfo
        {
            public IUpdatable updatable;
            public bool valid;
        }

        // static stuff
        static UpdateManager s_instance = null;
        static List<UpdatableSchedule> s_cachedUpdatables = null;

        public int maxUpdatables = 10000;
        int lastValidScheduleIndex = 0;
        int lastValidRegularIndex = 0;


        enum UpdateType
        {
            Regular,
            Scheduled
        }

        UpdatableSchedule[] scheduledUpdates;
        UpdatableInfo [] regularUpdates;


        public static UpdateManager Instance
        {
            get
            {
                return s_instance;
            }
        }

        public static void RegisterUpdatable(IUpdatable upd)
        {
            if(s_instance == null)
            {
                if (s_cachedUpdatables == null)
                    s_cachedUpdatables = new List<UpdatableSchedule>();
                if (s_cachedUpdatables.FindIndex((x) => x.updatable == upd) == -1)
                    s_cachedUpdatables.Add(new UpdatableSchedule(upd, 0f, 0f, 0f, 0f));
            }
            else
            {
                s_instance.RegisterUpdatableInternal(upd);
            }
        }


        public static void RegisterUpdatable(IUpdatable upd, float startTimeMin, float startTimeMax, float updateIntervalMin, float updateIntervalMax)
        {
            if(s_instance == null)
            {
                if (s_cachedUpdatables == null)
                    s_cachedUpdatables = new List<UpdatableSchedule>();
                if(s_cachedUpdatables.FindIndex((x)=>x.updatable == upd)==-1)
                    s_cachedUpdatables.Add(new UpdatableSchedule(upd, startTimeMin, startTimeMax, updateIntervalMin, updateIntervalMax));
            }
            else
            {
                if (updateIntervalMax > 0f && updateIntervalMin > 0f)
                {
                    s_instance.RegisterUpdatableInternal(upd, startTimeMin, startTimeMax, updateIntervalMin, updateIntervalMax);
                }
                else
                {
                    s_instance.RegisterUpdatableInternal(upd);
                }
            }
        }
        

        void RegisterUpdatableInternal(IUpdatable upd, float startTimeMin, float startTimeMax, float updateIntervalMin, float updateIntervalMax)
        {
            int index = FirstValidIndex(UpdateType.Scheduled);
            if(index != -1)
            {
                ReserveIndex(index, UpdateType.Scheduled);
                scheduledUpdates[index] = new UpdatableSchedule(upd, startTimeMin, startTimeMax, updateIntervalMin, updateIntervalMax);
                scheduledUpdates[index].timeTillNextUpdate = Random.Range(startTimeMin, startTimeMax);
                scheduledUpdates[index].valid = true;
            }
            else
            {
                Debug.Log("UpdateManager has no more slots", this);
            }
        }

        void RegisterUpdatableInternal(IUpdatable upd)
        {
            int index = FirstValidIndex(UpdateType.Regular);
            if(index != -1)
            {
                ReserveIndex(index, UpdateType.Regular);
                regularUpdates[index].valid = true;
                regularUpdates[index].updatable = upd;
            }
        }

        public static void DeregisterUpdatable(IUpdatable upd)
        {
            if(s_instance == null && s_cachedUpdatables == null)
            {
                s_cachedUpdatables.RemoveAt(s_cachedUpdatables.FindIndex((x) => x.updatable == upd));
            }
            else
            {
                s_instance.DeregisterUpdatableInternal(upd);
            }
        }

        void DeregisterUpdatableInternal(IUpdatable upd)
        {
            for(int i = 0; i <= lastValidScheduleIndex; i++)
            {
                if(scheduledUpdates[i].updatable == upd)
                {
                    scheduledUpdates[i].updatable = null;
                    scheduledUpdates[i].valid = false;
                    FreeIndex(i, UpdateType.Scheduled);
                    return;
                }
            }
            for(int i = 0; i <= lastValidRegularIndex; i++)
            {
                if(regularUpdates[i].updatable == upd)
                {
                    regularUpdates[i].valid = false;
                    regularUpdates[i].updatable = null;
                    FreeIndex(i, UpdateType.Regular);
                    return;
                }
            }
        }

        int FirstValidIndex(UpdateType type)
        {
            if (type == UpdateType.Scheduled)
            {
                for (int i = 0; i < scheduledUpdates.Length; i++)
                {
                    if (!scheduledUpdates[i].valid) return i;
                }
            }
            else if(type == UpdateType.Regular)
            {
                for(int i = 0; i < regularUpdates.Length; i++)
                {                    
                    if (!regularUpdates[i].valid) return i;
                }
            }
            return -1;
        }

        void ReserveIndex(int index, UpdateType type)
        {
            if (type == UpdateType.Scheduled)
            {
                if (index > lastValidScheduleIndex)
                {
                    lastValidScheduleIndex = index;
                }
            }
            else if (type == UpdateType.Regular)
            {
                if(index > lastValidRegularIndex)
                {
                    lastValidRegularIndex = index;
                }
            }
        }

        void FreeIndex(int index, UpdateType type)
        {
            if (type == UpdateType.Scheduled)
            {
                if (index == lastValidScheduleIndex)
                {
                    lastValidScheduleIndex--;
                }
            }
            if (type == UpdateType.Regular)
            {
                if (index == lastValidRegularIndex)
                {
                    lastValidRegularIndex--;
                }
            }
        }
            
        // Use this for initialization
        void Awake()
        {
            if(s_instance == null)
            {
                s_instance = this;
            }

            scheduledUpdates = new UpdatableSchedule[maxUpdatables];
            for(int i = 0; i < scheduledUpdates.Length; i++)
            {
                scheduledUpdates[i].valid = false;
            }

            regularUpdates = new UpdatableInfo[maxUpdatables];
            for(int i = 0; i < regularUpdates.Length; i++)
            {
                regularUpdates[i].valid = false;
            }

            lastValidScheduleIndex = 0;
            lastValidRegularIndex = 0;

            if (s_cachedUpdatables != null)
            {
                for (int i = 0; i < s_cachedUpdatables.Count; i++)
                {
                    if (s_cachedUpdatables[i].updateIntervalMax > 0f && s_cachedUpdatables[i].updateIntervalMin > 0f)
                    {
                        RegisterUpdatableInternal(s_cachedUpdatables[i].updatable,
                            s_cachedUpdatables[i].startTimeMin,
                            s_cachedUpdatables[i].startTimeMax,
                            s_cachedUpdatables[i].updateIntervalMin,
                            s_cachedUpdatables[i].updateIntervalMax);
                    }
                    else
                    {
                        RegisterUpdatableInternal(s_cachedUpdatables[i].updatable);
                    }
                }
            }
        }


        // Update is called once per frame
        void Update()
        {
            for (int i = 0; i <= lastValidScheduleIndex; i++)
            {
                scheduledUpdates[i].timeTillNextUpdate -= Time.deltaTime;
                if (scheduledUpdates[i].valid && scheduledUpdates[i].timeTillNextUpdate <= 0f)
                {
                    scheduledUpdates[i].updatable.ManagedUpdate();
                    scheduledUpdates[i].timeTillNextUpdate = Random.Range(scheduledUpdates[i].updateIntervalMin, scheduledUpdates[i].updateIntervalMax);
                }
            }

            for(int i = 0; i <= lastValidRegularIndex; i++)
            {
                if(regularUpdates[i].valid)
                {
                    regularUpdates[i].updatable.ManagedUpdate();
                }
            }
        }
    }
}
