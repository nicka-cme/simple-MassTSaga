<Query Kind="Program">
  <Reference>C:\Develop\CME Connect\cme-core\CmeConnect.Content.Common\bin\Debug\CmeConnect.Content.Common.dll</Reference>
  <Reference>C:\Develop\CME Connect\cme-core\CmeConnect.Content.Contracts\bin\Debug\CmeConnect.Content.Contracts.dll</Reference>
  <Reference Relative="bin\Debug\MassTransSaga.exe">P:\My Develop\Wiggly Wombat\C#\MassTransSaga\MassTransSaga\bin\Debug\MassTransSaga.exe</Reference>
  <NuGetReference>MassTransit</NuGetReference>
  <NuGetReference>MassTransit.RabbitMQ</NuGetReference>
  <Namespace>CmeConnect.Content.Common.Events</Namespace>
  <Namespace>CmeConnect.Content.Common.Model</Namespace>
  <Namespace>CmeConnect.Content.Contracts.Model</Namespace>
  <Namespace>MassTransit</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>MassTransSaga.Model</Namespace>
</Query>

readonly Uri HostAddress = new Uri("rabbitmq://localhost/");
const string RabbitUserName = "guest";
const string RabbitPassword = "guest";
readonly int Total = 10_000;

async Task Main()
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


	var endPoint = await busControl.GetSendEndpoint(new Uri("rabbitmq://localhost/Wiggly.Wombat.TheGreatSaga"));

	var sameGuid = Guid.NewGuid();
	for (int i = 1; i <= Total; i++)
	{
		 await endPoint.Send(new UpdateGreatState(sameGuid, "MagicMike"));
	}
}

public class UpdateGreatState : IUpdateGreatState
{
	public UpdateGreatState(Guid eventId, string magic)
	{
		this.EventId = eventId;
		this.Magic = magic;
	}

	public Guid EventId { get; }

	public string Magic { get; }
}