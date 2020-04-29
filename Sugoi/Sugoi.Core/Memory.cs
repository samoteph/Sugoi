using System;
using System.Collections.Generic;
using System.Text;

namespace Sugoi.Core
{
    internal class Memory
    {
        public const int MEMORY_LENGTH = 64000; // 64000 * 4 octet => 256k

        private int[] rawMemory = new int[MEMORY_LENGTH];
    
        public int this[int address]
        {
            get
            {
                return this.rawMemory[address];
            }

            set
            {
                this.rawMemory[address] = value;
            }
        }

        public bool IsStarted
        {
            get
            {
                return isStarted;
            }
        }

        private bool isStarted = false;

        public void Start()
        {
            if(isStarted == true)
            {
                return;
            }

            isStarted = true;

            // nettoyage de la mémoire
            for(int x = 0; x < MEMORY_LENGTH; x++)
            {
                rawMemory[x] = 0x00;
            }
        }

        public void Stop()
        {
            if(isStarted == false)
            {
                return;
            }

            // ici on arrete les choses

            isStarted = false;
        }
    }
}
