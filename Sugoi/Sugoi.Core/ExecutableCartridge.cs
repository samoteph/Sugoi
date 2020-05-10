
using System.Reflection;

namespace Sugoi.Core.IO
{
    public abstract class ExecutableCartridge : Cartridge
    {
        public abstract void Start(Machine machine);
    }
}
