namespace Spark
{
    public interface IInteractable
    {
        public void OnInteract(Player player);
        public void OnSecondaryInteract(Player player) { }
    }
}
