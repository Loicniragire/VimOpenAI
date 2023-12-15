using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kimado.Core;
using Kimado.Core.Contracts;
using BugAnalysis.DataService.Interfaces;
using BugAnalysis.DataService.Models;

namespace BugAnalysis.DataService.Services
{
    public class BugAnalysisDataService : DataServiceBase, IBugAnalysisDataService
    {
        private readonly IServiceContainer _serviceContainer;

        public BugAnalysisDataService(IServiceContainer serviceContainer) : base(serviceContainer)
        {
            _serviceContainer = serviceContainer;
        }

		// TODO
		// IBugAnalysisDataService implementation
		//
	}
}
