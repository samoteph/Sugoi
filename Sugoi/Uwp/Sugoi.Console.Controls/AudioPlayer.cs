using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media.Audio;
using Windows.Media.Render;
using Windows.Storage;

namespace SamuelBlanchard.Audio
{
    public class AudioPlayer<TKey>
    {
        public AudioPlayer()
        {
        }

        public AudioGraph AudioGraph
        {
            get
            {
                return _audioGraph;
            }
        }

        private AudioGraph _audioGraph;

        public AudioDeviceOutputNode AudioDeviceOuputNode
        {
            get
            {
                return _outputNode;
            }
        }

        private AudioDeviceOutputNode _outputNode;

        private Dictionary<TKey, AudioFileInputSource> soundLibrary = new Dictionary<TKey, AudioFileInputSource>();

        private class AudioFileInputSource
        {
            public int Index
            {
                get
                {
                    return _index;
                }
            }

            private int _index;

            public List<AudioFileInputNode> InputNodes
            {
                get;
                private set;
            } = new List<AudioFileInputNode>();

            public AudioFileInputNode GetInputNode()
            {
                int index = _index;
                var nodes = this.InputNodes;

                index = _index % nodes.Count;

                var result = nodes[index];

                _index++;

                return result;
            }
        }

        public bool IsMute
        {
            get
            {
                return _isMute;
            }

            set
            {
                if (this._isMute != value)
                {
                    this._isMute = value;

                    if (this._audioGraph != null)
                    {
                        if (value == true)
                        {
                            volumeBeforeMute = Volume;
                            Volume = 0;
                        }
                        else
                        {
                            Volume = volumeBeforeMute;
                        }
                    }
                }
            }
        }

        private bool _isMute = false;

        /// <summary>
        /// Swith mute
        /// </summary>

        public void SwitchMute()
        {
            this.IsMute = !IsMute;
        }

        public double Volume
        {
            get
            {
                return _outputNode.OutgoingGain;
            }

            set
            {
                _outputNode.OutgoingGain = value;
            }
        }

        private double volumeBeforeMute = 1.0;

        public bool IsInitialized
        {
            get;
            private set;
        }

        public async Task<bool> InitializeAsync()
        {
            if (this.IsInitialized == true)
            {
                return true;
            }

            var result = await AudioGraph.CreateAsync(new AudioGraphSettings(AudioRenderCategory.Media));

            if (result.Status != AudioGraphCreationStatus.Success)
            {
                return false;
            }

            _audioGraph = result.Graph;
            var outputResult = await _audioGraph.CreateDeviceOutputNodeAsync();
            if (outputResult.Status != AudioDeviceNodeCreationStatus.Success)
            {
                return false;
            }

            _outputNode = outputResult.DeviceOutputNode;

            if (this.IsMute == false)
            {
                _audioGraph.Start();
            }

            this.IsInitialized = true;

            return true;
        }

        public async Task<bool> CopySound(TKey keySource, TKey keyDestination)
        {
            if (this.IsInitialized == false)
            {
                return false;
            }

            var inputSource = this.GetSound(keySource);

            var file = inputSource.GetInputNode().SourceFile;

            return await this.AddSoundAsync(keyDestination, file);
        }

        public async Task<bool> AddSoundAsync(TKey key, StorageFile soundFile, int inputCount = 1)
        {
            if (this.IsInitialized == false)
            {
                return false;
            }

            if (soundLibrary.ContainsKey(key) == false)
            {
                var source = new AudioFileInputSource();

                for (int x = 0; x < inputCount; x++)
                {
                    var fileInputNodeResult = await _audioGraph.CreateFileInputNodeAsync(soundFile);

                    if (fileInputNodeResult.Status != AudioFileNodeCreationStatus.Success)
                    {
                        return false;
                    }

                    var fileInputNode = fileInputNodeResult.FileInputNode;

                    fileInputNode.Stop();

                    fileInputNode.AddOutgoingConnection(_outputNode);

                    source.InputNodes.Add(fileInputNode);
                }

                this.soundLibrary.Add(key, source);
            }
            else
            {
                throw new Exception("The sound '" + key + "' already exists in the sound library!");
            }

            return true;
        }


        public async Task<bool> AddSoundFromApplicationAsync(TKey key, string uriFile, int inputCount = 1)
        {
            var soundFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(uriFile));
            await AddSoundAsync(key, soundFile, inputCount);

            return true;
        }

        public void Stop(TKey key)
        {
            if (this.IsInitialized == false)
            {
                return;
            }

            var fileInputSource = GetSound(key);

            foreach (var node in fileInputSource.InputNodes)
            {
                node.Stop();
            }
        }

        public bool IsStopping
        {
            get;
            private set;
        }

        public void Stop()
        {
            if (this.IsInitialized == false)
            {
                return;
            }

            this.IsStopping = true;

            foreach (var audioKey in this.soundLibrary.Keys)
            {
                Stop(audioKey);
            }

            this.IsStopping = false;
        }

        /// <summary>
        /// Si l'on retire tous les sons on arrête de les jouer
        /// </summary>

        public void RemoveSound()
        {
            if (this.IsInitialized == false)
            {
                return;
            }

            this.IsStopping = true;

            foreach (var audioKey in this.soundLibrary.Keys)
            {
                RemoveSound(audioKey);
            }

            this.IsStopping = false;
        }

        /// <summary>
        /// Retirer le son de la librairie
        /// </summary>
        /// <param name="key"></param>
        public void RemoveSound(TKey key)
        {
            if (this.IsInitialized == false)
            {
                return;
            }

            var fileInputSource = GetSound(key);

            foreach (var node in fileInputSource.InputNodes)
            {
                node.Stop();
                node.RemoveOutgoingConnection(_outputNode);
                node.Dispose();
            }

            this.soundLibrary.Remove(key);
        }

        /// <summary>
        /// Jouer et attendre
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>

        public async Task<bool> PlaySoundAsync(TKey key, double volume = 1)
        {
            if (this.IsInitialized == false)
            {
                return false;
            }

            var fileInputSource = GetSound(key);

            var fileInputNode = fileInputSource.GetInputNode();

            TypedEventHandler<AudioFileInputNode, object> completed = null;

            TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();

            completed = (audioFileInputNode, args) =>
            {
                fileInputNode.FileCompleted -= completed;
                taskCompletionSource.TrySetResult(null);
            };

            fileInputNode.FileCompleted += completed;

            fileInputNode.Seek(TimeSpan.Zero);

            this.SetVolume(fileInputNode, volume);

            fileInputNode.Start();

            await taskCompletionSource.Task;

            return true;
        }

        public void SetVolume(TKey key, double volume)
        {
            var fileInputSource = GetSound(key);

            foreach (var node in fileInputSource.InputNodes)
            {
                SetVolume(node, volume);
            }
        }

        private void SetVolume(AudioFileInputNode fileInputNode, double volume)
        {
            fileInputNode.OutgoingGain = volume;
        }

        public bool PlayLoop(TKey key, double volume = 1)
        {
            return this.PlaySound(key, volume, true);
        }

        public bool PlaySound(TKey key, double volume = 1)
        {
            return this.PlaySound(key, volume, false);
        }

        private bool PlaySound(TKey key, double volume = 1, bool isLoop = false)
        {
            if (this.IsInitialized == false)
            {
                return false;
            }

            if (this.IsStopping == true)
            {
                return false;
            }

            var fileInputSource = GetSound(key);

            var fileInputNode = fileInputSource.GetInputNode();

            fileInputNode.Stop();

            if (isLoop)
            {
                fileInputNode.LoopCount = null;
            }
            else
            {
                fileInputNode.LoopCount = 0;
            }

            fileInputNode.Seek(TimeSpan.Zero);

            this.SetVolume(fileInputNode, volume);

            if (this.IsStopping == false)
            {
                fileInputNode.Start();
            }

            return true;
        }

        private AudioFileInputSource GetSound(TKey key)
        {
            if (soundLibrary.ContainsKey(key))
            {
                return this.soundLibrary[key];
            }

            throw new Exception("The sound '" + key + "' doesn't exist in the sound library!");
        }
    }
}