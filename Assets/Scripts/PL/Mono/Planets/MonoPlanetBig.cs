namespace Planetar
{
    public class MonoPlanetBig : MonoPlanetCustom
    {

        protected override void Start()
        {
            
        }

        protected override void DoEnable()
        {
            enabled = true;
        }

        protected override void DoDisable()
        {
            enabled = false;
        }
    }
}