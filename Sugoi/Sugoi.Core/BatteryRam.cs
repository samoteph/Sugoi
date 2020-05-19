using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sugoi.Core
{
    public class BatteryRam
    {
        public const int BATTERY_RAM_SIZE = 10 * 1000;

        private Machine machine;
        private byte[] memory; 
    
        public void WriteInt(int address, int value)
        {
            memory[address + 0] = (byte)(value >> 24);
            memory[address + 1] = (byte)(value >> 16);
            memory[address + 2] = (byte)(value >> 8);
            memory[address + 3] = (byte)value;
        }

        public int ReadInt(int address)
        {
            int value =
            memory[address + 0] << 24 | 
            memory[address + 1] << 16 |
            memory[address + 2] << 8 |
            memory[address + 3];

            return value;
        }

        public void WriteByte(int address, byte value)
        {
            memory[address] = value;
        }

        public byte ReadByte(int address)
        {
            return memory[address];
        }

        public void WriteChar(int address, char value)
        {
            int intChar = (int)value;
            memory[address + 0] = (byte)(intChar >> 8);
            memory[address + 1] = (byte)(intChar);
        }

        public char ReadChar(int address)
        {
            int intChar = 
            memory[address + 0] << 8 |
            memory[address + 1];

            return (char)intChar;
        }

        /// <summary>
        /// Chargement de la Ram
        /// </summary>

        public async Task StartAsync(Machine machine)
        {
            this.machine = machine;

            byte[] ram = await machine.ReadBatteryRamAsync();
        
            if(ram == null || ram.Length == 0)
            {
                ram = new byte[BATTERY_RAM_SIZE];
            }

            this.memory = ram;
        }

        /// <summary>
        /// Ecriture de la Ram
        /// </summary>

        public Task<bool> FlashAsync()
        {
            return machine.WriteBatteryRamAsync(memory);
        }
    }
}
