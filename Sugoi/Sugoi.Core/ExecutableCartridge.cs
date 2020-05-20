
using System.Reflection;
using System.Threading.Tasks;

namespace Sugoi.Core.IO
{
    public abstract class ExecutableCartridge : Cartridge
    {
        public abstract Task StartAsync(Machine machine);
    }
}
