using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Kimado.Core;
using Kimado.Core.Contracts;
using BugAnalysis.DataService.Interfaces;
using BugAnalysis.FunctionalService.Interfaces;
using BugAnalysis.FunctionalService.Models;

namespace BugAnalysis.FunctionalService.Services
{
    public class BugAnalysisFunctionalService : FunctionalServiceBase, IBugAnalysisFunctionalService
    {
        private readonly IServiceContainer _serviceContainer;
        public BugAnalysisFunctionalService(IServiceContainer serviceContainer) : base(serviceContainer)
        {
            _serviceContainer = serviceContainer;
        }

		// TODO
		// IBugAnalysisFunctionalService implementation
	}
}
