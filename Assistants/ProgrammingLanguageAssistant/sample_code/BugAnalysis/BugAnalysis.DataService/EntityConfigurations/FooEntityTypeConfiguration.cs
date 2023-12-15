using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BugAnalysis.DataService.Models;

namespace BugAnalysis.DataService.EntityConfigurations
{
    public class FooEntityTypeConfiguration : IEntityTypeConfiguration<Foo>
    {
        public void Configure(EntityTypeBuilder<Foo> builder)
        {
			builder.ToTable("Foo", SoftwareAnalysisContext.DEFAULT_SCHEMA);
			builder.HasKey(t => t.Id);

			// TODO
			// Entity type definition...
			//
		}
	}
}
