namespace Assets.LD48.Scripts
{
    public class FuelIndicator : UIIndicator
    {
        private void Awake()
        {
            Ship.OnFuelChanged += this.UpdateIndicator;
        }

        private void OnDestroy()
        {
            Ship.OnFuelChanged -= this.UpdateIndicator;
        }
    }
}