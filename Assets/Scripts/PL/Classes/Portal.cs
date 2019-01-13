namespace Planetar
{
    public class Portal : Shared.Interactive
    {
        public Planet Source;
        public Planet Target;
        public bool Breakable;
        public int Limit;
        public SSHRole Role;

        public bool IsActive()
        {
            return (Limit > 0) || (Limit == -1);
        }
    }

    public class PortalShip : Portal
    {
        public Ship Initiator;

        public PortalShip(Ship AInitiator)
        {
            Initiator = AInitiator;
            Initiator.ShowPath(true);
        }
    }
}