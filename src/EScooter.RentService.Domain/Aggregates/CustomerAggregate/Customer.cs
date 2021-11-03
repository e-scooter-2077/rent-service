using EasyDesk.CleanArchitecture.Domain.Metamodel;
using EasyDesk.CleanArchitecture.Domain.Metamodel.Results;
using EasyDesk.Tools;
using EasyDesk.Tools.Options;
using System;
using static EasyDesk.CleanArchitecture.Domain.Metamodel.Results.ResultImports;
using static EasyDesk.Tools.Options.OptionImports;

namespace EScooter.RentService.Domain.Aggregates.CustomerAggregate
{
    /// <summary>
    /// Represents a customer within the Rent Context, i.e. an entity able to perform rents.
    /// </summary>
    public class Customer : AggregateRoot
    {
        /// <summary>
        /// Creates a new <see cref="Customer"/>.
        /// This constructor should only be used for re-hydration by the infrastructure layer.
        /// </summary>
        /// <param name="id">The customer identifier.</param>
        /// <param name="ongoingRentId">The optional ongoing rent Id.</param>
        public Customer(Guid id, Option<Guid> ongoingRentId)
        {
            Id = id;
            OngoingRentId = ongoingRentId;
        }

        /// <summary>
        /// The unique identifier of this customer.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Returns the Id of the currently active rent for this customer, if any.
        /// </summary>
        public Option<Guid> OngoingRentId { get; private set; }

        private bool HasOngoingRent => OngoingRentId.IsPresent;

        /// <summary>
        /// Creates a new <see cref="Customer"/> given its identifier.
        /// </summary>
        /// <param name="id">The customer Id.</param>
        /// <returns>The new <see cref="Customer"/>.</returns>
        public static Customer Create(Guid id) => new(id, ongoingRentId: None);

        /// <summary>
        /// Starts a new rent for this customer.
        /// </summary>
        /// <param name="rentId">The rent Id.</param>
        /// <returns>
        /// <para>
        ///     <see cref="RentAlreadyOngoing"/>: if a rent is ongoing for this customer.
        /// </para>
        /// <para>
        ///     <see cref="Ok"/>: otherwise.
        /// </para>
        /// </returns>
        public Result<Nothing> StartRent(Guid rentId)
        {
            return RequireFalse(HasOngoingRent, () => new RentAlreadyOngoing())
                .IfSuccess(_ => OngoingRentId = rentId);
        }

        /// <summary>
        /// Ends the ongoing rent for this customer.
        /// </summary>
        /// <returns>
        /// <para>
        ///     <see cref="NoOngoingRent"/>: if no rent is ongoing for this customer.
        /// </para>
        /// <para>
        ///     <see cref="Ok"/>: otherwise.
        /// </para>
        /// </returns>
        public Result<Nothing> EndRent()
        {
            return RequireTrue(HasOngoingRent, () => new NoOngoingRent())
                .IfSuccess(_ => OngoingRentId = None);
        }
    }

    /// <summary>
    /// An error returned when trying to request a rent while another one is still ongoing.
    /// </summary>
    public record RentAlreadyOngoing : DomainError;

    /// <summary>
    /// An error returned when trying to perform an action on the ongoing rent while not having one.
    /// </summary>
    public record NoOngoingRent : DomainError;
}
