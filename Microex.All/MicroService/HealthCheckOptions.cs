namespace Microex.All.MicroService
{
    public class HealthCheckOptions
    {
        public int Interval { get; set; } = 300;
        public int DeregisterTimeout { get; set; } = 900;
    }
}