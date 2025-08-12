using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Extensions;
using System.Collections.Concurrent;
using Extensions.ThreadGroup;
using System.Threading;

namespace TheChosenProject
{
    public abstract class ConcurrentSmartThreadQueue<T>
    {
        public ConcurrentQueue<T> Queues;

        public ThreadItem thread;

        public int Count => Queues.Count;

        public ConcurrentSmartThreadQueue(int Processors)
        {
            Queues = new ConcurrentQueue<T>();
        }

        protected abstract void OnDequeue(T Value, int time);

        public bool Finish()
        {
            while (Queues.Count > 0)
            {
                Thread.Sleep(1);
            }
            Thread.Sleep(500);
            return true;
        }

        public void Start(int period)
        {
            thread = new ThreadItem(period, "ConquerThreadQueue", Work);
            thread.Open();
        }

        public void Work()
        {
            try
            {
                Time32 timer;
                timer = Time32.Now;
                T obj;
                while (Queues.TryDequeue(out obj))
                {
                    OnDequeue(obj, timer.AllMilliseconds);
                }
            }
            catch (Exception e)
            {
                ServerKernel.Log.SaveLog(e.ToString(), false, LogType.EXCEPTION);
            }
        }

        public virtual void Enqueue(T obj)
        {
            Queues.Enqueue(obj);
        }
    }
}
