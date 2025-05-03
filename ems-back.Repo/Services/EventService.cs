using ems_back.Repo.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ems_back.Repo.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IUserRepository _userRepository;
        public EventService(IEventRepository eventRepository, IOrganizationRepository organizationRepository, IUserRepository userRepository)
        {
            _eventRepository = eventRepository;
            _organizationRepository = organizationRepository;
            _userRepository = userRepository;
        }
        // Implement methods from IEventService here


    }
}
