namespace Assets.LD48.Scripts
{
    public class HealthIndicator : UIIndicator
    {
        private void Awake()
        {
            Ship.OnHealthChanged += this.UpdateIndicator;
        }

        private void OnDestroy()
        {
            Ship.OnHealthChanged -= this.UpdateIndicator;
        }
    }
}