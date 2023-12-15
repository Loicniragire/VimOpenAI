using FluentValidation;
using BugAnalysis.Api.Models.Requests;

namespace BugAnalysis.Api.Validators
{
	public class FooRequestValidator: AbstractValidator<FooRequest>
	{
		public FooRequestValidator()
		{
			/*
			 * Validation rules
			 */
		}
	}

}
