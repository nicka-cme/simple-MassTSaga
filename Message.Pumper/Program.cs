namespace Message.Pumper
{
    using System;
    using System.Threading.Tasks;

    using MassTransit;

    using MassTransSaga.Model;

    public class Program
    {
        private const string RabbitUserName = "guest";

        private const string RabbitPassword = "guest";

        private const int Total = 1_000;

        private static readonly Uri HostAddress = new Uri("rabbitmq://localhost/");

        static void Main(string[] args)
        {

            var busControl = Bus.Factory.CreateUsingRabbitMq(
                cfg =>
                    {
                        var host = cfg.Host(
                            HostAddress,
                            h =>
                                {
                                    h.Username(RabbitUserName);
                                    h.Password(RabbitPassword);
                                });
                    });


            var endPoint = busControl.GetSendEndpoint(new Uri(HostAddress, "Wiggly.Wombat.TheGreatSaga")).Result;

            var sameGuid = Guid.NewGuid();
            for (int i = 1; i <= Total; i++)
            {
                endPoint.Send(new UpdateGreatState(sameGuid, "MagicMike")).Wait();
            }
        }
    }
}
