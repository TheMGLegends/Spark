using System.Collections.Generic;

namespace Spark
{
    public interface IElectricalDevice
    {
        public bool IsPowered { get; }
        public IReadOnlyList<Terminal> Terminals { get; }

        public void SetPowered(bool powered);
        public bool CanCurrentPass(Terminal terminalA, Terminal terminalB);
    }
}
