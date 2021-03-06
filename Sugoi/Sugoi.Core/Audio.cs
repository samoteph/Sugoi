﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sugoi.Core
{
    public class Audio
    {
        private Machine machine;
        private HashSet<string> soundNames = new HashSet<string>();

        public void Start(Machine machine)
        {
            this.machine = machine;
            soundNames.Clear();
        }

        public Task PreloadAsync(string name, int channelCount)
        {
            try
            {
                if (this.soundNames.Contains(name) == false)
                {
                    soundNames.Add(name);
                    return this.machine.PreloadSoundAsyncCallBack(name, channelCount);
                }
                else
                {
                    return Task.CompletedTask;
                }
            }
            catch(Exception ex)
            {
                throw new Exception("Error during the preloading of sound '" + name + "'. Exception message=" + ex.Message);
            }
        }

        public string CurrentLoopName
        {
            get;
            private set;
        }

        public void PlayLoop(string name)
        {
            if (CurrentLoopName != name)
            {
                this.CurrentLoopName = name;
                this.Play(name, 1, true);
            }
        }

        public void Play(string name)
        {
            this.Play(name, 1, false);
        }

        public void Play(string name, double volume, bool isLoop)
        {
            if (this.soundNames.Contains(name) == true)
            {
                this.machine.PlaySoundCallBack?.Invoke(name, volume, isLoop);
            }
            else
            {
                throw new Exception("the sound '" + name +"' is not preloaded!");
            }
        }

        public void Pause(string name)
        {
            if (this.soundNames.Contains(name) == true)
            {
                this.machine.PauseSoundCallBack?.Invoke(name);
            }
            else
            {
                throw new Exception("the sound '" + name + "' is not preloaded!");
            }
        }

        public void Pause()
        {
            if (this.machine.PauseSoundCallBack != null)
            {
                foreach (var soundName in soundNames)
                {
                    this.machine.PauseSoundCallBack.Invoke(soundName);
                }
            }
        }

        public void Stop(string name)
        {
            if (this.soundNames.Contains(name) == true)
            {
                this.machine.StopSoundCallBack?.Invoke(name);
            }
            else
            {
                throw new Exception("the sound '" + name + "' is not preloaded!");
            }
        }
    }
}
