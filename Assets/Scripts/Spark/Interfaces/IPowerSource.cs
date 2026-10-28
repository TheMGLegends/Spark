
namespace Spark
{
    public interface IPowerSource
    {
        public Terminal PositiveTerminal { get; }
        public Terminal NegativeTerminal { get; }
    }
}
