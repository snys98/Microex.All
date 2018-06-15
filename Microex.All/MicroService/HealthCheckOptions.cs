namespace Microex.All.MicroService
{
    public class HealthCheckOptions
    {
        public int Interval { get; set; } = 30;
        public int DeregisterTimeout { get; set; } = 60;
    }
}