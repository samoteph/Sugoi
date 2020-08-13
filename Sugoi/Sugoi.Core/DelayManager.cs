using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Sugoi.Core
{
    /// <summary>
    /// Decremente a multiple counters of frame then execute actions (one by counter)
    /// </summary>

    public class DelayManager
    {
        private DelayItem[] items;
        private int index = 0;

        private object locker = new object();

        public DelayManager(int size = 50)
        {
            items = new DelayItem[size];

            for (int i = 0; i < items.Length; i++)
            {
                items[i] = new DelayItem();
            }
        }

            private DelayItem FindFreeDelayItem()
        {
            lock (locker)
            {
                var length = items.Length;

                for (int i = 0; i < length; i++)
                {
                    int j = (index + i) % length;

                    if(items[j].IsFree)
                    {
                        index = j;
                        return items[j];
                    }
                }

                throw new Exception("No free DelayItem!");
            }
        }

        public void AddDelay(int frame, Action completed)
        {
            Debug.Assert(completed != null, "the completed action must be non nullable!");

            var item = this.FindFreeDelayItem();
            item.Rent(frame, completed);
        }

        /// <summary>
        /// Execute one frame of all DelayItems
        /// </summary>

        public void ExecuteOneFrame()
        {
            lock (locker)
            {
                for(int i=0; i < items.Length; i++)
                {
                    var item = items[i];

                    // se libère automatiquement
                    item.ExecuteOneFrame();
                }
            }
        }

    }

    internal class DelayItem
    {
        public void Rent(int frameBeforeExecution, Action completed)
        {
            Debug.Assert(frameBeforeExecution > 0);

            this.FrameBeforeExecution = frameBeforeExecution;
            this.Completed = completed;
        }

        public void Return()
        {
            this.FrameBeforeExecution = -1;
        }

        public bool IsFree
        {
            get
            {
                return this.FrameBeforeExecution < 0;
            }
        }

        public int FrameBeforeExecution
        {
            get;
            private set;
        } = -1;

        public Action Completed
        {
            get;
            private set;
        }

        public bool ExecuteOneFrame()
        {
            // par sécurité si l'execution du Completed prend plus d'une frame
            if(this.FrameBeforeExecution <= 0)
            {
                return false;
            }

            this.FrameBeforeExecution--;

            if(this.FrameBeforeExecution == 0)
            {
                this.Completed.Invoke();
                // libère
                this.Return();
                return true;
            }

            return false;
        }
    }
}
