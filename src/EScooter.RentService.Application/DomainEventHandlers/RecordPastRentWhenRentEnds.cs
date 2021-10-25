using EasyDesk.CleanArchitecture.Application.Data;
using EasyDesk.CleanArchitecture.Application.Mediator;
using EasyDesk.CleanArchitecture.Application.Responses;
using EasyDesk.Tools;
using EScooter.RentService.Domain.Aggregates.CustomerAggregate;
using EScooter.RentService.Domain.Aggregates.PastRentAggregate;
using System.Threading.Tasks;
using static EasyDesk.CleanArchitecture.Application.Responses.ResponseImports;

namespace EScooter.RentService.Application.DomainEventHandlers
{
    /// <summary>
    /// A domain event handler that records the end of a rent creating a new <see cref="PastRent"/> with its outcome.
    /// </summary>
    public class RecordPastRentWhenRentEnds : NotificationHandlerBase<RentEndedEvent>
    {
        private readonly IPastRentRepository _pastRentRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordPastRentWhenRentEnds"/> class.
        /// </summary>
        /// <param name="pastRentRepository">The past rent repository.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        public RecordPastRentWhenRentEnds(IPastRentRepository pastRentRepository, IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _pastRentRepository = pastRentRepository;
        }

        /// <inheritdoc/>
        protected override Task<Response<Nothing>> Handle(RentEndedEvent ev)
        {
            var pastRent = new PastRent(
                ev.Rent.Id,
                ev.Rent.ScooterId,
                ev.Customer.Id,
                ev.Rent.RequestTimestamp,
                ev.Outcome);
            _pastRentRepository.Save(pastRent);
            return OkAsync;
        }
    }
}
