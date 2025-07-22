using System.Text.Json;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;

namespace CommandsService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopedFactory;
        private readonly IMapper _mapper;

        public EventProcessor(IServiceScopeFactory scopedFactory, IMapper mapper)
        {
            _scopedFactory = scopedFactory;
            _mapper = mapper;
        }

        public void ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);

            switch (eventType)
            {
                case EventType.PlatformPublished:
                    AddPlatform(message);
                    break;
                default:
                    break;
            }
        }

        private void AddPlatform(string platformPublishedMessage)
        {
            using (var scope = _scopedFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();
                var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);

                try
                {
                    var platform = _mapper.Map<Platform>(platformPublishedDto);

                    if (!repo.ExternalPlatformExists(platform.Id))
                    {
                        repo.CreatePlatform(platform);
                        repo.SaveChanges();
                        Console.WriteLine("--> Platform added succesfuly");
                    }
                    else
                    {
                        Console.WriteLine("--> Platform already exists");
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> Could not add platform to DB {ex.Message}");
                }
            }
        }

        private EventType DetermineEvent(string notificationMessage)
        {
            Console.WriteLine("--> Determining Event");

            var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

            switch (eventType?.Event)
            {
                case "Platform_Published":
                    Console.WriteLine("--> Platform_Published event detected");
                    return EventType.PlatformPublished;
                default:
                    return EventType.Undetermined;
            }
        }

        enum EventType
        {
            PlatformPublished,
            Undetermined
        }
    }
}