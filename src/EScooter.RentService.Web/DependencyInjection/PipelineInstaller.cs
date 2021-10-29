using System;
using EasyDesk.CleanArchitecture.Web.DependencyInjection;
using EScooter.RentService.Application;
using EScooter.RentService.Infrastructure;

namespace EScooter.RentService.Web.DependencyInjection
{
    /// <summary>
    /// A service installer that configures the pipeline for this service.
    /// </summary>
    public class PipelineInstaller : PipelineInstallerBase
    {
        /// <inheritdoc/>
        protected override Type ApplicationAssemblyMarker => typeof(ApplicationMarker);

        /// <inheritdoc/>
        protected override Type InfrastructureAssemblyMarker => typeof(InfrastructureMarker);

        /// <inheritdoc/>
        protected override Type WebAssemblyMarker => typeof(Startup);

        /// <inheritdoc/>
        protected override bool UsesPublisher => true;

        /// <inheritdoc/>
        protected override bool UsesConsumer => true;
    }
}
